using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphExtraordinary {
  internal static Identifier ID = IdObj("extraordinary");
  internal static PartType TYPE = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.extraordinaryGlow,
      strokeTexture = Resources.extraordinaryStroke,
      baseTexture = Resources.extraordinaryPanel,
      hoverTexture = Resources.extraordinaryPanel,
      glyphHexes = [new(0,0)],
      CustomPermissionCheck = perms => perms.Contains(ID),
      isUnique = true // no point in duplicates
    };
  internal static void LoadContent() {
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.extraordinaryBase.GetHalfSize();
      renderer.RenderBase(Resources.extraordinaryBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.markerLighting, new(0, 0), Vector2.Zero); 
      renderer.RenderUpright(Resources.extraordinaryGlyph, new(0, 0), Vector2.Zero); 
    });
    QApi.AddPartTypeToPanel(TYPE, false);
  }
}