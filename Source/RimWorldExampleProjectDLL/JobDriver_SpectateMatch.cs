using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobDriver_SpectateMatch : JobDriver
{
    private const TargetIndex MySpotOrChairInd = TargetIndex.A;

    private const TargetIndex WatchTargetInd = TargetIndex.B;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var spectator = pawn;
        var target = job.GetTarget(MySpotOrChairInd);
        var spectate = job;
        return spectator.Reserve(target, spectate, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var haveChair = job.GetTarget(MySpotOrChairInd).HasThing;
        if (haveChair)
        {
            this.EndOnDespawnedOrNull(MySpotOrChairInd);
        }

        yield return Toils_Goto.GotoCell(MySpotOrChairInd, PathEndMode.OnCell);
        yield return new Toil
        {
            tickAction = delegate
            {
                var thing = pawn.mindState?.duty?.focus.Thing;
                if (thing is Building_Bell { currentState: Building_Bell.State.fight })
                {
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None);
                }

                pawn.rotationTracker?.FaceCell(job.GetTarget(WatchTargetInd).Cell);
                pawn.GainComfortFromCellIfPossible();
                if (pawn.IsHashIntervalTick(100))
                {
                    pawn.jobs.CheckForJobOverride();
                }
            },
            defaultCompleteMode = ToilCompleteMode.Never,
            handlingFacing = true
        };
    }
}