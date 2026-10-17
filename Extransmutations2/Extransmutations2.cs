using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

internal class Extransmutations2 : QuintessentialMod, IDataMod {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public static Extransmutations2 Instance { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

  public override string ModId => "extransmutations2";

  public static Identifier LiquidationDonor2 { get => IdObj("$liquidation-donor-2"); }
  public static Identifier LiquidationDonor1 { get => IdObj("$liquidation-donor-1"); }
  public static Identifier LiquidationSink2 { get => IdObj("$liquidation-sink-2"); }
  public static Identifier LiquidationSink1 { get => IdObj("$liquidation-sink-1"); }

  internal static string Section = "extransmutations-section";

  public override void Load() { }

  public override void LoadContent() {
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.LINE);
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.TRI);
    GlyphInversion.LoadContent();
    GlyphCompletion.LoadContent();
    GlyphAeration.LoadContent();
    GlyphLiquidation.LoadContent();
    GlyphCompaction.LoadContent();
    GlyphInduction.LoadContent();
  }
  public override void LoadCompatContent() { }
  public override void FinaliseContent() { }

  public override void PostLoad() { }
  public override void Unload() { }

  internal static void Log(string s) => Logger.Log($"[Extransmutations] {s}");
  internal static string IdStr(string key) => $"{Instance.ModId}:{key}";
  internal static string IdObj(string key) => new Identifier(Instance.ModId, key);
  internal static bool HasTag(AtomReference ar, Identifier tag) {
    if (!AtomTag.AtomTags.TryGetValue(tag, out var atomTag)) return false;
    return atomTag.HasAtom(ar);
  }
  internal static bool HasTag(AtomReference ar, Identifier tag, out Identifier? target) {
    target = null;
    if (!AtomTag.AtomTags.TryGetValue(tag, out var atomTag)) return false;
    return atomTag.HasAtom(ar, out target);
  }
  internal static void AddAllRecipes<T>(Identifier partId, T recipes)
  where T : IEnumerable<(string, GlyphRecipe)> {
    foreach (var e in recipes) {
      QApi.AddRecipe(Instance, e.Item2, IdObj($"{partId.name}-{e.Item1}"), partId);
    }
  }
  internal enum DelAnim { DISPOSAL, NONE, CONSUME }
  internal static void TryTransmute(Sim sim, Part part, HexIndex partPos,
  DelAnim delAnim = DelAnim.DISPOSAL) {
    var offsetFlash = new Vector2(-72, 0);
    if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference input
    && sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType output) {
      input.molecule.ReplaceAtom(output, input.pos);
      input.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor,
          TransmutationEffectRenderMode.AsEffect, input.atomType, Assets.textures.atoms.projection_effect, 30f);
    }
    else if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference onlyIn) {
      onlyIn.molecule.RemoveAtom(onlyIn.pos);
      if (delAnim == DelAnim.DISPOSAL) {
        sim.solutionEditor.glyphEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(partPos)),
          Resources.destroyAnim, 30f, offsetFlash, 0f)
        );
      }
      else if (delAnim == DelAnim.CONSUME) {
        sim.solutionEditor.consumptionEffects.Add(
          new ConsumptionEffect(sim.solutionEditor,
          onlyIn));
      }
    }
    else if (sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType onlyOut
    && !sim.HasAtomAt(part, partPos, true)) {
      Molecule newMol = new();
      newMol.AddAtom(new(onlyOut), part.InFrontBy(partPos));
      sim.molecules.Add(newMol);
      sim.solutionEditor.glyphEffects.Add(
        new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
        HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(partPos)),
        Resources.destroyAnim, 30f, offsetFlash, 0f)
      );
    }
  }
}
