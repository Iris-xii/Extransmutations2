using System.Collections.Generic;
using MonoMod;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public class patch_Translations { 
  [MonoModPublic]
  [MonoModIgnore]
  public static Dictionary<string, LocString> translationDict;
}