using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobGiver_Cheer : JobGiver_SpectateDutySpectateRect
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        return new Job(JobDefOfArena.Cheer);
    }
}