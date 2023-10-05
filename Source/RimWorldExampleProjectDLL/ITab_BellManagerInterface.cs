using UnityEngine;
using Verse;

namespace ArenaBell;

internal class ITab_BellManagerInterface
{
    private static readonly Listing_Standard listingStandard = new Listing_Standard();

    public static void DrawArenaTab(Rect rect, Building_Bell bell)
    {
    }

    private static void FillTabArea()
    {
        LabelWithTooltip("Brawl area", "Remov later");
    }

    private static void LabelWithTooltip(string label, string tooltip)
    {
        var rect = listingStandard.GetRect(Text.CalcHeight(label, listingStandard.ColumnWidth));
        Widgets.Label(rect, label);
        DoTooltip(rect, tooltip);
    }

    private static void DoTooltip(Rect rect, string tooltip)
    {
        if (tooltip.NullOrEmpty())
        {
            return;
        }

        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }

        TooltipHandler.TipRegion(rect, tooltip);
    }

    private static string FighterLabel(Building_Bell bell, int index)
    {
        if (index == 0)
        {
            if (bell.fighter1.p != null)
            {
                return bell.fighter1.p.AnimalOrWildMan()
                    ? $"{bell.fighter1.p.Name.ToStringShort} ({bell.fighter1.p.def.race.AnyPawnKind.label})"
                    : bell.fighter1.p.Name.ToStringShort;
            }
        }

        if (index != 1)
        {
            return "PA.Select".Translate();
        }

        if (bell.fighter2.p == null)
        {
            return "PA.Select".Translate();
        }

        return bell.fighter2.p.AnimalOrWildMan()
            ? $"{bell.fighter2.p.Name.ToStringShort} ({bell.fighter2.p.def.race.AnyPawnKind.label})"
            : bell.fighter2.p.Name.ToStringShort;
    }
}