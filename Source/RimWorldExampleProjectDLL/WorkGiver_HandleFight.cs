using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000018 RID: 24
    public class WorkGiver_HandleFight : WorkGiver_Scanner
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00003F18 File Offset: 0x00002118
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForDef(ThingDefOfArena.Building_ArenaBell);

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600006D RID: 109 RVA: 0x00003F34 File Offset: 0x00002134
        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        // Token: 0x0600006E RID: 110 RVA: 0x00003F48 File Offset: 0x00002148
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00003F5C File Offset: 0x0000215C
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOfArena.Building_ArenaBell);
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00003F88 File Offset: 0x00002188
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

            if (building.currentState == Building_Bell.State.rest)
            {
                return false;
            }

            if (building.currentState != Building_Bell.State.scheduled)
            {
                if (building.fighter1.isInFight && building.fighter1.isInFight)
                {
                    return false;
                }
            }

            if (!pawn.CanReserve(building.fighter1.p, 1, -1, null, forced))
            {
                return false;
            }

            return pawn.CanReserve(building.fighter2.p, 1, -1, null, forced);
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00004090 File Offset: 0x00002290
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is Building_Bell building && building.currentState != Building_Bell.State.scheduled)
            {
                if (building.fighter1.isInFight && building.fighter1.isInFight)
                {
                    return null;
                }
            }

            if (t is Building_Bell bell)
            {
                return new Job(JobDefOfArena.HaulingPrisoner, bell.getPrisonerForHaul(), bell,
                    bell.getFighterStandPoint())
                {
                    count = 1
                };
            }

            return null;
        }
    }
}