using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArenaBell;

[StaticConstructorOnStartup]
public class CompBell : ThingComp
{
    private static readonly List<IntVec3> validCells = new List<IntVec3>();

    public float radius = 3f;

    public IEnumerable<IntVec3> ValidCells
    {
        get
        {
            var cellRect = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius));
            return ValidCellsAround(parent.Position, parent.Map, cellRect.ContractedBy(1));
        }
    }

    private void decreaseRad()
    {
        radius = Mathf.Max(1.9f, radius - 1f);
    }

    private void increaseRad()
    {
        radius = Mathf.Min(25.9f, radius + 1f);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref radius, "radius", 9.9f);
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();
        var currentMap = Find.CurrentMap;
        var cellRect = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius));
        var cellRect2 = cellRect.ContractedBy(1);
        GenDraw.DrawFieldEdges(ValidCellsAround(parent.Position, currentMap, cellRect), Color.gray);
        GenDraw.DrawFieldEdges(ValidCellsAround(parent.Position, currentMap, cellRect2));
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var baseGizmo in base.CompGetGizmosExtra())
        {
            yield return baseGizmo;
        }

        yield return new Command_Action
        {
            action = increaseRad,
            defaultLabel = "Increase radius",
            defaultDesc = "Increase arena radius",
            hotKey = KeyBindingDefOf.Misc5,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ExpandRadius")
        };
        yield return new Command_Action
        {
            action = decreaseRad,
            defaultLabel = "Decrease radius",
            defaultDesc = "Decrease arena radius",
            hotKey = KeyBindingDefOf.Misc5,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ShrinkRadius")
        };
    }

    private List<IntVec3> ValidCellsAround(IntVec3 pos, Map map, CellRect rect)
    {
        validCells.Clear();
        List<IntVec3> result;
        if (!pos.InBounds(map))
        {
            result = validCells;
        }
        else
        {
            var region = pos.GetRegion(map);
            if (region == null)
            {
                result = validCells;
            }
            else
            {
                RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
                {
                    foreach (var item in r.Cells)
                    {
                        if (InDistOfRect(item, rect))
                        {
                            validCells.Add(item);
                        }
                    }

                    return false;
                }, 13);
                result = validCells;
            }
        }

        return result;
    }

    private bool InDistOfRect(IntVec3 pos, CellRect rect)
    {
        var num = (float)pos.x;
        var num2 = (float)pos.z;
        return num <= rect.maxX && num >= rect.minX && num2 <= rect.maxZ && num2 >= rect.minZ;
    }
}