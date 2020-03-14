using System;
using UnityEngine;
using Verse;

namespace ArenaBell
{
	// Token: 0x0200001A RID: 26
	internal class ITab_BellManagerInterface
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00003A42 File Offset: 0x00001C42
		public static void DrawArenaTab(Rect rect, Building_Bell bell)
		{
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004549 File Offset: 0x00002749
		private static void FillTabArea(Rect rect)
		{
			ITab_BellManagerInterface.LabelWithTooltip("Brawl area", "Remov later");
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000455C File Offset: 0x0000275C
		private static void LabelWithTooltip(string label, string tooltip)
		{
			Rect rect = ITab_BellManagerInterface.listingStandard.GetRect(Text.CalcHeight(label, ITab_BellManagerInterface.listingStandard.ColumnWidth));
			Widgets.Label(rect, label);
			ITab_BellManagerInterface.DoTooltip(rect, tooltip);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004598 File Offset: 0x00002798
		private static void DoTooltip(Rect rect, string tooltip)
		{
			bool flag = !tooltip.NullOrEmpty();
			if (flag)
			{
				bool flag2 = Mouse.IsOver(rect);
				if (flag2)
				{
					Widgets.DrawHighlight(rect);
				}
				TooltipHandler.TipRegion(rect, tooltip);
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000045D4 File Offset: 0x000027D4
		private static string FighterLabel(Building_Bell bell, int index)
		{
			bool flag = index == 0;
			if (flag)
			{
				bool flag2 = bell.fighter1.p != null;
				if (flag2)
				{
					return bell.fighter1.p.Name.ToStringShort;
				}
			}
			bool flag3 = index == 1;
			if (flag3)
			{
				bool flag4 = bell.fighter2.p != null;
				if (flag4)
				{
					return bell.fighter2.p.Name.ToStringShort;
				}
			}
			return "Select";
		}

		// Token: 0x0400002E RID: 46
		private static Listing_Standard listingStandard = new Listing_Standard();
	}
}
