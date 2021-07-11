using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x0200000E RID: 14
    public class JobGiver_Cheer : JobGiver_SpectateDutySpectateRect
    {
        // Token: 0x0600002D RID: 45 RVA: 0x00002B34 File Offset: 0x00000D34
        protected override Job TryGiveJob(Pawn pawn)
        {
            return new Job(JobDefOfArena.Cheer);
        }
    }
}