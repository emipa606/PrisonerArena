using RimWorld;
using Verse;

namespace ArenaBell;

[DefOf]
public static class MentalStateDefOfArena
{
    public static MentalStateDef Fighter;

    static MentalStateDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(MentalStateDefOf));
    }
}