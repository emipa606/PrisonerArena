using System;
using System.Collections.Generic;
using System.Linq;
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
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDefOfArena.Building_ArenaBell);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003F34 File Offset: 0x00002134
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003F48 File Offset: 0x00002148
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003F5C File Offset: 0x0000215C
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOfArena.Building_ArenaBell).Cast<Thing>();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003F88 File Offset: 0x00002188
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_Bell building = t as Building_Bell;
			bool flag = t.Faction != pawn.Faction;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = building == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = building.IsForbidden(pawn);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = building.currentState == Building_Bell.State.rest;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = building.currentState != Building_Bell.State.scheduled;
							if (flag5)
							{
								bool flag6 = building.fighter1.isInFight && building.fighter1.isInFight;
								if (flag6)
								{
									return false;
								}
							}
							LocalTargetInfo target = building;
							bool flag7 = !pawn.CanReserve(building.fighter1.p, 1, -1, null, forced);
							if (flag7)
							{
								result = false;
							}
							else
							{
								bool flag8 = !pawn.CanReserve(building.fighter2.p, 1, -1, null, forced);
								result = !flag8;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004090 File Offset: 0x00002290
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_Bell building = t as Building_Bell;
			bool flag = building.currentState != Building_Bell.State.scheduled;
			if (flag)
			{
				bool flag2 = building.fighter1.isInFight && building.fighter1.isInFight;
				if (flag2)
				{
					return null;
				}
			}
			Building_Bell bell = t as Building_Bell;
			return new Job(JobDefOfArena.HaulingPrisoner, bell.getPrisonerForHaul(), bell, bell.getFighterStandPoint())
			{
				count = 1
			};
		}
	}
}
