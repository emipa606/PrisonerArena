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
        // Token: 0x04000001 RID: 1
        private static readonly List<IntVec3> validCells = new();

        // Token: 0x04000002 RID: 2
        public float radius = 3f;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000006 RID: 6 RVA: 0x00002140 File Offset: 0x00000340
        public IEnumerable<IntVec3> ValidCells
        {
            get
            {
                var cellRect = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius));
                return ValidCellsAround(parent.Position, parent.Map, cellRect.ContractedBy(1));
            }
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private void decreaseRad()
        {
            radius = Mathf.Max(1.9f, radius - 1f);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000206F File Offset: 0x0000026F
        private void increaseRad()
        {
            radius = Mathf.Min(25.9f, radius + 1f);
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000208E File Offset: 0x0000028E
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref radius, "radius", 9.9f);
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020B0 File Offset: 0x000002B0
        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            var currentMap = Find.CurrentMap;
            var cellRect = CellRect.CenteredOn(parent.Position, 1).ExpandedBy(Mathf.RoundToInt(radius));
            var cellRect2 = cellRect.ContractedBy(1);
            GenDraw.DrawFieldEdges(ValidCellsAround(parent.Position, currentMap, cellRect), Color.gray);
            GenDraw.DrawFieldEdges(ValidCellsAround(parent.Position, currentMap, cellRect2));
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002130 File Offset: 0x00000330
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

        // Token: 0x06000007 RID: 7 RVA: 0x0000219C File Offset: 0x0000039C
        private List<IntVec3> ValidCellsAround(IntVec3 pos, Map map, CellRect rect)
        {
            validCells.Clear();
            var flag = !pos.InBounds(map);
            List<IntVec3> result;
            if (flag)
            {
                result = validCells;
            }
            else
            {
                var region = pos.GetRegion(map);
                var flag2 = region == null;
                if (flag2)
                {
                    result = validCells;
                }
                else
                {
                    RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
                    {
                        foreach (var item in r.Cells)
                        {
                            var flag3 = InDistOfRect(item, rect);
                            if (flag3)
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

        // Token: 0x06000008 RID: 8 RVA: 0x0000224C File Offset: 0x0000044C
        private bool InDistOfRect(IntVec3 pos, CellRect rect)
        {
            var num = (float) pos.x;
            var num2 = (float) pos.z;
            return num <= rect.maxX && num >= rect.minX && num2 <= rect.maxZ && num2 >= rect.minZ;
        }
    }
}