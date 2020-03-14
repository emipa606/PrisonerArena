using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace ArenaBell
{
	// Token: 0x02000019 RID: 25
	internal class ITab_BellManagerUtility
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00004124 File Offset: 0x00002324
		public static string FighterLabel(int index, Building_Bell bell)
		{
			bool flag = bell.fighter1.p == null;
			string result;
			if (flag)
			{
				result = "Select";
			}
			else
			{
				bool flag2 = index == 0;
				if (flag2)
				{
					result = bell.fighter1.p.Name.ToStringShort;
				}
				else
				{
					bool flag3 = index == 1;
					if (flag3)
					{
						result = bell.fighter2.p.Name.ToStringShort;
					}
					else
					{
						result = "error";
					}
				}
			}
			return result;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004198 File Offset: 0x00002398
		public static void OpenActor1SelectMenu(Building_Bell bell)
		{
			List<Pawn> actorList = new List<Pawn>();
			StringBuilder s = new StringBuilder();
			bool flag = bell.Map.mapPawns.PrisonersOfColonySpawned == null;
			if (flag)
			{
				Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
			}
			else
			{
				bool flag2 = bell.Map.mapPawns.PrisonersOfColonySpawnedCount <= 0;
				if (flag2)
				{
					Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
				}
				else
				{
					foreach (Pawn candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
					{
						actorList.Add(candidate);
					}
					bool flag3 = actorList != null;
					if (flag3)
					{
						bool flag4 = actorList.Count <= 0;
						if (flag4)
						{
							Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
							return;
						}
					}
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					foreach (Pawn actor in actorList)
					{
						Pawn localCol = actor;
						Action action = delegate()
						{
							bool flag5 = localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
							if (flag5)
							{
								bell.fighter1.p = localCol;
							}
							else
							{
								Messages.Message(localCol.Name.ToStringShort + " can't move and won't be a good fighter.", MessageTypeDefOf.RejectInput, true);
							}
						};
						list.Add(new FloatMenuOption(localCol.LabelShort, action, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000435C File Offset: 0x0000255C
		public static void OpenActor2SelectMenu(Building_Bell bell)
		{
			List<Pawn> actorList = new List<Pawn>();
			StringBuilder s = new StringBuilder();
			bool flag = bell.Map.mapPawns.PrisonersOfColonySpawned == null;
			if (flag)
			{
				Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
			}
			else
			{
				bool flag2 = bell.Map.mapPawns.PrisonersOfColonySpawnedCount <= 0;
				if (flag2)
				{
					Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
				}
				else
				{
					foreach (Pawn candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
					{
						actorList.Add(candidate);
					}
					bool flag3 = actorList != null;
					if (flag3)
					{
						bool flag4 = actorList.Count <= 0;
						if (flag4)
						{
							Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput, true);
							return;
						}
					}
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					foreach (Pawn actor in actorList)
					{
						Pawn localCol = actor;
						Action action = delegate()
						{
							bool flag5 = localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
							if (flag5)
							{
								bell.fighter2.p = localCol;
							}
							else
							{
								Messages.Message(localCol.Name.ToStringShort + " can't move and won't be a good fighter.", MessageTypeDefOf.RejectInput, true);
							}
						};
						list.Add(new FloatMenuOption(localCol.LabelShort, action, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
		}

		// Token: 0x0400002B RID: 43
		public static float ButtonSize = 40f;

		// Token: 0x0400002C RID: 44
		public static float SpacingOffset = 15f;

		// Token: 0x0400002D RID: 45
		public static float ColumnSize = 245f;
	}
}
