using System;
using System.Collections.Generic;
using System.IO;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal struct GlyphTex {
  public required Texture @base;
  public required Texture glow;
  public required Texture stroke;
  public required Texture tray;
}

internal static class Resources {
  internal static Texture TexOld(string s) =>
    AssetLoaderHelper.LoadTexture(s);
  internal static Texture Tex(string s) {
    var path = Path.Combine(["textures",Instance.ModId,s]);
    return AssetLoaderHelper.LoadTexture(path);
  }
  internal static GlyphTex Glyph(string glyphFolder) {
    var pathBase = Path.Combine(["textures",Instance.ModId,glyphFolder,"base"]);
    var pathGlow = Path.Combine(["textures",Instance.ModId,glyphFolder,"glow"]);
    var pathStroke = Path.Combine(["textures",Instance.ModId,glyphFolder,"stroke"]);
    var pathTray = Path.Combine(["textures",Instance.ModId,glyphFolder,"tray"]);
    return new() {
      @base = AssetLoaderHelper.LoadTexture(pathBase),
      glow = AssetLoaderHelper.LoadTexture(pathGlow),
      stroke = AssetLoaderHelper.LoadTexture(pathStroke),
      tray = AssetLoaderHelper.LoadTexture(pathTray),
    };
  }
  internal static Texture[] Anim(string folder, string file, int count, int pad = 4) {
    Texture[] anim = new Texture[count];
    for (int i = 0; i < count; i++) {
      anim[i] = TexOld(System.IO.Path.Combine(folder, file) + "_" + (i + 1).ToString().PadLeft(pad, '0'));
    }
    return anim;
  }

  public static Texture[] calcifyAnimation = Anim("textures/parts/calcification_glyph_flash.array", "calcify_glyph", 10);
  public static Texture bowlTexture = TexOld("textures/parts/calcinator_bowl");
  public static Texture bowlProjTexture = TexOld("textures/parts/projection_glyph/metal_bowl");
  public static Texture bondBowlTexture = TexOld("textures/parts/bonder_ring");
  public static Texture transmutationInversionSpotTexture = TexOld("textures/parts/cardinal_inversion_symbols");
  public static Texture transmutationCycleSpotTexture = TexOld("textures/parts/cardinal_cycle_symbols");
  public static Texture calcifySpotTexture = TexOld("textures/parts/calcinator_symbols");
  public static Texture saltBowlGlyphSpot = TexOld("textures/parts/salt_glyph_engraved");
  //public Texture cardinalCompletionGlyphAny = Tex("textures/parts/any_cardinal_completion");
  public static Texture diamondGlow = TexOld("textures/select/diamond_glow");
  public static Texture oneByThreeGlow = TexOld("textures/select/1by3_glow");
  public static Texture completionPanel = TexOld("textures/panel/completion_panel");
  public static Texture inversionPanel = TexOld("textures/panel/invert_panel");

  public static Texture cyclePanel = TexOld("textures/panel/cycle_panel");
  public static Texture cyclePanelV2 = TexOld("textures/panel/cycle_panel_v2");
  public static Texture cardinalCycleBase = TexOld("textures/parts/cardinal_revolution/base");
  public static Texture cardinalCycleGlow = TexOld("textures/parts/cardinal_revolution/glow");
  public static Texture cardinalCycleStroke = TexOld("textures/parts/cardinal_revolution/stroke");
  public static Texture cardinalCycleBaseV2 = TexOld("textures/parts/cardinal_revolution_v2/base");
  public static Texture cardinalCycleGlowV2 = TexOld("textures/parts/cardinal_revolution_v2/glow");
  public static Texture cardinalCycleStrokeV2 = TexOld("textures/parts/cardinal_revolution_v2/stroke");

