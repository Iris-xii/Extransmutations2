using System;
using System.Collections.Generic;
using System.Linq;
using Quintessential;

namespace Extransmutations2;

using static ETUtils;
using static Extransmutations2;

internal static class GlyphLiquidation {


  internal static HexIndex[] LIQ_UP = [new(0, 0), new(-1, 2)];
  internal static HexIndex LIQ_DOWN = new(0, 1);
  internal static Identifier ID = IdObj("liquidation");
  internal static void LoadContent() {
    PartType glyphLiquidation = new() {
      Id = ID,
      cost = 10,
      isFullHexCover = true,
      glowTexture = Resources.liquidationGlow,
      strokeTexture = Resources.liquidationStroke,
      baseTexture = Resources.liquidationPanel,
      hoverTexture = Resources.liquidationPanel,
      glyphHexes = [new(0, 0), new(0, 1), new(-1, 2)],
      CustomPermissionCheck = perms => perms.Contains(ID),
    };
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    QApi.AddPartType(Instance, glyphLiquidation, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 centre = Resources.liquidationBase.GetHalfSize();
      renderer.RenderBase(Resources.liquidationBase, new Vector2(-1, -1), centre, 0f);
      renderer.RenderRotating(Resources.bowlProjTexture, new(0, 1), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, new(0, 0), Vector2.Zero);
      renderer.RenderRotating(Resources.bowlTexture, new(-1, 2), Vector2.Zero);
      renderer.RenderUpright(Resources.waterUp, new(0, 0), Vector2.Zero);
      renderer.RenderUpright(Resources.waterDown, new(0, 1), Vector2.Zero);
      renderer.RenderUpright(Resources.waterUp, new(-1, 2), Vector2.Zero);
    });
    QApi.AddPartTypeToPanel(glyphLiquidation, false);
    QApi.AddRecipe(Instance, new((sim, part) => { return true; }), IdObj("liquidation-always"), ID); // liquidation does not use recipes
    QApi.BindGlyphCycle(glyphLiquidation, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      AtomReference[] liqUpRef = [.. LIQ_UP
      .Select(h => {sim.GetAtomReference(part,h,false,out var a); return a;})
      .Where(e => e is not null)];
      if (!sim.GetAtomReference(part, LIQ_DOWN, false, out AtomReference liqDownRef)) return false;

      int donorValue = 0;
      if (HasTag(liqDownRef, LiquidationDonor1)) { donorValue += 1; }
      if (HasTag(liqDownRef, LiquidationDonor2)) { donorValue += 2; }

      int requiredUpValue = 0;
      bool atLeastOneInvalidTargetPresent = false; // makes it so that if it canTransmute, 
                                                   // we know for sure all atoms involved are
                                                   // transmutable.
      foreach (var atomRef in liqUpRef) {
        bool isValidTarget = false;
        if (HasTag(atomRef, LiquidationSink1)) { requiredUpValue += 1; isValidTarget = true; }
        if (HasTag(atomRef, LiquidationSink2)) { requiredUpValue += 2; isValidTarget = true; }
        if (!isValidTarget) { atLeastOneInvalidTargetPresent = true; }
      }
      bool canTransmute = (donorValue > 0) && (requiredUpValue <= donorValue) 
        && !atLeastOneInvalidTargetPresent
        && liqUpRef.Length > 0;
      if (canTransmute) {
        foreach (var r in liqUpRef) {
          HasTag(r, LiquidationSink1, out Identifier? targetId);
          if (targetId is null) { HasTag(r, LiquidationSink2, out targetId); }
          AtomType target = AtomTypes.GetByID(targetId!.Value);

          r.molecule.ReplaceAtom(target, r.pos);
          r.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor,
            TransmutationEffectRenderMode.AsEffect, r.atomType, Assets.textures.atoms.projection_effect, 30f);
          sim.solutionEditor.consumptionEffects.Add(
            new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
            HexGrid.standardGrid.ToPixelCoords(r.pos),
            Resources.bowlGlow, 30f, Vector2.Zero, 0f)
          ); 
          sim.solutionEditor.consumptionEffects.Add(
            new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
            HexGrid.standardGrid.ToPixelCoords(r.pos),
            Resources.waterUpGlow, 30f, Vector2.Zero, 0f)
          );
        } 
        AtomType targetDown;
        {
          HasTag(liqDownRef, LiquidationDonor2, out Identifier? targetId);
          if (targetId is null) { HasTag(liqDownRef, LiquidationDonor1, out targetId); }
          targetDown = AtomTypes.GetByID(targetId!.Value);
        } 
        liqDownRef.molecule.ReplaceAtom(targetDown, liqDownRef.pos);
        liqDownRef.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor,
          TransmutationEffectRenderMode.AsEffect, liqDownRef.atomType, Assets.textures.atoms.projection_effect, 30f);
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(liqDownRef.pos),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(liqDownRef.pos),
          Resources.waterDownGlow, 30f, Vector2.Zero, 0f)
        );
        Resources.liquidationSound.isPlaying = false;
        Resources.liquidationSound.PlaySound(sim.solutionEditor.method_506());
        return true;
      }
      return false;
    }));
  }
}