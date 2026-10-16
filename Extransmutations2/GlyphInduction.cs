using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class GlyphInduction {
  internal static Identifier ID = IdObj("induction");
  internal static HexIndex BOWL = new(0, 0);
  internal static HexIndex DEAD_HEX = new(-1, 0);
  internal static HexIndex HOOK = new(1, 0);
  internal static PartType TYPE = null!;
  internal static void LoadContent() {
    TYPE = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.inductionGlow,
      strokeTexture = Resources.inductionStroke,
      baseTexture = Resources.inductionPanel,
      hoverTexture = Resources.inductionPanel,
      glyphHexes = [new(0, 0), new(-1, 0)],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    PartType glyphInduction = TYPE;
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    QApi.AddPartType(Instance, glyphInduction, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.inductionBase.GetHalfSize();
      renderer.RenderBase(Resources.inductionBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlTexture, new(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.fireGlyph, new(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.markerLighting, new(-1, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.markerDetails, new(-1, 0), Vector2.Zero);
    });
    QApi.AddPartTypeToPanel(glyphInduction, false);
    QApi.AddCycleEvent(new(CycleEvent.CycleEventExecutionType.First, First));
    QApi.AddCycleEvent(new(CycleEvent.CycleEventExecutionType.Last, Last));
  }

  internal static Dictionary<Atom, AtomType> inductionDict = [];
  internal static HashSet<HexIndex> inductionBowls = [];
  internal static void First(Sim sim, CycleEvent.CycleEventExecutionType executionType) {
    inductionDict = [];
    foreach (var mol in sim.molecules) {
      foreach (var atom in mol.GetAtoms()) {
        inductionDict[atom.Value] = atom.Value.atomType;
      }
    }
    foreach (var part in sim.solutionEditor.GetSolution().parts) {
      if (part.GetType().Id != ID) continue;
      inductionBowls.Add(part.InFrontBy(BOWL));
    }
  }
  internal static void Last(Sim sim, CycleEvent.CycleEventExecutionType executionType) {
    HashSet<HexIndex> changed = [];
    foreach (var mol in sim.molecules) {
      foreach (var atom in mol.GetAtoms()) {
        if (inductionDict.ContainsKey(atom.Value)) {
          if (inductionDict[atom.Value] != atom.Value.atomType) {
            changed.Add(atom.Key);
          }
        }
      }
    }
    changed.RemoveWhere(h => inductionBowls.Contains(h));
    foreach (var part in sim.solutionEditor.GetSolution().parts) {
      if (part.GetType().Id != ID) continue;
      if (changed.Contains(part.InFrontBy(HOOK))
      && sim.GetAtomReference(part, BOWL, false, out var saltRef)
      && saltRef.atomType == AtomTypes.salt) {
        sim.RecipeInputs[BOWL] = saltRef;
        sim.RecipeOutputs[BOWL] = AtomTypes.fire;
        TryTransmute(sim, part, BOWL);
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(BOWL)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        Resources.inductionSound.isPlaying = false;
        Resources.inductionSound.PlaySound(sim.solutionEditor.method_506());
      }
    }
  }
}