  public static Texture cardinalInversionBase = TexOld("textures/parts/cardinal_inversion/base");
  public static Texture cardinalInversionGlow = TexOld("textures/parts/cardinal_inversion/glow");
  public static Texture cardinalInversionStroke = TexOld("textures/parts/cardinal_inversion/stroke");
  public static Texture cardinalCompletionBase = TexOld("textures/parts/completion_new_base");
  public static Texture cardinalCompletionGlow = TexOld("textures/parts/completion/glow");
  public static Texture cardinalCompletionStroke = TexOld("textures/parts/completion/stroke");
  public static Texture dejectionBase = TexOld("textures/parts/dejection/base");
  public static Texture dejectionGlow = TexOld("textures/parts/dejection/glow");
  public static Texture dejectionStroke = TexOld("textures/parts/dejection/stroke");
  public static Texture bowlHole = TexOld("textures/parts/extransmutations_hole");
  public static Texture anyCardinal = TexOld("textures/parts/any_cardinal");
  public static Texture backCardinal = TexOld("textures/parts/back_cardinal");
  public static Texture ichorIcon = TexOld("textures/parts/ichor_icon");
  public static Texture[] destroyAnim = Anim("textures/parts/disposal_flash.array", "disposal_flash", 30);
  public static Texture saltBowlGlyphSpotWhite = TexOld("textures/parts/salt_glyph_white");
  public static Texture[] dejectionIris = Anim("textures/parts/dejection_iris.array", "iris_full_cardinal_previous", 16);
  public static Texture aerationBase = TexOld("textures/parts/aeration/base");
  public static Texture aerationGlow = TexOld("textures/parts/aeration/glow");
  public static Texture aerationStroke = TexOld("textures/parts/aeration/stroke");
  public static Texture[] airIris = Anim("textures/parts/iris_full_air.array", "iris_full_air", 16);
  public static Texture aerationBaseDetail = TexOld("textures/parts/aeration/base_detail");
  public static Texture liquidationBase = TexOld("textures/parts/liquidation/base");
  public static Texture liquidationGlow = TexOld("textures/parts/liquidation/glow");
  public static Texture liquidationStroke = TexOld("textures/parts/liquidation/stroke");
  public static Texture quicksilverGlyphSpot = TexOld("textures/parts/quicksilver_glyph");
  public static Texture compactionBase = TexOld("textures/parts/compaction/base");
  public static Texture compactionGlow = TexOld("textures/parts/compaction/glow");
  public static Texture compactionStroke = TexOld("textures/parts/compaction/stroke");
  public static Texture earthGlyph = TexOld("textures/parts/earth_glyph");
  public static Texture inductionBase = TexOld("textures/parts/induction/base");
  public static Texture inductionGlow = TexOld("textures/parts/induction/glow");
  public static Texture inductionStroke = TexOld("textures/parts/induction/stroke");
  public static Texture markerLighting = TexOld("textures/parts/marker_lighting");
  public static Texture markerDetails = TexOld("textures/parts/marker_details");
  public static Texture fireGlyph = TexOld("textures/parts/fire_glyph");
  public static Texture dejectionPanel = TexOld("textures/panel/dejection_panel");
  public static Texture recombinationPanel = TexOld("textures/panel/recombination_panel");
  public static Texture aerationPanel = TexOld("textures/panel/aeration_panel");
  public static Texture liquidationPanel = TexOld("textures/panel/liquidation_panel");
  public static Texture compactionPanel = TexOld("textures/panel/compaction_panel");
  public static Texture inductionPanel = TexOld("textures/panel/induction_panel");
  public static Texture extraordinaryBase = TexOld("textures/parts/extraordinary/base");
  public static Texture extraordinaryGlow = TexOld("textures/parts/extraordinary/glow");
  public static Texture extraordinaryStroke = TexOld("textures/parts/extraordinary/stroke");
  public static Texture extraordinaryGlyph = TexOld("textures/parts/extraordinary_glyph");
  public static Texture waterGlyph = TexOld("textures/parts/water_glyph");
  public static Texture extraordinaryPanel = TexOld("textures/parts/extraordinary/panel");
  public static Texture restorationBase = TexOld("textures/parts/et_restoration/base");
  public static Texture restorationGlow = TexOld("textures/parts/et_restoration/glow");
  public static Texture restorationStroke = TexOld("textures/parts/et_restoration/stroke");
  public static Texture restorationPanel = TexOld("textures/parts/et_restoration/panel");

  public static Texture[] anyGlowArray = Anim("textures/parts/any_cardinal.array", "a", 9);
  public static Texture[] saltGlyphArray = Anim("textures/parts/salt_glyph.array", "a", 9);
  public static Texture[] inversionGlowArray = Anim("textures/parts/cardinal_inversion.array", "a", 9);
  public static Texture[] cycleGlowArray = Anim("textures/parts/cardinal_cycle.array", "a", 9);
  public static Texture[] ichorGlowArray = Anim("textures/parts/ichor_glow.array", "a", 9);
  public static Texture[] bowlGlow = Anim("textures/parts/bowl_glow.array", "a", 9);
  public static Texture[] aerationBreathe = Anim("textures/parts/aeration_breath.array", "breath", 16);
  public static Texture[] waterDownGlow = Anim("textures/parts/water_down.array", "a", 9);
  public static Texture[] waterUpGlow = Anim("textures/parts/water_up.array", "a", 9);
  public static Texture waterDown = TexOld("textures/parts/water_down");
  public static Texture waterUp = TexOld("textures/parts/water_up");

  public static GlyphTex quintConjunction = Glyph("quint-conjunction");

  public static Sound aerationSound = Sound("aeration_sound");
  public static Sound cardinalCompletionSound = Sound("cardinal_completion_sound");
  public static Sound cardinalInversionSound = Sound("cardinal_inversion_sound");
  public static Sound cardinalRotationSound = Sound("cardinal_rotation_sound");
  public static Sound compactionSound = Sound("compaction_sound", 0.3f);
  public static Sound dejectionSound = Sound("dejection_sound");
  public static Sound inductionSound = Sound("induction_sound");
  public static Sound recombinationSound = Sound("recombination_sound");
  public static Sound restorationSound = Sound("restoration_sound");
  public static Sound liquidationSound = Sound("liquidation_sound");
  public static Sound quintConjunctionSinkSound = Sound("sim_step");
  public static Sound quintConjunctionFinishSound = Sound("glyph_unification");
  


  internal static Sound Sound(string sRaw, float volume = 0.4f) { 
    string fullPath = Path.Combine(Instance.Meta.PathToDirectory,$"Content/sounds/{sRaw}.wav") ;
    AudioFile audio = AudioManager.layer_0_LoadAudioFile(fullPath);
    Sound sound = new() {
      field_4061 = audio,
      id = $"{Instance.ModId}:{sRaw}"
    };
    patch_Volume.volumeDict.Add(sound.id, volume);
    return sound;
  }

}