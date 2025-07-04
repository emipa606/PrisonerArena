using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell;

public class WorkGiver_HandleFight : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest =>
        ThingRequest.ForDef(ThingDefOfArena.Building_ArenaBell);

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOfArena.Building_ArenaBell);
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building = t as Building_Bell;
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (building == null)
        {
            return false;
        }

        if (building.IsForbidden(pawn))
        {
            return false;
        }

        switch (building.currentState)
        {
            case Building_Bell.State.rest:
                return false;
            case Building_Bell.State.scheduled:
                return pawn.CanReserve(building.fighter1.p, 1, -1, null, forced) &&
                       pawn.CanReserve(building.fighter2.p, 1, -1, null, forced);
        }

        if (building.fighter1.isInFight)
        {
            return false;
        }

        return pawn.CanReserve(building.fighter1.p, 1, -1, null, forced) &&
               pawn.CanReserve(building.fighter2.p, 1, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is Building_Bell building && building.currentState != Building_Bell.State.scheduled)
        {
            if (building.fighter1.isInFight)
            {
                return null;
            }
        }

        if (t is Building_Bell bell)
        {
            return new Job(JobDefOfArena.HaulingPrisoner, bell.GetPrisonerForHaul(), bell,
                bell.GetFighterStandPoint())
            {
                count = 1
            };
        }

        return null;
    }
}