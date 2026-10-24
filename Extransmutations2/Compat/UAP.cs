using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quintessential;
namespace Extransmutations2.Compat;

using static ETUtils;
using static Extransmutations2;

using Recipes = (string, GlyphRecipe)[];
internal class UAP {
  const string UAPId = "uncommonprimes";
  static internal UAP? inst = null;
  required internal AtomType bellum;
  required internal AtomType obscurum;
  required internal AtomType lux;
  required internal AtomType pax;

  private UAP() { }

  internal static void LoadCompatContent() {
    if (!QuintessentialLoader.IsModPresent(UAPId)) return;
    UAP.inst = new() {
      bellum = AtomTypes.GetByID(new(UAPId, "bellum")),
      obscurum = AtomTypes.GetByID(new(UAPId, "obscurum")),
      lux = AtomTypes.GetByID(new(UAPId, "lux")),
      pax = AtomTypes.GetByID(new(UAPId, "pax")),
    };
    QApi.AddPermissionIfNotPresent(Instance, GlyphExtraordinary.ID.name, GlyphExtraordinary.ID.name, Section);

    static bool ExtraordinaryCheck(Sim sim, Part part) =>
      sim.solutionEditor.GetSolution().parts.Any(p => p.GetType() == GlyphExtraordinary.TYPE);

    AddAllRecipes<Recipes>(GlyphCompletion.ID, [
      ("ordinal-cycle",GlyphCompletion.Recipe((inst.bellum,inst.obscurum,inst.lux,inst.pax), ExtraordinaryCheck)),
    ]);
    AddAllRecipes<Recipes>(GlyphInversion.ID, [
      ("lux-obscurum",GlyphInversion.Recipe(inst.lux,inst.obscurum, ExtraordinaryCheck)),
      ("obscurum-lux",GlyphInversion.Recipe(inst.obscurum,inst.lux, ExtraordinaryCheck)),
      ("pax-bellum",GlyphInversion.Recipe(inst.pax,inst.bellum, ExtraordinaryCheck)),
      ("bellum-pax",GlyphInversion.Recipe(inst.bellum,inst.pax, ExtraordinaryCheck)),
    ]);
    var revRecipes = Recipes (GlyphRevolution.Variant v) => [
      ("bellum-obscurum",GlyphRevolution.Recipe(v,inst.bellum,inst.obscurum, ExtraordinaryCheck)),
      ("obscurum-pax",GlyphRevolution.Recipe(v,inst.obscurum,inst.pax, ExtraordinaryCheck)),
      ("pax-lux",GlyphRevolution.Recipe(v,inst.pax,inst.lux, ExtraordinaryCheck)),
      ("lux-bellum",GlyphRevolution.Recipe(v,inst.lux,inst.bellum, ExtraordinaryCheck))
    ];
    AddAllRecipes(GlyphRevolution.NamespacedID(GlyphRevolution.Variant.LINE),revRecipes(GlyphRevolution.Variant.LINE));
    AddAllRecipes(GlyphRevolution.NamespacedID(GlyphRevolution.Variant.TRI),revRecipes(GlyphRevolution.Variant.TRI));

    AddAllRecipes<Recipes>(GlyphRestoration.ID, [
      ("restore-bellum",GlyphRestoration.Recipe(inst.bellum, ExtraordinaryCheck)),
      ("restore-lux",GlyphRestoration.Recipe(inst.lux, ExtraordinaryCheck)),
      ("restore-obscurum",GlyphRestoration.Recipe(inst.obscurum, ExtraordinaryCheck)),
      ("restore-pax",GlyphRestoration.Recipe(inst.pax, ExtraordinaryCheck)),
    ]);
    AddAllRecipes<Recipes>(GlyphDejection.ID, [
      ("lux-pax",GlyphDejection.Recipe(inst.lux,inst.pax,ExtraordinaryCheck)),
      ("pax-obscurum",GlyphDejection.Recipe(inst.pax,inst.obscurum,ExtraordinaryCheck)),
      ("obscurum-bellum",GlyphDejection.Recipe(inst.obscurum,inst.bellum,ExtraordinaryCheck)),
      ("bellum-lux",GlyphDejection.Recipe(inst.bellum,inst.lux,ExtraordinaryCheck))
    ]);
  }
}