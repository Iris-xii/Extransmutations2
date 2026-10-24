using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphQuintConjunction {
  const uint WATER_FLAG = 1, FIRE_FLAG = 2, EARTH_FLAG = 4, AIR_FLAG = 8;
  const uint PLAYED_LAST_CHIME_FLAG = 16;
  const uint FLAG_ALL = WATER_FLAG | FIRE_FLAG | EARTH_FLAG | AIR_FLAG;
  internal static HexIndex CARD_HOLE = new(0, 0);
  internal static HexIndex QUINT_OUTPUT = new(1, 0);
  internal static Identifier ID = IdObj("quint-conjunction");
  internal static PartType TYPE = new() {
    Id = ID,
    cost = 20 + ARM_COST*2,
    isFullHexCover = true,
    glowTexture = Resources.quintConjunction.glow,
    strokeTexture = Resources.quintConjunction.stroke,
    baseTexture = Resources.quintConjunction.tray,
    hoverTexture = Resources.quintConjunction.tray,
    glyphHexes = [CARD_HOLE, QUINT_OUTPUT],
    CustomPermissionCheck = perms => perms.Contains(ID),
  };
  internal static GlyphRecipe Recipe(AtomType sinkType, uint setsFlag) => new((sim, part) => {
    var pPart = (patch_Part)((object)part);
    if (sim.GetAtomReference(part, CARD_HOLE, false, out var maybeCard)
    && maybeCard.atomType == sinkType
    && !maybeCard.inMultiAtomMolecule
    && !maybeCard.isHeldByArm
    && (pPart.Extransmutations2Uint & setsFlag) == 0
    ) {
      sim.RecipeInputs[CARD_HOLE] = maybeCard;
      pPart.Extransmutations2Uint |= setsFlag;
      if ((pPart.Extransmutations2Uint & FLAG_ALL) == FLAG_ALL && !sim.HasAtomAt(part, QUINT_OUTPUT, true)) {
        sim.RecipeOutputs[QUINT_OUTPUT] = AtomTypes.quintessence;
      }
      return true;
    }
    else if ((pPart.Extransmutations2Uint & FLAG_ALL) == FLAG_ALL && !sim.HasAtomAt(part, QUINT_OUTPUT, true)) {
      sim.RecipeOutputs[QUINT_OUTPUT] = AtomTypes.quintessence;
      return true;
    }
    return false;
  });
  internal static void RenderState(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
    var pPart = (patch_Part)((object)part);
    bool waterActive, fireActive, earthActive, airActive;
    if (editor.GetSimPlayState() == SimPlayState.Stopped) {
      (waterActive, fireActive, earthActive, airActive) = (false, false, false, false);
    }
    else {
      waterActive = (pPart.Extransmutations2Uint & WATER_FLAG) > 0;
      fireActive = (pPart.Extransmutations2Uint & FIRE_FLAG) > 0;
      earthActive = (pPart.Extransmutations2Uint & EARTH_FLAG) > 0;
      airActive = (pPart.Extransmutations2Uint & AIR_FLAG) > 0;
    }
    (bool, Texture, Texture)[] textureMap = [
      (waterActive,Assets.textures.parts.unification.symbol_water_active,Assets.textures.parts.unification.symbol_water_inactive),
      (fireActive,Assets.textures.parts.unification.symbol_fire_active,Assets.textures.parts.unification.symbol_fire_inactive),
      (earthActive,Assets.textures.parts.unification.symbol_earth_active,Assets.textures.parts.unification.symbol_earth_inactive),
      (airActive,Assets.textures.parts.unification.symbol_air_active,Assets.textures.parts.unification.symbol_air_inactive),
    ];
    foreach (var entry in textureMap) {
      Texture which = entry.Item1 ? entry.Item3 : entry.Item2;
      renderer.RenderUpright(which, CARD_HOLE, Vector2.Zero);
    }
  }
  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section,length: 3);
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.quintConjunction.@base.GetHalfSize();
      renderer.RenderBase(Resources.quintConjunction.@base, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Assets.textures.parts.animismus.input, CARD_HOLE, Vector2.Zero);
      DrawIris(part, pos, editor, renderer, QUINT_OUTPUT, Assets.textures.parts.iris_full);
      renderer.RenderUpright(Assets.textures.parts.unification.symbol_divider, CARD_HOLE, Vector2.Zero);
      RenderState(part, pos, editor, renderer);
    });
    QApi.BindGlyphCycle(TYPE, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      var pPart = (patch_Part)((object)part);
      if (sim.GetCycle() == 0) { pPart.Extransmutations2Uint = 0; }
      if (pss.isProcessing) {
        Molecule newMol = new();
        newMol.AddAtom(new(pss.processingAtoms[0]), part.InFrontBy(QUINT_OUTPUT));
        sim.molecules.Add(newMol);
        pss.isProcessing = false;
        Resources.quintConjunctionFinishSound.EPlay(sim);
      }
      if (isCycleStart && recipe.Predicate.InvokeAndClear(sim, part)) {
        TryTransmute(sim, part, CARD_HOLE, delAnim: DelAnim.CONSUME);
        if (sim.RecipeOutputs.GetValueOrDefault(QUINT_OUTPUT) is AtomType quintType) {
          TryTransmute(sim, part, QUINT_OUTPUT, prodType: ProdType.GLYPH_PRODUCE);
          int stage = BitOperations.PopCount(pPart.Extransmutations2Uint & FLAG_ALL) - 1;
          if (stage >= Resources.quintConjunctionSinkSound.Length) { stage = 0; } //don't crash over sounds
          if((pPart.Extransmutations2Uint & PLAYED_LAST_CHIME_FLAG) == 0) {Resources.quintConjunctionSinkSound[stage].EPlay(sim);}
          if(stage == 3) {pPart.Extransmutations2Uint |= PLAYED_LAST_CHIME_FLAG;}

          pPart.Extransmutations2Uint = 0;
        }
        else {
          int stage = BitOperations.PopCount(pPart.Extransmutations2Uint & FLAG_ALL) - 1;
          if (stage >= Resources.quintConjunctionSinkSound.Length) { stage = 0; } //don't crash over sounds
          if((pPart.Extransmutations2Uint & PLAYED_LAST_CHIME_FLAG) == 0) {Resources.quintConjunctionSinkSound[stage].EPlay(sim);}
          if(stage == 3) {pPart.Extransmutations2Uint |= PLAYED_LAST_CHIME_FLAG;}
        }
        return true;
      }
      return false;
    }));
    QApi.AddPartTypeToPanel(TYPE, false);
    (string, GlyphRecipe)[] recipes = [
      ("sink-water",Recipe(AtomTypes.water,WATER_FLAG)),
      ("sink-fire",Recipe(AtomTypes.fire,FIRE_FLAG)),
      ("sink-earth",Recipe(AtomTypes.earth,EARTH_FLAG)),
      ("sink-air",Recipe(AtomTypes.air,AIR_FLAG)),
    ];
    AddAllRecipes(ID, recipes);
  }
}