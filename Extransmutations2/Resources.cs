using System;
using System.Collections.Generic;
using System.IO;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class Resources {
  internal static Texture Tex(string s) =>
    AssetLoaderHelper.LoadTexture(s);
  internal static Texture[] Anim(string folder, string file, int count, int pad = 4) {
    Texture[] anim = new Texture[count];
    for (int i = 0; i < count; i++) {
      anim[i] = Tex(System.IO.Path.Combine(folder, file) + "_" + (i + 1).ToString().PadLeft(pad, '0'));
    }
    return anim;
  }

  public static Texture[] calcifyAnimation = Anim("textures/parts/calcification_glyph_flash.array", "calcify_glyph", 10);
  public static Texture bowlTexture = Tex("textures/parts/calcinator_bowl");
  public static Texture bowlProjTexture = Tex("textures/parts/projection_glyph/metal_bowl");
  public static Texture bondBowlTexture = Tex("textures/parts/bonder_ring");
  public static Texture transmutationInversionSpotTexture = Tex("textures/parts/cardinal_inversion_symbols");
  public static Texture transmutationCycleSpotTexture = Tex("textures/parts/cardinal_cycle_symbols");
  public static Texture calcifySpotTexture = Tex("textures/parts/calcinator_symbols");
  public static Texture saltBowlGlyphSpot = Tex("textures/parts/salt_glyph_engraved");
  //public Texture cardinalCompletionGlyphAny = Tex("textures/parts/any_cardinal_completion");
  public static Texture diamondGlow = Tex("textures/select/diamond_glow");
  public static Texture oneByThreeGlow = Tex("textures/select/1by3_glow");
  public static Texture completionPanel = Tex("textures/panel/completion_panel");
  public static Texture inversionPanel = Tex("textures/panel/invert_panel");

  public static Texture cyclePanel = Tex("textures/panel/cycle_panel");
  public static Texture cyclePanelV2 = Tex("textures/panel/cycle_panel_v2");
  public static Texture cardinalCycleBase = Tex("textures/parts/cardinal_revolution/base");
  public static Texture cardinalCycleGlow = Tex("textures/parts/cardinal_revolution/glow");
  public static Texture cardinalCycleStroke = Tex("textures/parts/cardinal_revolution/stroke");
  public static Texture cardinalCycleBaseV2 = Tex("textures/parts/cardinal_revolution_v2/base");
  public static Texture cardinalCycleGlowV2 = Tex("textures/parts/cardinal_revolution_v2/glow");
  public static Texture cardinalCycleStrokeV2 = Tex("textures/parts/cardinal_revolution_v2/stroke");


  public static Texture cardinalInversionBase = Tex("textures/parts/cardinal_inversion/base");
  public static Texture cardinalInversionGlow = Tex("textures/parts/cardinal_inversion/glow");
  public static Texture cardinalInversionStroke = Tex("textures/parts/cardinal_inversion/stroke");
  public static Texture cardinalCompletionBase = Tex("textures/parts/completion_new_base");
  public static Texture cardinalCompletionGlow = Tex("textures/parts/completion/glow");
  public static Texture cardinalCompletionStroke = Tex("textures/parts/completion/stroke");
  public static Texture dejectionBase = Tex("textures/parts/dejection/base");
  public static Texture dejectionGlow = Tex("textures/parts/dejection/glow");
  public static Texture dejectionStroke = Tex("textures/parts/dejection/stroke");
  public static Texture bowlHole = Tex("textures/parts/extransmutations_hole");
  public static Texture anyCardinal = Tex("textures/parts/any_cardinal");
  public static Texture backCardinal = Tex("textures/parts/back_cardinal");
  public static Texture ichorIcon = Tex("textures/parts/ichor_icon");
  public static Texture[] destroyAnim = Anim("textures/parts/disposal_flash.array", "disposal_flash", 30);
  public static Texture saltBowlGlyphSpotWhite = Tex("textures/parts/salt_glyph_white");
  public static Texture[] dejectionIris = Anim("textures/parts/dejection_iris.array", "iris_full_cardinal_previous", 16);
  public static Texture aerationBase = Tex("textures/parts/aeration/base");
  public static Texture aerationGlow = Tex("textures/parts/aeration/glow");
  public static Texture aerationStroke = Tex("textures/parts/aeration/stroke");
  public static Texture[] airIris = Anim("textures/parts/iris_full_air.array", "iris_full_air", 16);
  public static Texture aerationBaseDetail = Tex("textures/parts/aeration/base_detail");
  public static Texture liquidationBase = Tex("textures/parts/liquidation/base");
  public static Texture liquidationGlow = Tex("textures/parts/liquidation/glow");
  public static Texture liquidationStroke = Tex("textures/parts/liquidation/stroke");
  public static Texture quicksilverGlyphSpot = Tex("textures/parts/quicksilver_glyph");
  public static Texture compactionBase = Tex("textures/parts/compaction/base");
  public static Texture compactionGlow = Tex("textures/parts/compaction/glow");
  public static Texture compactionStroke = Tex("textures/parts/compaction/stroke");
  public static Texture earthGlyph = Tex("textures/parts/earth_glyph");
  public static Texture inductionBase = Tex("textures/parts/induction/base");
  public static Texture inductionGlow = Tex("textures/parts/induction/glow");
  public static Texture inductionStroke = Tex("textures/parts/induction/stroke");
  public static Texture markerLighting = Tex("textures/parts/marker_lighting");
  public static Texture markerDetails = Tex("textures/parts/marker_details");
  public static Texture fireGlyph = Tex("textures/parts/fire_glyph");
  public static Texture dejectionPanel = Tex("textures/panel/dejection_panel");
  public static Texture recombinationPanel = Tex("textures/panel/recombination_panel");
  public static Texture aerationPanel = Tex("textures/panel/aeration_panel");
  public static Texture liquidationPanel = Tex("textures/panel/liquidation_panel");
  public static Texture compactionPanel = Tex("textures/panel/compaction_panel");
  public static Texture inductionPanel = Tex("textures/panel/induction_panel");
  public static Texture extraordinaryBase = Tex("textures/parts/extraordinary/base");
  public static Texture extraordinaryGlow = Tex("textures/parts/extraordinary/glow");
  public static Texture extraordinaryStroke = Tex("textures/parts/extraordinary/stroke");
  public static Texture extraordinaryGlyph = Tex("textures/parts/extraordinary_glyph");
  public static Texture waterGlyph = Tex("textures/parts/water_glyph");
  public static Texture extraordinaryPanel = Tex("textures/parts/extraordinary/panel");
  public static Texture restorationBase = Tex("textures/parts/et_restoration/base");
  public static Texture restorationGlow = Tex("textures/parts/et_restoration/glow");
  public static Texture restorationStroke = Tex("textures/parts/et_restoration/stroke");
  public static Texture restorationPanel = Tex("textures/parts/et_restoration/panel");

  public static Texture[] anyGlowArray = Anim("textures/parts/any_cardinal.array", "a", 9);
  public static Texture[] saltGlyphArray = Anim("textures/parts/salt_glyph.array", "a", 9);
  public static Texture[] inversionGlowArray = Anim("textures/parts/cardinal_inversion.array", "a", 9);
  public static Texture[] cycleGlowArray = Anim("textures/parts/cardinal_cycle.array", "a", 9);
  public static Texture[] ichorGlowArray = Anim("textures/parts/ichor_glow.array", "a", 9);
  public static Texture[] bowlGlow = Anim("textures/parts/bowl_glow.array", "a", 9);
  public static Texture[] aerationBreathe = Anim("textures/parts/aeration_breath.array", "breath", 16);

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
  public static Texture[] waterDownGlow = Anim("textures/parts/water_down.array", "a", 9);
  public static Texture[] waterUpGlow = Anim("textures/parts/water_up.array", "a", 9);
  public static Texture waterDown = Tex("textures/parts/water_down");
  public static Texture waterUp = Tex("textures/parts/water_up");


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