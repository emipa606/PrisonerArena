using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x0200000F RID: 15
    public class JobGiver_WatchFight : JobGiver_SpectateDutySpectateRect
    {
        // Token: 0x0600002F RID: 47 RVA: 0x00002B5C File Offset: 0x00000D5C
        protected override Job TryGiveJob(Pawn pawn)
        {
            var duty = pawn.mindState.duty;
            var flag = duty == null;
            Job result;
            if (flag)
            {
                result = null;
            }
            else
            {
                IntVec3 cell;
                var flag2 = !SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out cell,
                    duty.spectateRectAllowedSides);
                if (flag2)
                {
                    result = null;
                }
                else
                {
                    var centerCell = duty.spectateRect.CenterCell;
                    var edifice = cell.GetEdifice(pawn.Map);
                    var flag3 = edifice != null && edifice.def.category == ThingCategory.Building &&
                                edifice.def.building.isSittable && pawn.CanReserve(edifice);
                    if (flag3)
                    {
                        result = new Job(JobDefOfArena.SpectateFightingMatch, edifice, centerCell);
                    }
                    else
                    {
                        result = new Job(JobDefOfArena.SpectateFightingMatch, cell, centerCell);
                    }
                }
            }

            return result;
        }
    }
}