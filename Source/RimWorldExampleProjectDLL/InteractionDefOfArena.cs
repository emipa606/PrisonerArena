using RimWorld;

namespace ArenaBell;

[DefOf]
public static class InteractionDefOfArena
{
    public static InteractionDef Cheer;

    static InteractionDefOfArena()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(InteractionDefOf));
    }
}