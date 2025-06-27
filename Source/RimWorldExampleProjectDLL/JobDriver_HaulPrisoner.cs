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

    private Pawn Takee => (Pawn)job.GetTarget(TakeeIndex).Thing;

    private Building_Bell BellRef => (Building_Bell)job.GetTarget(BellIndex).Thing;

    private IntVec3 DropPosition => (IntVec3)job.GetTarget(DropIndex);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TakeeIndex);
        this.FailOnDestroyedOrNull(BellIndex);
        yield return Toils_Reserve.Reserve(TakeeIndex);
        yield return Toils_Reserve.Reserve(BellIndex);
        this.FailOn(delegate
        {
            _ = job.GetTarget(TakeeIndex).Thing as IBillGiver;
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
                BellRef.StartTheShow();
            }
        };
        yield return Toils_Goto.GotoThing(TakeeIndex, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(TakeeIndex);
        yield return Toils_Goto.GotoCell(DropIndex, PathEndMode.Touch);
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

                haulFinished();
                BellRef.PrisonerDelivered(Takee);
                if (BellRef.currentState == Building_Bell.State.fight)
                {
                    return;
                }

                var carryPrisonerJob = new Job(JobDefOfArena.HaulingPrisoner, BellRef.GetPrisonerForHaul(), BellRef,
                    BellRef.GetFighterStandPoint())
                {
                    count = 1
                };
                pawn.jobs.TryTakeOrderedJob(carryPrisonerJob);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    private void haulFinished()
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