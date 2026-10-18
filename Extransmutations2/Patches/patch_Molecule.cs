using System;
using System.Collections.Generic;
using System.Linq;
using Extransmutations2;
using Mono.Cecil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using MonoMod.Utils;

using static Extransmutations2.Extransmutations2;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class patch_Molecule : Molecule {
  [MonoModIgnore]
  public extern Maybe<LocString> orig_GetDisplayName();

  public new Maybe<LocString> GetDisplayName() {
    var orig = orig_GetDisplayName();
    if (orig.HasValue()) return orig;
    if (GetAtoms().Count == 2 && GetBonds().Count == 1
    && GetBonds()[0].type == BondTypeEnum.Standard
    && GetAtoms().Any(a => a.Value.atomType.Id == Ichor.Type.Id)) {
      LocString loc = new();
      foreach (var atom in GetAtoms()) {
        if (atom.Value.atomType.Id == Ichor.Type.Id) { continue; }
        else {
          loc = atom.Value.atomType.name;
          break;
        }
      }
      return LocString.Format(Instance.Translate("naming.tainted"), loc);
    }
    return MaybeHelper.empty;
  }
}