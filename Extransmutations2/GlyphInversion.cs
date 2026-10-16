using System;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class GlyphInversion {

  internal static HexIndex CALC_SPOT = new(1, 0);
  internal static HexIndex TRAN_SPOT = new(0, 0);
  internal static Identifier ID = IdObj("cardinal-inversion");
  internal static void LoadContent() {
    PartType cardinalInversion = new() {
      Id = ID,
      cost = 10, // Cost
      isFullHexCover = true, // Is a glyph (?)
      glowTexture = Resources.cardinalInversionGlow, // Shadow/glow
      strokeTexture =  Resources.cardinalInversionStroke, // Stroke/outline
      baseTexture =  Resources.inversionPanel, // Panel icon
      hoverTexture =  Resources.inversionPanel, // Hovered panel icon
      glyphHexes = [
                new(0, 0),
                new(1, 0),
            ], // Spaces used 
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPartType(Instance,cardinalInversion,ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = new(123f, 47f);// new Vector2(42f, 49f) + new Vector2(0f,-1f);
      renderer.RenderBase(Resources.cardinalInversionBase, new Vector2(-1, -1), centre, 0);
      renderer.RenderRotating(Resources.bowlTexture, CALC_SPOT, Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, TRAN_SPOT, Vector2.Zero);
      renderer.RenderUpright(Resources.calcifySpotTexture, CALC_SPOT, Vector2.Zero);
      renderer.RenderUpright(Resources.transmutationInversionSpotTexture, TRAN_SPOT, Vector2.Zero);
    });
    (string,GlyphRecipe)[] recipes = [];
    AddAllRecipes(ID,recipes); 
    QApi.AddPartTypeToPanel(cardinalInversion,false); 
  }
}