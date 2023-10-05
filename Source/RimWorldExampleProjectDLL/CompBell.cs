using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArenaBell;

[StaticConstructorOnStartup]
public class CompBell : ThingComp
{
    private const float circleAddition = 0.3f;
    private static readonly List<IntVec3> validCells = new List<IntVec3>();

    public float radius = 9.9f;

    public bool useCircle;

    public IEnumerable<IntVec3> ValidCells
    {
        get
        {
            if (useCircle)
            {
                validCells.Clear();
                var region = parent.Position.GetRegion(parent.Map);
                if (region == null)
                {
                    return validCells;
                }

                RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
                {
                    foreach (var item in r.Cells)
                    {
                        if (item.InHorDistOf(parent.Position, radius + circleAddition))
                        {
                            validCells.Add(item);
                        }
                    }

                    return false;
                }, 13);

                return validCells;
            }

            var cellRect = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius));
            return ValidCellsAround(parent.Position, parent.Map, cellRect.ContractedBy(1));
        }
    }

    private void decreaseRad()
    {
        radius = Mathf.Max(1f, radius - 1f);
    }

    private void increaseRad()
    {
        radius = Mathf.Min(25f, radius + 1f);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref radius, "radius", 9.9f);
        Scribe_Values.Look(ref useCircle, "useCircle");
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();

        var region = parent.Position.GetRegion(parent.Map);
        if (region == null)
        {
            return;
        }

        var outerSquareCells = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius)).Cells;
        var innerSquareCells = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius))
            .ContractedBy(1).Cells;
        var validOuterCells = new List<IntVec3>();
        var validInnerCells = new List<IntVec3>();

        RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
        {
            foreach (var item in r.Cells)
            {
                if (useCircle)
                {
                    if (item.InHorDistOf(parent.Position, radius + circleAddition + 1))
                    {
                        validOuterCells.Add(item);
                    }

                    if (item.InHorDistOf(parent.Position, radius + circleAddition))
                    {
                        validInnerCells.Add(item);
                    }

                    continue;
                }

                if (outerSquareCells.Contains(item))
                {
                    validOuterCells.Add(item);
                }

                if (innerSquareCells.Contains(item))
                {
                    validInnerCells.Add(item);
                }
            }

            return false;
        }, 13);

        GenDraw.DrawFieldEdges(validOuterCells, Color.gray);
        GenDraw.DrawFieldEdges(validInnerCells);
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
            defaultLabel = "PA.IncreaseRadius".Translate(),
            defaultDesc = "PA.IncreaseRadiusTT".Translate(),
            hotKey = KeyBindingDefOf.Misc5,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ExpandRadius")
        };
        yield return new Command_Action
        {
            action = decreaseRad,
            defaultLabel = "PA.DecreaseRadius".Translate(),
            defaultDesc = "PA.DecreaseRadiusTT".Translate(),
            hotKey = KeyBindingDefOf.Misc6,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ShrinkRadius")
        };
        if (!useCircle)
        {
            yield return new Command_Action
            {
                action = () => useCircle = !useCircle,
                defaultLabel = "PA.SwitchToCircle".Translate(),
                defaultDesc = "PA.SwitchToCircleTT".Translate(),
                hotKey = KeyBindingDefOf.Misc7,
                icon = ContentFinder<Texture2D>.Get("UI/Commands/UseCircle")
            };
        }
        else
        {
            yield return new Command_Action
            {
                action = () => useCircle = !useCircle,
                defaultLabel = "PA.SwitchToSquare".Translate(),
                defaultDesc = "PA.SwitchToSquareTT".Translate(),
                hotKey = KeyBindingDefOf.Misc7,
                icon = ContentFinder<Texture2D>.Get("UI/Commands/UseSquare")
            };
        }
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