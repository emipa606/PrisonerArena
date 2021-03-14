using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000009 RID: 9
    public class JobDriver_SpectateMatch : JobDriver
    {
        // Token: 0x04000006 RID: 6
        private const TargetIndex MySpotOrChairInd = TargetIndex.A;

        // Token: 0x04000007 RID: 7
        private const TargetIndex WatchTargetInd = TargetIndex.B;

        // Token: 0x0600001B RID: 27 RVA: 0x00002740 File Offset: 0x00000940
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var spectator = pawn;
            var target = job.GetTarget(TargetIndex.A);
            var spectate = job;
            return spectator.Reserve(target, spectate, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002779 File Offset: 0x00000979
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var haveChair = job.GetTarget(TargetIndex.A).HasThing;
            var flag = haveChair;
            if (flag)
            {
                this.EndOnDespawnedOrNull(TargetIndex.A);
            }

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil
            {
                tickAction = delegate
                {
                    var flag2 = ((Building_Bell) pawn.mindState.duty.focus.Thing).currentState ==
                                Building_Bell.State.fight;
                    if (flag2)
                    {
                        JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None);
                    }

                    pawn.rotationTracker.FaceCell(job.GetTarget(TargetIndex.B).Cell);
                    pawn.GainComfortFromCellIfPossible();
                    var flag3 = pawn.IsHashIntervalTick(100);
                    if (flag3)
                    {
                        pawn.jobs.CheckForJobOverride();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never,
                handlingFacing = true
            };
        }
    }
}