using System.Linq;
using UnityEngine;
using Verse;

namespace ArenaBell;

public class PlaceWorker_ArenaRadius : PlaceWorker
{
    public virtual void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
    {
        var cellRect = CellRect.CenteredOn(center, 1).ExpandedBy(Mathf.RoundToInt(3f));
        var cellRect2 = cellRect.ExpandedBy(1);
        GenDraw.DrawFieldEdges(cellRect.Cells.ToList(), Color.white);
        GenDraw.DrawFieldEdges(cellRect2.Cells.ToList(), Color.gray);
    }
}