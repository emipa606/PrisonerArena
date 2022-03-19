using RimWorld;
using Verse.AI;

namespace ArenaBell;

[DefOf]
public static class DutyDefOfArena
{
    public static DutyDef SpectateFight;

    static DutyDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
    }
}