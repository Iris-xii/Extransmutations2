using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphDejection {

  internal static HexIndex[] PROD_HOLE = [new(1, 0), new(0, 1)];
  internal static HexIndex CARDINAL_BOWL = new(0, 0);
  internal static Identifier ID = IdObj("dejection");

  internal static GlyphRecipe Recipe(AtomType from, AtomType to) => new(
    (sim, part) => {
      if (sim.GetAtomReference(part, CARDINAL_BOWL, false, out var initialCard)
      && initialCard.atomType == from
      && PROD_HOLE.All(h => !sim.HasAtomAt(part, h, true))) {
        sim.RecipeInputs[CARDINAL_BOWL] = initialCard;
        sim.RecipeOutputs[CARDINAL_BOWL] = Ichor.Type;
        foreach (var hole in PROD_HOLE) { sim.RecipeOutputs[hole] = to; }
        return true;
      }
      return false;
    }
  );
  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, IchorSection);
    PartType glyphDejection = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.dejectionGlow,
      strokeTexture = Resources.dejectionStroke,
      baseTexture = Resources.dejectionPanel,
      hoverTexture = Resources.dejectionPanel,
      glyphHexes = [
                new(0, 0),
                new(1, 0),
                new(0, 1),
        ],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPartType(Instance, glyphDejection, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.dejectionBase.GetHalfSize();
      renderer.RenderBase(Resources.dejectionBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlHole, new HexIndex(1, 0), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlHole, new HexIndex(0, 1), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, new HexIndex(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.anyCardinal, new HexIndex(0, 0), Vector2.Zero);
      for (int i = 0; i < PROD_HOLE.Length; i++) {
        var hole = PROD_HOLE[i];
        DrawIris(part, pos, editor, renderer, hole, Resources.dejectionIris, processingIndex: i);
      }
    });
    QApi.BindGlyphCycle(glyphDejection, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if (pss.isProcessing) {
        for (int i = 0; i < PROD_HOLE.Length; i++) {
          var hole = PROD_HOLE[i];
          AtomType target = pss.processingAtoms[i];
          var mol = new Molecule();
          mol.AddAtom(new(target), part.InFrontBy(hole));
          sim.molecules.Add(mol);
        }
        pss.isProcessing = false;
      }
      if (!isCycleStart || !recipe.Predicate.InvokeAndClear(sim, part)) return false;
      for (int i = 0; i < PROD_HOLE.Length; i++) {
        var hole = PROD_HOLE[i];
        TryTransmute(sim, part, hole, prodType: ProdType.GLYPH_PRODUCE, processingIndex: i);
      }
      TryTransmute(sim, part, CARDINAL_BOWL);
      sim.solutionEditor.consumptionEffects.Add(
        new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
        HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CARDINAL_BOWL)),
        Resources.bowlGlow, 30f, Vector2.Zero, 0f)
      );
      sim.solutionEditor.consumptionEffects.Add(
        new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
        HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(CARDINAL_BOWL)),
        Resources.anyGlowArray, 30f, Vector2.Zero, 0f)
      );
      Resources.dejectionSound.isPlaying = false;
      Resources.dejectionSound.PlaySound(sim.solutionEditor.method_506());
      return true;
    }));
    (string, GlyphRecipe)[] recipes = [
      ("fire-earth", Recipe(AtomTypes.fire,AtomTypes.earth)),
      ("earth-water",Recipe(AtomTypes.earth,AtomTypes.water)),
      ("water-air",  Recipe(AtomTypes.water,AtomTypes.air)),
      ("air-fire",   Recipe(AtomTypes.air,AtomTypes.fire))
    ];
    AddAllRecipes(ID, recipes);
    QApi.AddPartTypeToPanel(glyphDejection, false);
  }
}