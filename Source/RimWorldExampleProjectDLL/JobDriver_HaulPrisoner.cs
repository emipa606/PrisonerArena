using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x0200000A RID: 10
    public class JobDriver_HaulPrisoner : JobDriver
    {
        // Token: 0x04000008 RID: 8
        private const TargetIndex TakeeIndex = TargetIndex.A;

        // Token: 0x04000009 RID: 9
        private const TargetIndex BellIndex = TargetIndex.B;

        // Token: 0x0400000A RID: 10
        private const TargetIndex DropIndex = TargetIndex.C;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600001F RID: 31 RVA: 0x00002830 File Offset: 0x00000A30
        private Pawn Takee => (Pawn) job.GetTarget(TargetIndex.A).Thing;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000020 RID: 32 RVA: 0x00002858 File Offset: 0x00000A58
        private Building_Bell BellRef => (Building_Bell) job.GetTarget(TargetIndex.B).Thing;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000021 RID: 33 RVA: 0x0000287E File Offset: 0x00000A7E
        private IntVec3 DropPosition => (IntVec3) job.GetTarget(TargetIndex.C);

        // Token: 0x06000022 RID: 34 RVA: 0x00002891 File Offset: 0x00000A91
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            this.FailOn(delegate
            {
                var billGiver = job.GetTarget(TargetIndex.A).Thing as IBillGiver;
                return BellRef.currentState == Building_Bell.State.rest;
            });
            AddFinishAction(delegate
            {
                var flag = Takee == BellRef.fighter1.p;
                if (flag)
                {
                    var isInFight = BellRef.fighter1.isInFight;
                    if (!isInFight)
                    {
                        BellRef.TryCancelBrawl();
                    }
                }
                else
                {
                    var flag2 = Takee == BellRef.fighter2.p;
                    if (!flag2)
                    {
                        return;
                    }

                    var isInFight2 = BellRef.fighter2.isInFight;
                    if (!isInFight2)
                    {
                        BellRef.TryCancelBrawl();
                    }
                }
            });
            yield return new Toil
            {
                initAction = delegate
                {
                    var flag = BellRef.currentState == Building_Bell.State.scheduled;
                    if (!flag)
                    {
                        return;
                    }

                    BellRef.currentState = Building_Bell.State.preparation;
                    BellRef.startTheShow();
                }
            };
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.Touch);
            yield return new Toil
            {
                initAction = delegate
                {
                    var position = DropPosition;
                    pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out _);
                    var flag = !BellRef.Destroyed;
                    if (!flag)
                    {
                        return;
                    }

                    HaulFinished();
                    BellRef.PrisonerDelievered(Takee);
                    var flag2 = BellRef.currentState != Building_Bell.State.fight;
                    if (!flag2)
                    {
                        return;
                    }

                    var carryPrisonerJob = new Job(JobDefOfArena.HaulingPrisoner, BellRef.getPrisonerForHaul(), BellRef,
                        BellRef.getFighterStandPoint())
                    {
                        count = 1
                    };
                    pawn.jobs.TryTakeOrderedJob(carryPrisonerJob);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000028A1 File Offset: 0x00000AA1
        private void HaulFinished()
        {
            Takee.Position = DropPosition;
            Takee.Notify_Teleported(false);
            Takee.stances.CancelBusyStanceHard();
        }

        // Token: 0x06000024 RID: 36 RVA: 0x000028D8 File Offset: 0x00000AD8
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var taker = pawn;
            LocalTargetInfo target = Takee;
            var takeJob = job;
            return taker.Reserve(target, takeJob, 1, -1, null, errorOnFailed);
        }
    }
}