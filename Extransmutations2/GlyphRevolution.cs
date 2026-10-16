using System;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class GlyphRevolution {
  internal enum Variant {
    LINE, TRI
  }
  private static T PerVar<T>(Variant v, T ifLine, T ifTri) =>
    v switch {
      Variant.LINE => ifLine,
      Variant.TRI => ifTri,
      _ => throw new NotImplementedException(),
    };
  internal static string ID(Variant v) =>
  PerVar(v, ifLine: "cardinal-revolution-line", ifTri: "cardinal-revolution-tri");
  internal static Identifier NamespacedID(Variant v) => new(Instance.ModId, ID(v));
  internal static Texture Glow(Variant v) => PerVar(v,
    ifLine: Resources.cardinalCycleGlow,
    ifTri: Resources.cardinalCycleGlowV2
    );
  internal static Texture Outline(Variant v) => PerVar(v,
    ifLine: Resources.cardinalCycleStroke,
    ifTri: Resources.cardinalCycleStrokeV2
    );
  internal static Texture Panel(Variant v) => PerVar(v,
    ifLine: Resources.cyclePanel,
    ifTri: Resources.cyclePanelV2
    );
  internal static HexIndex CalcSpot1(Variant v) => PerVar(v,
    ifLine: new HexIndex(-1, 0),
    ifTri: new HexIndex(0, 0)
    );
  internal static HexIndex TransmuteSpot(Variant v) => PerVar(v,
    ifLine: new HexIndex(1, 0),
    ifTri: new HexIndex(1, 0)
    );
  internal static HexIndex CalcSpot2(Variant v) => PerVar(v,
    ifLine: new HexIndex(0, 0),
    ifTri: new HexIndex(0, 1)
    );
  internal static PartType PType(Variant v) => new() {
    isFullHexCover = true,
    Id = NamespacedID(v),
    cost = 10, // Cost 
    glowTexture = Glow(v), // Shadow/glow
    strokeTexture = Outline(v), // Stroke/outline
    baseTexture = Panel(v), // Panel icon
    hoverTexture = Panel(v), // Hovered panel icon
    glyphHexes = [ // Spaces used
      CalcSpot1(v),
      TransmuteSpot(v),
      CalcSpot2(v),
    ],
    CustomPermissionCheck = perms => perms.Contains(NamespacedID(v)),
  };

  internal static void RenderLine(
  Part part, Vector2 position, SolutionEditorBase editor, PartRenderer helper) {
    var v = Variant.LINE;
    Texture b = Resources.cardinalCycleBase;
    Vector2 center = b.GetHalfSize();
    helper.RenderBase(b, center);
    helper.RenderRotating(Resources.bowlTexture, CalcSpot1(v), Vector2.Zero);
    helper.RenderRotating(Resources.bowlTexture, CalcSpot2(v), Vector2.Zero);
    helper.RenderRotating(Resources.bowlTexture, TransmuteSpot(v), Vector2.Zero);
    helper.RenderUpright(Resources.calcifySpotTexture, CalcSpot1(v), Vector2.Zero);
    helper.RenderUpright(Resources.calcifySpotTexture, CalcSpot2(v), Vector2.Zero);
    helper.RenderUpright(Resources.transmutationCycleSpotTexture, TransmuteSpot(v), Vector2.Zero);
  }
  internal static void RenderTri(Part part, Vector2 position, SolutionEditorBase editor, PartRenderer helper) {
    var v = Variant.TRI;
    Texture b = Resources.cardinalCycleBaseV2;
    Vector2 center = b.GetHalfSize();
    helper.RenderBase(b, center);
    helper.RenderRotating(Resources.bowlTexture, CalcSpot1(v), Vector2.Zero);
    helper.RenderRotating(Resources.bowlTexture, CalcSpot2(v), Vector2.Zero);
    helper.RenderRotating(Resources.bowlTexture, TransmuteSpot(v), Vector2.Zero);
    helper.RenderUpright(Resources.calcifySpotTexture, CalcSpot1(v), Vector2.Zero);
    helper.RenderUpright(Resources.calcifySpotTexture, CalcSpot2(v), Vector2.Zero);
    helper.RenderUpright(Resources.transmutationCycleSpotTexture, TransmuteSpot(v), Vector2.Zero);
  }

  internal static GlyphRecipe Recipe(Variant v, AtomType cardinalFrom, AtomType cardinalTo) {
    return new((sim, part) => {
      if (sim.GetAtomReference(part, CalcSpot1(v), false, out var calc1)
      && calc1.atomType == cardinalFrom
      && sim.GetAtomReference(part, CalcSpot2(v), false, out var calc2)
      && calc2.atomType == cardinalFrom
      && sim.GetAtomReference(part, TransmuteSpot(v), false, out var trans)
      && trans.atomType == cardinalFrom
      ) {
        sim.RecipeInputs[CalcSpot1(v)] = calc1;
        sim.RecipeInputs[CalcSpot2(v)] = calc2;
        sim.RecipeInputs[TransmuteSpot(v)] = trans;
        sim.RecipeOutputs[CalcSpot1(v)] = AtomTypes.salt;
        sim.RecipeOutputs[CalcSpot2(v)] = AtomTypes.salt;
        sim.RecipeOutputs[TransmuteSpot(v)] = cardinalTo;
        return true;
      }
      return false;
    });
  }

  internal static void LoadContent(Variant v) {
    var type = PType(v);
    if (v == Variant.LINE) {
      QApi.AddPartType(Extransmutations2.Instance, type, ID(v), RenderLine);
    }
    else if (v == Variant.TRI) {
      QApi.AddPartType(Extransmutations2.Instance, type, ID(v), RenderTri);
    }
    QApi.AddPartTypeToPanel(type, false);
    QApi.AddPuzzlePermission(Extransmutations2.Instance,
      ID(v),
      displayName: ID(v), sectionName: Section);

    (string, GlyphRecipe)[] recipesDef = [
      ("air-water",Recipe(v, AtomTypes.air, AtomTypes.water)),
      ("water-earth",Recipe(v, AtomTypes.water, AtomTypes.earth)),
      ("earth-fire",Recipe(v, AtomTypes.earth, AtomTypes.fire)),
      ("fire-air",Recipe(v, AtomTypes.fire, AtomTypes.air)),
      //("weird",new((sim, part) => {
      //  if(sim.GetAtomReference(part,TransmuteSpot(v),false,out var gold)
      //  && gold.atomType == AtomTypes.gold) {
      //    sim.RecipeInputs[TransmuteSpot(v)] = gold;
      //    sim.RecipeOutputs[CalcSpot1(v)] = AtomTypes.quicksilver;
      //    return true;
      //  }
      //  return false;
      //}))
    ];
    foreach (var r in recipesDef) {
      QApi.AddRecipe(Instance,
        recipe: r.Item2,
        recipeId: new(Instance.ModId, $"{ID(v)}-{r.Item1}"),
        partId: NamespacedID(v));
    }

    QApi.BindGlyphCylce(type, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) => {
      if (recipe.Predicate.InvokeAndClear(sim, part)) {
        TryTransmute(sim,part,CalcSpot1(v));
        TryTransmute(sim,part,CalcSpot2(v));
        TryTransmute(sim,part,TransmuteSpot(v));
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CalcSpot1(v)) ),
          Resources.calcifyAnimation, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CalcSpot2(v))),
          Resources.calcifyAnimation, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(TransmuteSpot(v))),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(TransmuteSpot(v))),
          Resources.cycleGlowArray, 30f, Vector2.Zero, 0f)
        ); 
        Resources.cardinalRotationSound.isPlaying = false; 
        Resources.cardinalRotationSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }
    ));
  }
}