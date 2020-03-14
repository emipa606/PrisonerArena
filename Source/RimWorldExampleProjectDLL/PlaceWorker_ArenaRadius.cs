using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace ArenaBell
{
	// Token: 0x02000017 RID: 23
	public class PlaceWorker_ArenaRadius : PlaceWorker
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00003EB0 File Offset: 0x000020B0
		public virtual void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
		{
			CellRect cellRect = CellRect.CenteredOn(center, 1).ExpandedBy(Mathf.RoundToInt(3f));
			CellRect cellRect2 = cellRect.ExpandedBy(1);
			GenDraw.DrawFieldEdges(cellRect.Cells.ToList<IntVec3>(), Color.white);
			GenDraw.DrawFieldEdges(cellRect2.Cells.ToList<IntVec3>(), Color.gray);
		}
	}
}
