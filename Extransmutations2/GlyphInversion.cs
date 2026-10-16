using System;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class GlyphInversion { 
  internal static HexIndex CALC_SPOT = new(1, 0);
  internal static HexIndex TRAN_SPOT = new(0, 0);
  internal static Identifier ID = IdObj("cardinal-inversion");
  internal static GlyphRecipe Recipe(AtomType from,AtomType to) => new(
    (sim,part) => {
      if(sim.GetAtomReference(part,TRAN_SPOT,false,out var transAtom) 
      && transAtom.atomType == from
      && sim.GetAtomReference(part,CALC_SPOT,false, out var calcAtom)
      && calcAtom.atomType == from
      ) {
        sim.RecipeInputs[TRAN_SPOT] = transAtom;
        sim.RecipeInputs[CALC_SPOT] = calcAtom;
        sim.RecipeOutputs[CALC_SPOT] = AtomTypes.salt;
        sim.RecipeOutputs[TRAN_SPOT] = to;
        return true;
      }
      return false;
    }
  );
  internal static void LoadContent() {
    PartType cardinalInversion = new() {
      Id = ID,
      cost = 10, // Cost
      isFullHexCover = true, // Is a glyph (?)
      glowTexture = Resources.cardinalInversionGlow, // Shadow/glow
      strokeTexture = Resources.cardinalInversionStroke, // Stroke/outline
      baseTexture = Resources.inversionPanel, // Panel icon
      hoverTexture = Resources.inversionPanel, // Hovered panel icon
      glyphHexes = [
                new(0, 0),
                new(1, 0),
            ], // Spaces used 
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPuzzlePermission(Instance,ID.name,ID.name,Section);
    QApi.AddPartType(Instance, cardinalInversion, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = new(123f, 47f);// new Vector2(42f, 49f) + new Vector2(0f,-1f);
      renderer.RenderBase(Resources.cardinalInversionBase, new Vector2(-1, -1), centre, 0);
      renderer.RenderRotating(Resources.bowlTexture, CALC_SPOT, Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, TRAN_SPOT, Vector2.Zero);
      renderer.RenderUpright(Resources.calcifySpotTexture, CALC_SPOT, Vector2.Zero);
      renderer.RenderUpright(Resources.transmutationInversionSpotTexture, TRAN_SPOT, Vector2.Zero);
    });
    QApi.BindGlyphCylce(cardinalInversion, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) => {
      if(recipe.Predicate.InvokeAndClear(sim,part)) {
        TryTransmute(sim,part,CALC_SPOT);
        TryTransmute(sim,part,TRAN_SPOT);
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CALC_SPOT) ),
          Resources.calcifyAnimation, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(TRAN_SPOT) ),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(TRAN_SPOT) ),
          Resources.inversionGlowArray, 30f, Vector2.Zero, 0f)
        );
        Resources.cardinalInversionSound.isPlaying = false; 
        Resources.cardinalInversionSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }));
    (string, GlyphRecipe)[] recipes = [
      ("water-fire",Recipe(AtomTypes.water,AtomTypes.fire)),
      ("fire-water",Recipe(AtomTypes.fire,AtomTypes.water)),
      ("earth-air",Recipe(AtomTypes.earth,AtomTypes.air)),
      ("air-earth",Recipe(AtomTypes.air,AtomTypes.earth)),
    ];
    AddAllRecipes(ID, recipes);
    QApi.AddPartTypeToPanel(cardinalInversion, false);
  }
}