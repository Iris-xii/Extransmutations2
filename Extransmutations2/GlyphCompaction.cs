using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphCompaction {
  internal static Identifier ID = IdObj("compaction");
  internal static HexIndex BOWL = new(0,0);
  internal static GlyphRecipe Recipe(AtomType from,AtomType to) => new(
    (sim,part) => {
      if(sim.GetAtomReference(part,BOWL,false,out var bowlAtom)
      && bowlAtom.atomType == from) {
        int bonds = 0;
        foreach(var bond in bowlAtom.molecule.GetBonds()) {
          if(bond.hexPos1 != bowlAtom.pos && bond.hexPos2 != bowlAtom.pos) continue;
          bonds += bond.GetBondTypes().Count;
        }
        sim.RecipeInputs[BOWL] = bowlAtom;
        sim.RecipeOutputs[BOWL] = to;
        return bonds >= 3;
      }
      return false;
    }
  );
  internal static void LoadContent() {
    PartType glyphCompaction = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.compactionGlow,
      strokeTexture = Resources.compactionStroke,
      baseTexture = Resources.compactionPanel,
      hoverTexture = Resources.compactionPanel,
      glyphHexes = [new(0, 0)],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    QApi.AddPartType(Instance, glyphCompaction, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.compactionBase.GetHalfSize();
      renderer.RenderBase(Resources.compactionBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlTexture, new(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.earthGlyph, new(0, 0), Vector2.Zero);
    });
    QApi.BindGlyphCycle(glyphCompaction, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if(recipe.Predicate.InvokeAndClear(sim,part)) {
        TryTransmute(sim,part,BOWL);
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(BOWL)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        Resources.compactionSound.isPlaying = false;
        Resources.compactionSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }));
    QApi.AddPartTypeToPanel(glyphCompaction, false);
    (string, GlyphRecipe)[] recipes = [
      ("compact-salt",Recipe(AtomTypes.salt,AtomTypes.earth))
    ];
    AddAllRecipes(ID, recipes);
  }
}