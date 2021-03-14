using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000007 RID: 7
    public class JobDriver_Cheer : JobDriver
    {
        // Token: 0x06000010 RID: 16 RVA: 0x0000231C File Offset: 0x0000051C
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x0000232F File Offset: 0x0000052F
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return new Toil
            {
                tickAction = delegate
                {
                    pawn.GainComfortFromCellIfPossible();
                    var flag = pawn.IsHashIntervalTick(100);
                    if (flag)
                    {
                        pawn.jobs.CheckForJobOverride();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
        }
    }
}