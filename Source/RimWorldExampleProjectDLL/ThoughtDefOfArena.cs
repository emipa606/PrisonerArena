using RimWorld;

namespace ArenaBell;

[DefOf]
public static class ThoughtDefOfArena
{
    public static ThoughtDef ArenaWinner;

    public static ThoughtDef ArenaLoser;

    static ThoughtDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
    }
}