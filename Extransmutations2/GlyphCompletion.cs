using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static Extransmutations2;

internal static class GlyphCompletion {

  internal static HexIndex[] CARD_POS = [new(1, 0), new(0, 0), new(1, -1)];
  internal static HexIndex SALT_POS = new(0, 1);
  internal static Identifier ID = IdObj("cardinal-completion");

  internal static GlyphRecipe Recipe(AtomType[] allCardTypes) {
    return new(
    (sim, part) => {
      List<(HexIndex, AtomReference)> transmuteCandidates = [];
      AtomReference? saltAtomTransmute = null;
      bool hasSalt = false;
      HashSet<AtomType> seenCardinals = [];
      foreach (var cardinalPosition in CARD_POS) {
        if (sim.GetAtomReference(part, cardinalPosition, allowPartAttachedAtoms: false, out var c1)) {
          sim.RecipeInputs[cardinalPosition] = c1; // normal atom
          transmuteCandidates.Add((cardinalPosition, c1));
          seenCardinals.Add(c1.atomType);
        }
        else if (sim.GetAtomReference(part, cardinalPosition, allowPartAttachedAtoms: true, out var cwheel)) {
          seenCardinals.Add(cwheel.atomType); // wheel atom, counts but not transmuted
        }
      }
      if (sim.GetAtomReference(part, SALT_POS, allowPartAttachedAtoms: false, out var s1)
      && s1.atomType == AtomTypes.salt) {
        sim.RecipeInputs[SALT_POS] = s1; // normal salt 
        saltAtomTransmute = s1;
        hasSalt = true;
      }
      else if (sim.GetAtomReference(part, SALT_POS, allowPartAttachedAtoms: true, out var swheel)
      && swheel.atomType == AtomTypes.salt) {
        hasSalt = true; //wheel salt
      }
      seenCardinals.RemoveWhere(at => {
        return at is null || !allCardTypes.Contains(at);
      });
      AtomType? maybeMissing = allCardTypes
      .Where(card => !seenCardinals.Contains(card))
      .FirstOrDefault();
      if (seenCardinals.Count == 3 && hasSalt && maybeMissing is AtomType missing) {
        foreach (var tcard in transmuteCandidates) {
          sim.RecipeOutputs[tcard.Item1] = missing;
        }
        if (saltAtomTransmute is not null) {
          sim.RecipeOutputs[SALT_POS] = missing;
        }
        return true;
      }
      return false;
    }
  );
  }

  internal static void LoadContent() {
    PartType cardinalCompletion = new() {
      Id = ID,
      cost = 10, // Cost
      isFullHexCover = true, // Is a glyph (?)
      glowTexture = Resources.cardinalCompletionGlow, // Shadow/glow
      strokeTexture = Resources.cardinalCompletionStroke, // Stroke/outline
      baseTexture = Resources.completionPanel, // Panel icon
      hoverTexture = Resources.completionPanel, // Hovered panel icon
      glyphHexes = [
                new(0, 0),
                new(1, 0),
                new(0, 1),
                new(1, -1),
            ], // Spaces used 
      CustomPermissionCheck = perms => perms.Contains(ID)
    };
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    QApi.AddPartType(Instance, cardinalCompletion, ID.name,
      (part, pos, editor, renderer) => {
        Vector2 centre = new(/*82.5f*/41.25f, 119.5f);// new Vector2(42f, 49f) + new Vector2(0f,-1f);
        renderer.RenderBase(Resources.cardinalCompletionBase, new Vector2(-1, -1), centre, 0);
        renderer.RenderRotating(Resources.bowlTexture, new HexIndex(1, 0), Vector2.Zero);
        renderer.RenderRotating(Resources.bowlTexture, new HexIndex(0, 0), Vector2.Zero);
        renderer.RenderRotating(Resources.bowlTexture, new HexIndex(1, -1), Vector2.Zero);
        renderer.RenderRotating(Resources.bowlTexture, new HexIndex(0, 1), Vector2.Zero);

        renderer.RenderUpright(Resources.anyCardinal, new HexIndex(1, 0), Vector2.Zero);
        renderer.RenderUpright(Resources.anyCardinal, new HexIndex(0, 0), Vector2.Zero);
        renderer.RenderUpright(Resources.anyCardinal, new HexIndex(1, -1), Vector2.Zero);
        renderer.RenderUpright(Resources.saltBowlGlyphSpot, new HexIndex(0, 1), Vector2.Zero);
      });
    QApi.AddPartTypeToPanel(cardinalCompletion, false);

    QApi.BindGlyphCylce(cardinalCompletion, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, simState, recipe, isCycleStart) => {
      if (recipe.Predicate.InvokeAndClear(sim, part)) {
        foreach (var cardPos in CARD_POS) { TryTransmute(sim, part, cardPos); }
        TryTransmute(sim, part, SALT_POS);

        foreach (var cardPos in CARD_POS) {
          sim.solutionEditor.consumptionEffects.Add(
            new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
            HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(cardPos)),
            Resources.bowlGlow, 30f, Vector2.Zero, 0f)
          );
          sim.solutionEditor.consumptionEffects.Add(
            new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
            HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(cardPos)),
            Resources.anyGlowArray, 30f, Vector2.Zero, 0f)
          );
        }
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(SALT_POS)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(SALT_POS)),
          Resources.saltGlyphArray, 30f, Vector2.Zero, 0f)
        );

        Resources.cardinalCompletionSound.isPlaying = false;
        Resources.cardinalCompletionSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }));
    (string, GlyphRecipe)[] recipes = [
      ("cardinal-cycle",Recipe([AtomTypes.water,AtomTypes.fire,AtomTypes.earth,AtomTypes.air]))
    ];
    AddAllRecipes(ID, recipes);
  }
}