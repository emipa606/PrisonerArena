using RimWorld;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000004 RID: 4
    [DefOf]
    public static class DutyDefOfArena
    {
        // Token: 0x04000003 RID: 3
        public static DutyDef SpectateFight;

        // Token: 0x0600000D RID: 13 RVA: 0x000022E1 File Offset: 0x000004E1
        static DutyDefOfArena()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
        }
    }
}