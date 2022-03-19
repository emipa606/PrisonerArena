using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobGiver_WatchFight : JobGiver_SpectateDutySpectateRect
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        var duty = pawn.mindState.duty;
        Job result;
        if (duty == null)
        {
            result = null;
        }
        else
        {
            if (!SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out var cell,
                    duty.spectateRectAllowedSides))
            {
                result = null;
            }
            else
            {
                var centerCell = duty.spectateRect.CenterCell;
                var edifice = cell.GetEdifice(pawn.Map);
                if (edifice != null && edifice.def.category == ThingCategory.Building &&
                    edifice.def.building.isSittable && pawn.CanReserve(edifice))
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