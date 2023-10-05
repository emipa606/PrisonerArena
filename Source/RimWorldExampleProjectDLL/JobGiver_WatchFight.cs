using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class JobGiver_WatchFight : JobGiver_SpectateDutySpectateRect
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        var duty = pawn.mindState.duty;
        if (duty == null)
        {
            return null;
        }

        var bellcomp = ((Building_Bell)duty.focus)?.TryGetComp<CompBell>();
        if (bellcomp == null)
        {
            return null;
        }

        bool foundCell;
        IntVec3 cellFound;
        if (bellcomp.useCircle)
        {
            foundCell = SpectatorCellFinder.TryFindCircleSpectatorCellFor(pawn,
                CellRect.CenteredOn(((Building_Bell)duty.focus).Position, 0),
                duty.spectateDistance.min,
                duty.spectateDistance.max, pawn.Map, out cellFound);
        }
        else
        {
            foundCell = SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map,
                out cellFound,
                duty.spectateRectAllowedSides);
        }

        if (!foundCell)
        {
            return null;
        }

        var centerCell = duty.spectateRect.CenterCell;
        var edifice = cellFound.GetEdifice(pawn.Map);
        if (edifice != null && edifice.def.category == ThingCategory.Building &&
            edifice.def.building.isSittable && pawn.CanReserve(edifice))
        {
            return new Job(JobDefOfArena.SpectateFightingMatch, edifice, centerCell);
        }

        return new Job(JobDefOfArena.SpectateFightingMatch, cellFound, centerCell);
    }
}