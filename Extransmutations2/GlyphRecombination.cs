using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphRecombination {
  internal static Identifier ID = IdObj("recombination");
  internal static HexIndex[] SALT_HOLE = [new(1, 0), new(0, 1)];
  internal static HexIndex ICHOR_BOWL = new(0, 0);

  internal static GlyphRecipe Recipe(AtomType saltElem, AtomType ichorToNeutralize, AtomType neutralizeTo,
  Func<Sim, Part, bool>? check = null) {
    check ??= AlwaysTrueCheck;
    return new((sim, part) => {
      if(!check(sim,part)) {return false;}
      if (sim.GetAtomReference(part, SALT_HOLE[0], false, out var salt1)
      && salt1.atomType == saltElem
      && !salt1.isHeldByArm
      && !salt1.inMultiAtomMolecule
      && sim.GetAtomReference(part, SALT_HOLE[1], false, out var salt2)
      && salt2.atomType == saltElem
      && !salt2.isHeldByArm
      && !salt2.inMultiAtomMolecule
      && sim.GetAtomReference(part, ICHOR_BOWL, false, out var ichorRef)
      && ichorRef.atomType == ichorToNeutralize
      ) {
        sim.RecipeInputs[SALT_HOLE[0]] = salt1;
        sim.RecipeInputs[SALT_HOLE[1]] = salt2;
        sim.RecipeInputs[ICHOR_BOWL] = ichorRef;
        sim.RecipeOutputs[ICHOR_BOWL] = neutralizeTo;
        return true;
      }
      return false;
    });
  }

  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, IchorSection);
    PartType glyphRecombination = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.dejectionGlow,
      strokeTexture = Resources.dejectionStroke,
      baseTexture = Resources.recombinationPanel,
      hoverTexture = Resources.recombinationPanel,
      glyphHexes = [
                new(0, 0),
                new(1, 0),
                new(0, 1),
        ],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPartType(Instance, glyphRecombination, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.dejectionBase.GetHalfSize();
      renderer.RenderBase(Resources.dejectionBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlHole, new HexIndex(1, 0), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlHole, new HexIndex(0, 1), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, new HexIndex(0, 0), Vector2.Zero);

      renderer.RenderUpright(Resources.ichorIcon, new HexIndex(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.saltBowlGlyphSpotWhite, new HexIndex(1, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.saltBowlGlyphSpotWhite, new HexIndex(0, 1), Vector2.Zero);
    });
    QApi.BindGlyphCycle(glyphRecombination, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if (recipe.Predicate.InvokeAndClear(sim, part)) {
        foreach (var saltHole in SALT_HOLE) { TryTransmute(sim, part, saltHole, delAnim: DelAnim.CONSUME); }
        TryTransmute(sim, part, ICHOR_BOWL);

        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(ICHOR_BOWL)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(ICHOR_BOWL)),
          Resources.ichorGlowArray, 30f, Vector2.Zero, 0f)
        );
        Resources.recombinationSound.isPlaying = false;
        Resources.recombinationSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }));
    (string, GlyphRecipe)[] recipes = [
      ("neutralize-ichor",Recipe(AtomTypes.salt,Ichor.Type,AtomTypes.salt))
    ];
    AddAllRecipes(ID, recipes);
    QApi.AddPartTypeToPanel(glyphRecombination, false);
  }

}