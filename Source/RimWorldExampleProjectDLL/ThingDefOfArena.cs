using RimWorld;
using Verse;

namespace ArenaBell;

[DefOf]
public static class ThingDefOfArena
{
    public static ThingDef Building_ArenaBell;

    static ThingDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}