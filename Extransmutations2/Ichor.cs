using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

/// <summary> Ichor Atom stuff </summary>
public static class Ichor {
  /// <summary> AtomType of Ichor </summary>
  public static AtomType Type { get => field!; set => field = value; }
  internal static bool ShouldSuppress(Sim sim) {
    var ichorId = Type.Id;
    Solution sol = sim.solutionEditor.GetSolution();
    HashSet<HexIndex> ichorSafe = [];
    foreach(var part in sol.parts) {
      if(!part.GetType().IsInputOutput()) continue;
      Molecule? mol = part.GetMoleculeForInputOutput(sol);
      if(mol is null) continue;
      foreach(var atom in mol.GetAtoms()) {
        if(atom.Value.atomType.Id != ichorId) continue;
        ichorSafe.Add(part.InFrontBy(atom.Key));
      }
    }
    foreach(var molecule in sim.molecules) {
      foreach(var atom in molecule.GetAtoms()) {
        if(atom.Value.atomType.Id != ichorId) continue;
        if(ichorSafe.Contains(atom.Key)) continue;
        return true;
      }
    }
    return false;
  }
  internal static void LoadContent() {
    Type = new() {
      Id = IdObj("ichor"),
      defaultName = "ichor",
      matteTextures = new() {
        diffuse = Resources.Tex("textures/atoms/ichor_diffuse"),
        shade = Resources.Tex("textures/atoms/ichor_shade")
      },
      symbol = Resources.Tex("textures/atoms/ichor"),
      shadow = Assets.textures.atoms.shadow,
    };
    QApi.AddAtomType(Instance, Type, "ichor");
  }
  internal static void PostLoad() { 
  }
}