using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using MonoMod.Utils;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
 
public class patch_Sim {
  public static bool ExtransmutationsIchorSuppression(bool suppress, Sim s) => 
    suppress || Extransmutations2.Ichor.ShouldSuppress(s);
    
  [MonoModILInject("RunCycleGlyphs")]
  public static void Patch_RunCycleGlyphs(MethodDefinition method, CustomAttribute attrib) { 
    TypeDefinition def = method.DeclaringType.Resolve();
    var isup = def.Methods.First(f => f.Name.Equals("ExtransmutationsIchorSuppression")).Resolve();
    ILCursor cursor = new(new ILContext(method));
    cursor.TryGotoNext(MoveType.After,
      instr => instr.MatchCallOrCallvirt("Sim", "AnyPartOverlapsMolecule"));
    cursor.TryGotoNext(MoveType.After,
      instr => instr.MatchCallOrCallvirt("Sim", "AnyPartOverlapsMolecule"));
    cursor.EmitLdarg(0);
    cursor.EmitCallvirt(isup);
  }
}