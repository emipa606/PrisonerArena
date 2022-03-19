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
        var target = job.GetTarget(TargetIndex.A);
        var spectate = job;
        return spectator.Reserve(target, spectate, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var haveChair = job.GetTarget(TargetIndex.A).HasThing;
        if (haveChair)
        {
            this.EndOnDespawnedOrNull(TargetIndex.A);
        }

        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
        yield return new Toil
        {
            tickAction = delegate
            {
                if (((Building_Bell)pawn.mindState.duty.focus.Thing).currentState ==
                    Building_Bell.State.fight)
                {
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None);
                }

                pawn.rotationTracker.FaceCell(job.GetTarget(TargetIndex.B).Cell);
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