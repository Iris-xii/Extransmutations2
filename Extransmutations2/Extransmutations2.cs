using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quintessential;

namespace Extransmutations2;
using static ETUtils;

internal class Extransmutations2 : QuintessentialMod, IDataMod {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public static Extransmutations2 Instance { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

  public override string ModId => "extransmutations2";

  public static Identifier LiquidationDonor2 { get => IdObj("$liquidation-donor-2"); }
  public static Identifier LiquidationDonor1 { get => IdObj("$liquidation-donor-1"); }
  public static Identifier LiquidationSink2 { get => IdObj("$liquidation-sink-2"); }
  public static Identifier LiquidationSink1 { get => IdObj("$liquidation-sink-1"); }

  internal static string Section { get => "extransmutations-section"; }
  internal static string IchorSection { get => "extransmutations-ichor-section"; }
  
  public override void Load() { }
  public override void LoadContent() {
    Ichor.LoadContent();
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.LINE);
    GlyphRevolution.LoadContent(GlyphRevolution.Variant.TRI);
    GlyphInversion.LoadContent();
    GlyphCompletion.LoadContent();
    GlyphAeration.LoadContent();
    GlyphLiquidation.LoadContent();
    GlyphCompaction.LoadContent();
    GlyphInduction.LoadContent();
    // -- ICHOR GLYPHS --
    GlyphDejection.LoadContent();
    GlyphRecombination.LoadContent();
    GlyphRestoration.LoadContent();
  }
  public override void LoadCompatContent() { }
  public override void FinaliseContent() { }

  public override void PostLoad() { }
  public override void Unload() { }


}
