using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobDriver_HaulPrisoner : JobDriver
{
    private const TargetIndex TakeeIndex = TargetIndex.A;

    private const TargetIndex BellIndex = TargetIndex.B;

    private const TargetIndex DropIndex = TargetIndex.C;

    private Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;

    private Building_Bell BellRef => (Building_Bell)job.GetTarget(TargetIndex.B).Thing;

    private IntVec3 DropPosition => (IntVec3)job.GetTarget(TargetIndex.C);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);
        this.FailOnDestroyedOrNull(TargetIndex.B);
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        this.FailOn(delegate
        {
            var unused = job.GetTarget(TargetIndex.A).Thing as IBillGiver;
            return BellRef.currentState == Building_Bell.State.rest;
        });
        AddFinishAction(delegate
        {
            if (Takee == BellRef.fighter1.p)
            {
                var isInFight = BellRef.fighter1.isInFight;
                if (!isInFight)
                {
                    BellRef.TryCancelBrawl();
                }
            }
            else
            {
                if (Takee != BellRef.fighter2.p)
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
                if (BellRef.currentState != Building_Bell.State.scheduled)
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
                if (BellRef.Destroyed)
                {
                    return;
                }

                HaulFinished();
                BellRef.PrisonerDelievered(Takee);
                if (BellRef.currentState == Building_Bell.State.fight)
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

    private void HaulFinished()
    {
        Takee.Position = DropPosition;
        Takee.Notify_Teleported(false);
        Takee.stances.CancelBusyStanceHard();
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var taker = pawn;
        LocalTargetInfo target = Takee;
        var takeJob = job;
        return taker.Reserve(target, takeJob, 1, -1, null, errorOnFailed);
    }
}