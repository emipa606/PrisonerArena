using RimWorld;
using Verse;

namespace ArenaBell;

[DefOf]
public static class JobDefOfArena
{
    public static JobDef HaulingPrisoner;

    public static JobDef SpectateFightingMatch;

    public static JobDef Cheer;

    static JobDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
    }
}