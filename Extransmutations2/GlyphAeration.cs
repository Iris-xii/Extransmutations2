using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphAeration {

  internal static HexIndex SALT_SPOT = new(0, 0);
  internal static HexIndex OUT_SPOT = new(1, 0);
  internal static Identifier ID = IdObj("aeration");

  internal static GlyphRecipe Recipe(AtomType from, AtomType to) {
    return new((sim, part) => {
      if (sim.GetAtomReference(part, SALT_SPOT, false, out var salt)
      && salt.atomType == from
      && !salt.isHeldByArm
      && !salt.inMultiAtomMolecule
      ) {
        sim.RecipeInputs[SALT_SPOT] = salt;
        sim.RecipeOutputs[OUT_SPOT] = to;
        return true;
      }
      return false;
    });
  }
  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    PartType glyphAeration = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.aerationGlow,
      strokeTexture = Resources.aerationStroke,
      baseTexture = Resources.aerationPanel,
      hoverTexture = Resources.aerationPanel,
      glyphHexes = [
                new(0, 0),
                new(1, 0),
                new(-1, 0),
                new(-1,1),
                new(0,1),
                new(0,-1),
                new(1,-1)
            ],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPartType(Instance, glyphAeration, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.aerationBase.GetHalfSize(); 
      renderer.RenderBase(Resources.aerationBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderBase(Resources.aerationBaseDetail, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlHole, new HexIndex(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.saltBowlGlyphSpotWhite, new HexIndex(0, 0), Vector2.Zero);
      // iris 
      DrawIris(part, pos, editor, renderer, Resources.airIris);
    });
    QApi.BindGlyphCycle(glyphAeration, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if (pss.isProcessing) {
        Molecule newMol = new();
        newMol.AddAtom(new(pss.processingAtoms[0]), part.InFrontBy(OUT_SPOT));
        sim.molecules.Add(newMol);
      }
      if (recipe.Predicate.InvokeAndClear(sim, part)) {
        TryTransmute(sim, part, SALT_SPOT, DelAnim.CONSUME);
        if (sim.RecipeOutputs.GetValueOrDefault(OUT_SPOT) is AtomType outA) {//hardcoded
          pss.processingAtoms ??= [null];
          pss.processingAtoms[0] = outA;
          pss.isProcessing = true;
          Resources.aerationSound.isPlaying = false;
          Resources.aerationSound.PlaySound(sim.solutionEditor.method_506());
          sim.solutionEditor.consumptionEffects.Add(
            new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
            HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(new(0, 0))),
            Resources.aerationBreathe, 30f, Vector2.Zero, part.GetRotation().ToRadians())
          );
        }
        return true;
      }
      return false;
    }));
    QApi.AddPartTypeToPanel(glyphAeration, false);
    (string, GlyphRecipe)[] recipes = [
      ("aeration-air",Recipe(AtomTypes.salt,AtomTypes.air))
    ];
    AddAllRecipes(ID, recipes);
  }
}