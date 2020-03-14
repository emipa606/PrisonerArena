using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArenaBell
{
	// Token: 0x02000002 RID: 2
	[StaticConstructorOnStartup]
	public class CompBell : ThingComp
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void decreaseRad()
		{
			this.radius = Mathf.Max(1.9f, this.radius - 1f);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000206F File Offset: 0x0000026F
		public void increaseRad()
		{
			this.radius = Mathf.Min(9.9f, this.radius + 1f);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000208E File Offset: 0x0000028E
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.radius, "radius", 9.9f, false);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020B0 File Offset: 0x000002B0
		public override void PostDrawExtraSelectionOverlays()
		{
			base.PostDrawExtraSelectionOverlays();
			Map currentMap = Find.CurrentMap;
			CellRect cellRect = CellRect.CenteredOn(this.parent.Position, 1).ExpandedBy(Mathf.RoundToInt(this.radius));
			CellRect cellRect2 = cellRect.ContractedBy(1);
			GenDraw.DrawFieldEdges(this.ValidCellsAround(this.parent.Position, currentMap, cellRect), Color.gray);
			GenDraw.DrawFieldEdges(this.ValidCellsAround(this.parent.Position, currentMap, cellRect2));
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002130 File Offset: 0x00000330
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo baseGizmo in base.CompGetGizmosExtra())
			{
				yield return baseGizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			yield return new Command_Action
			{
				action = new Action(this.increaseRad),
				defaultLabel = "Increase radius",
				defaultDesc = "Increase arena radius",
				hotKey = KeyBindingDefOf.Misc5,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/ExpandRadius", true)
			};
			yield return new Command_Action
			{
				action = new Action(this.decreaseRad),
				defaultLabel = "Decrease radius",
				defaultDesc = "Decrease arena radius",
				hotKey = KeyBindingDefOf.Misc5,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/ShrinkRadius", true)
			};
			yield break;
			yield break;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002140 File Offset: 0x00000340
		public IEnumerable<IntVec3> ValidCells
		{
			get
			{
				CellRect cellRect = CellRect.CenteredOn(this.parent.Position, 1).ExpandedBy(Mathf.RoundToInt(this.radius));
				return this.ValidCellsAround(this.parent.Position, this.parent.Map, cellRect.ContractedBy(1));
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000219C File Offset: 0x0000039C
		public List<IntVec3> ValidCellsAround(IntVec3 pos, Map map, CellRect rect)
		{
			CompBell.validCells.Clear();
			bool flag = !pos.InBounds(map);
			List<IntVec3> result;
			if (flag)
			{
				result = CompBell.validCells;
			}
			else
			{
				Region region = pos.GetRegion(map, RegionType.Set_Passable);
				bool flag2 = region == null;
				if (flag2)
				{
					result = CompBell.validCells;
				}
				else
				{
					RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate(Region r)
					{
						foreach (IntVec3 item in r.Cells)
						{
							bool flag3 = this.InDistOfRect(item, pos, rect);
							if (flag3)
							{
								CompBell.validCells.Add(item);
							}
						}
						return false;
					}, 13, RegionType.Set_Passable);
					result = CompBell.validCells;
				}
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000224C File Offset: 0x0000044C
		public bool InDistOfRect(IntVec3 pos, IntVec3 otherLoc, CellRect rect)
		{
			float num = (float)pos.x;
			float num2 = (float)pos.z;
			return num <= (float)rect.maxX && num >= (float)rect.minX && num2 <= (float)rect.maxZ && num2 >= (float)rect.minZ;
		}

		// Token: 0x04000001 RID: 1
		private static List<IntVec3> validCells = new List<IntVec3>();

		// Token: 0x04000002 RID: 2
		public float radius = 3f;
	}
}
