using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobDriver_Cheer : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return new Toil
        {
            tickAction = delegate
            {
                pawn.GainComfortFromCellIfPossible();
                if (pawn.IsHashIntervalTick(100))
                {
                    pawn.jobs.CheckForJobOverride();
                }
            },
            defaultCompleteMode = ToilCompleteMode.Never
        };
    }
}