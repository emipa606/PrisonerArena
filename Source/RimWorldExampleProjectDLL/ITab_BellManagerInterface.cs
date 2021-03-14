using UnityEngine;
using Verse;

namespace ArenaBell
{
    // Token: 0x0200001A RID: 26
    internal class ITab_BellManagerInterface
    {
        // Token: 0x0400002E RID: 46
        private static readonly Listing_Standard listingStandard = new();

        // Token: 0x06000078 RID: 120 RVA: 0x00003A42 File Offset: 0x00001C42
        public static void DrawArenaTab(Rect rect, Building_Bell bell)
        {
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00004549 File Offset: 0x00002749
        private static void FillTabArea()
        {
            LabelWithTooltip("Brawl area", "Remov later");
        }

        // Token: 0x0600007A RID: 122 RVA: 0x0000455C File Offset: 0x0000275C
        private static void LabelWithTooltip(string label, string tooltip)
        {
            var rect = listingStandard.GetRect(Text.CalcHeight(label, listingStandard.ColumnWidth));
            Widgets.Label(rect, label);
            DoTooltip(rect, tooltip);
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00004598 File Offset: 0x00002798
        private static void DoTooltip(Rect rect, string tooltip)
        {
            var flag = !tooltip.NullOrEmpty();
            if (!flag)
            {
                return;
            }

            var flag2 = Mouse.IsOver(rect);
            if (flag2)
            {
                Widgets.DrawHighlight(rect);
            }

            TooltipHandler.TipRegion(rect, tooltip);
        }

        // Token: 0x0600007C RID: 124 RVA: 0x000045D4 File Offset: 0x000027D4
        private static string FighterLabel(Building_Bell bell, int index)
        {
            var flag = index == 0;
            if (flag)
            {
                var flag2 = bell.fighter1.p != null;
                if (flag2)
                {
                    if (bell.fighter1.p.AnimalOrWildMan())
                    {
                        return $"{bell.fighter1.p.Name.ToStringShort} ({bell.fighter1.p.def.race.AnyPawnKind.label})";
                    }

                    return bell.fighter1.p.Name.ToStringShort;
                }
            }

            var flag3 = index == 1;
            if (!flag3)
            {
                return "Select";
            }

            var flag4 = bell.fighter2.p != null;
            if (!flag4)
            {
                return "Select";
            }

            if (bell.fighter2.p.AnimalOrWildMan())
            {
                return $"{bell.fighter2.p.Name.ToStringShort} ({bell.fighter2.p.def.race.AnyPawnKind.label})";
            }

            return bell.fighter2.p.Name.ToStringShort;
        }
    }
}