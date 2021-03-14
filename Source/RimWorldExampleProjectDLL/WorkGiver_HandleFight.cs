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
            var flag = t.Faction != pawn.Faction;
            if (flag)
            {
                return false;
            }

            var flag2 = building == null;
            if (flag2)
            {
                return false;
            }

            var flag3 = building.IsForbidden(pawn);
            if (flag3)
            {
                return false;
            }

            var flag4 = building.currentState == Building_Bell.State.rest;
            if (flag4)
            {
                return false;
            }

            var flag5 = building.currentState != Building_Bell.State.scheduled;
            if (flag5)
            {
                var flag6 = building.fighter1.isInFight && building.fighter1.isInFight;
                if (flag6)
                {
                    return false;
                }
            }

            var flag7 = !pawn.CanReserve(building.fighter1.p, 1, -1, null, forced);
            if (flag7)
            {
                return false;
            }

            var flag8 = !pawn.CanReserve(building.fighter2.p, 1, -1, null, forced);
            return !flag8;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00004090 File Offset: 0x00002290
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var building = t as Building_Bell;
            var flag = building != null && building.currentState != Building_Bell.State.scheduled;
            if (flag)
            {
                var flag2 = building.fighter1.isInFight && building.fighter1.isInFight;
                if (flag2)
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