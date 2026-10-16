using System.Collections.Generic;
using Quintessential;

namespace Extransmutations2;

internal class Extransmutations2 : QuintessentialMod, IDataMod {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public static Extransmutations2 Instance { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

  public override string ModId => "extransmutations2";
  internal static string Section = "extransmutations-section";

  public override void Load() { }

  public override void LoadContent() {
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.LINE);
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.TRI);
    GlyphInversion.LoadContent();
  }
  public override void LoadCompatContent() { }
  public override void FinaliseContent() { }

  public override void PostLoad() { }
  public override void Unload() { }

  internal static void Log(string s) => Logger.Log($"[Extransmutations] {s}");
  internal static string IdStr(string key) => $"{Instance.ModId}:{key}";
  internal static string IdObj(string key) => new Identifier(Instance.ModId, key);
  internal static void AddAllRecipes<T>(Identifier partId, T recipes)
  where T : IEnumerable<(string, GlyphRecipe)> {
    foreach (var e in recipes) {
      QApi.AddRecipe(Instance, e.Item2, IdObj(e.Item1), partId);
    }
  }
  internal static void TryTransmute(Sim sim, Part part, HexIndex partPos) {
    var offsetFlash = new Vector2(-72, 0);
    if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference input
    && sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType output) {
      input.molecule.ReplaceAtom(output, input.pos);
      input.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor,
          TransmutationEffectRenderMode.AsEffect, input.atomType, Assets.textures.atoms.projection_effect, 30f);
    }
    else if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference onlyIn) {
      onlyIn.molecule.RemoveAtom(onlyIn.pos);
      sim.solutionEditor.glyphEffects.Add(
        new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
        HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(partPos)),
        Resources.destroyAnim, 30f, offsetFlash, 0f)
      );
    }
    else if (sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType onlyOut) {
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
