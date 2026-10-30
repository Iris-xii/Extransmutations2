using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static IrisUtils;
using static Extransmutations2;

internal static class GlyphRestoration {

  internal static HexIndex CARD_BOWL = new(1, 0);
  internal static HexIndex ICHOR_BOWL = new(0, 0);
  internal static Identifier ID = IdObj("restoration");
  internal static PartType TYPE = new() {
    Id = ID,
    cost = 20, // expensive
    isFullHexCover = true, // Is a glyph (?)
    glowTexture = Resources.restorationGlow, // Shadow/glow
    strokeTexture = Resources.restorationStroke, // Stroke/outline
    baseTexture = Resources.restorationPanel, // Panel icon
    hoverTexture = Resources.restorationPanel, // Hovered panel icon
    glyphHexes = [new(0, 0), new(1, 0)],
    CustomPermissionCheck = perms => perms.Contains(ID),
  };

  internal static GlyphRecipe Recipe(AtomType destroyableCardinal, Func<Sim, Part, bool>? check = null) {
    check ??= AlwaysTrueCheck;
    return new((sim, part) => {
      if (!check(sim, part)) { return false; }
      if (sim.GetAtomReference(part, CARD_BOWL, false, out var cardRef)
      && cardRef.atomType == destroyableCardinal
      && sim.GetAtomReference(part, ICHOR_BOWL, false, out var ichorRef)
      && ichorRef.atomType == Ichor.Type) {
        sim.RecipeInputs[CARD_BOWL] = cardRef;
        sim.RecipeInputs[ICHOR_BOWL] = ichorRef;
        sim.RecipeOutputs[CARD_BOWL] = cardRef.atomType;
        sim.RecipeOutputs[ICHOR_BOWL] = ichorRef.atomType;
        return true;
      }
      return false;
    });
  }

  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, IchorSection);
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.restorationBase.GetHalfSize();
      renderer.RenderBase(Resources.restorationBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlTexture, new(0, 0), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, new(1, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.ichorIcon, new(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.anyCardinal, new(1, 0), Vector2.Zero);
    });
    QApi.AddCycleEvent(new(CycleEvent.CycleEventExecutionType.First, (sim, _) => {
      var hasPartPerm = (sim.GetSolution().GetPuzzle().CustomPermissions?.Contains(ID))??false;
      if(!hasPartPerm) {return;};
      foreach (var part in sim.GetSolution().parts) {
        if (part.GetType().Id != TYPE.Id) { continue; }
        if (!GlyphRecipe.Recipes.TryGetValue(part.GetType().Id, out var recipeDict)) { continue; }
        foreach (var recipeKV in recipeDict) {
          Identifier recipeName = recipeKV.Key;
          GlyphRecipe recipe = recipeKV.Value;
          if (recipe.Predicate.InvokeAndClear(sim, part)) {
            if (sim.RecipeOutputs.GetValueOrDefault(ICHOR_BOWL) is not AtomType ichorTarget) { continue; }
            if (sim.RecipeOutputs.GetValueOrDefault(CARD_BOWL) is not AtomType cardTarget) { continue; }
            if (sim.RecipeInputs.GetValueOrDefault(ICHOR_BOWL) is not AtomReference ichorMolRef) { continue; }
            if (sim.RecipeInputs.GetValueOrDefault(CARD_BOWL) is not AtomReference cardMolRef) { continue; }
            Molecule ichorMol = ichorMolRef.molecule;
            Molecule cardMol = cardMolRef.molecule;
            var ichorAtomsAll = ichorMol.GetAtoms().Where(a => a.Value.atomType == ichorTarget).ToArray();
            var cardAtomsAll = cardMol.GetAtoms().Where(a => a.Value.atomType == cardTarget).ToArray();
            if (ichorAtomsAll.Length != cardAtomsAll.Length) { continue; }

            foreach (var KV in ichorAtomsAll) {
              HexIndex atomPos = KV.Key;
              Atom atom = KV.Value;
              ichorMol.RemoveAtom(atomPos);
              sim.solutionEditor.glyphEffects.Add(
                new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
                HexGrid.standardGrid.ToPixelCoords(atomPos),
                Resources.destroyAnim, 30f, new Vector2(-72, 0), 0f)
              );
            }
            foreach (var KV in cardAtomsAll) {
              HexIndex atomPos = KV.Key;
              Atom atom = KV.Value;
              cardMol.RemoveAtom(atomPos);
              sim.solutionEditor.glyphEffects.Add(
                new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
                HexGrid.standardGrid.ToPixelCoords(atomPos),
                Resources.destroyAnim, 30f, new Vector2(-72, 0), 0f)
              );
            }
            sim.solutionEditor.consumptionEffects.Add(
              new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
              HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CARD_BOWL)),
              Resources.bowlGlow, 30f, Vector2.Zero, 0f)
            );
            sim.solutionEditor.consumptionEffects.Add(
              new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
              HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(ICHOR_BOWL)),
              Resources.bowlGlow, 30f, Vector2.Zero, 0f)
            );
            sim.solutionEditor.consumptionEffects.Add(
              new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
              HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CARD_BOWL)),
              Resources.anyGlowArray, 30f, Vector2.Zero, 0f)
            );
            sim.solutionEditor.consumptionEffects.Add(
              new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
              HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(ICHOR_BOWL)),
              Resources.ichorGlowArray, 30f, Vector2.Zero, 0f)
            );
            Resources.restorationSound.isPlaying = false;
            Resources.restorationSound.PlaySound(sim.solutionEditor.method_506());
            break;
          }
        }
      }
    }));
    (string, GlyphRecipe)[] recipes = [
      ("restore-water",Recipe(AtomTypes.water)),
      ("restore-fire",Recipe(AtomTypes.fire)),
      ("restore-earth",Recipe(AtomTypes.earth)),
      ("restore-air",Recipe(AtomTypes.air)),
    ];
    AddAllRecipes(ID, recipes);
    QApi.AddPartTypeToPanel(TYPE, false);
  }
}