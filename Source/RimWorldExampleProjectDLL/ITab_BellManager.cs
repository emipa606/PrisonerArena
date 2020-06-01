using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ArenaBell
{
    // Token: 0x0200001B RID: 27
    internal class ITab_BellManager : ITab
    {
        // Token: 0x1700000C RID: 12
        // (get) Token: 0x0600007F RID: 127 RVA: 0x00004664 File Offset: 0x00002864
        protected Building_Bell SelectBell
        {
            get
            {
                return (Building_Bell)base.SelThing;
            }
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00004681 File Offset: 0x00002881
        public ITab_BellManager()
        {
            this.size = new Vector2(550f, 315f);
            this.labelKey = "TabManage";
        }

        // Token: 0x06000081 RID: 129 RVA: 0x000046AC File Offset: 0x000028AC
        protected override void FillTab()
        {
            float num = 50f;
            Rect rect = GenUI.ContractedBy(new Rect(0f, num, this.size.x, this.size.y), 5f);
            List<TabRecord> pages = new List<TabRecord>();
            TabRecord item = new TabRecord("Manage", delegate ()
            {
                ITab_BellManager.tab = ITab_BellManager.ArenaCardTab.Manager;
            }, ITab_BellManager.tab == ITab_BellManager.ArenaCardTab.Manager);
            pages.Add(item);
            TabRecord item2 = new TabRecord("Leaderboard", delegate ()
            {
                ITab_BellManager.tab = ITab_BellManager.ArenaCardTab.Leaderboard;
            }, ITab_BellManager.tab == ITab_BellManager.ArenaCardTab.Leaderboard);
            pages.Add(item2);
            TabDrawer.DrawTabs(rect, pages, 200f);
            Rect rectTabs = new Rect(0f, num, rect.width, rect.height - num);

            bool flag = ITab_BellManager.tab == ITab_BellManager.ArenaCardTab.Manager;
            if (flag)
            {
                this.FillTabManager(rectTabs);
            }
            else
            {
                bool flag2 = ITab_BellManager.tab == ITab_BellManager.ArenaCardTab.Leaderboard;
                if (flag2)
                {
                    this.FillTabLeaderboard(rectTabs);
                }
            }
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000047BC File Offset: 0x000029BC
        private void FillTabManager(Rect rect)
        {
            ITab_BellManager.listingStandard.Begin(rect);
            float offset0 = GUI.skin.label.CalcSize(new GUIContent("Welcome_to_the_arena!")).x / 2f;
            ITab_BellManager.centeredText("Welcome to the arena!", new Vector2(rect.xMax / 2f - offset0 - 2f, 10f), 100, 0);
            Widgets.DrawLineHorizontal(rect.x - 10f, 35f, rect.width - 15f);
            string label0 = ITab_BellManager.FighterLabel(this.SelectBell, 0);
            ITab_BellManager.DrawButton(delegate
            {
                ITab_BellManagerUtility.OpenActor1SelectMenu(this.SelectBell);
            }, label0, new Vector2(rect.xMax - ITab_BellManager.buttonSize.x - 10f, 75f), "Fighter #2", true);
            string label = ITab_BellManager.FighterLabel(this.SelectBell, 1);
            ITab_BellManager.DrawButton(delegate
            {
                ITab_BellManagerUtility.OpenActor2SelectMenu(this.SelectBell);
            }, label, new Vector2(rect.xMin, 75f), "Fighter #1", true);
            float offset = GUI.skin.label.CalcSize(new GUIContent("Vs.")).x / 2f;
            ITab_BellManager.centeredText("Vs.", new Vector2(rect.xMax / 2f - offset, 75f), 0, 0);
            bool flag = this.SelectBell.currentState == Building_Bell.State.rest;
            if (flag)
            {
                ITab_BellManager.DrawButton(delegate
                {
                    this.SelectBell.brawl();
                }, "Brawl!", new Vector2(rect.xMax / 2f - ITab_BellManager.buttonSize.x / 2f, 135f), "Let the brawl begin!", true);
            }
            else
            {
                bool flag2 = this.SelectBell.currentState == Building_Bell.State.preparation || this.SelectBell.currentState == Building_Bell.State.scheduled;
                if (flag2)
                {
                    ITab_BellManager.DrawButton(delegate
                    {
                        this.SelectBell.TryCancelBrawl("");
                    }, "Cancel", new Vector2(rect.xMax / 2f - ITab_BellManager.buttonSize.x / 2f, 135f), "Cancel the brawl", true);
                }
                else
                {
                    bool flag3 = this.SelectBell.currentState == Building_Bell.State.fight;
                    if (flag3)
                    {
                        ITab_BellManager.DrawButton(delegate
                        {
                            this.SelectBell.endBrawl(null, true);
                        }, "Suspend the brawl", new Vector2(rect.xMax / 2f - ITab_BellManager.buttonSize.x / 2f, 135f), "Suspend the brawl", true);
                    }
                }
            }
            string currentChoice = "No killing!";
            if (SelectBell.toDeath) { currentChoice = "To the Death!"; }
            ITab_BellManager.DrawButton(delegate
            {
                ITab_BellManagerUtility.OpenFightTypeMenu(this.SelectBell);
            }, currentChoice, new Vector2(rect.xMax / 2f - ITab_BellManager.buttonSize.x / 2f, 175f), "Fight rules", true); //rect.xMin + 100f
            //currentChoice = "For the Glory!";
            //if (SelectBell.winnerGetsFreedom) { currentChoice = "For my Freedom!"; }
            //ITab_BellManager.DrawButton(delegate
            //{
            //    ITab_BellManagerUtility.OpenRewardTypeMenu(this.SelectBell);
            //}, currentChoice, new Vector2(rect.xMax - ITab_BellManager.buttonSize.x - 100f, 175f), "Winner reward", true);
            ITab_BellManager.listingStandard.Gap();
            ITab_BellManager.listingStandard.End();
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00004A54 File Offset: 0x00002C54
        private void FillTabLeaderboard(Rect rect)
        {
            ITab_BellManager.listingStandard.Begin(rect);
            float widthOffset = GUI.skin.label.CalcSize(new GUIContent("Winners")).x / 2f;
            ITab_BellManager.centeredText("Winners", new Vector2(rect.xMax / 2f - widthOffset, 10f), 0, 0);
            Widgets.DrawLineHorizontal(rect.x - 0f, 35f, rect.width - 15f);
            if (SelectBell.winners.Count == 0)
            {
                widthOffset = GUI.skin.label.CalcSize(new GUIContent("No winners yet")).x / 2f;
                ITab_BellManager.centeredText("No winners yet", new Vector2(rect.xMax / 2f - widthOffset, 135f), 0, 0);
            }
            else
            {
                float heightOffset = 40f;
                float lineHeight = 25f;
                var g = SelectBell.winners.GroupBy(i => i).OrderByDescending(group => group.Count());
                foreach (var grp in g)
                {
                    string currentWinner = $"{grp.Key} { grp.Count()} time";
                    if (grp.Count() > 1) { currentWinner += "s"; }
                    widthOffset = GUI.skin.label.CalcSize(new GUIContent(currentWinner)).x / 2f;
                    Rect row = new Rect(rect.xMax / 2f - widthOffset, heightOffset, rect.xMax, lineHeight);
                    Widgets.Label(row, currentWinner);
                    heightOffset = heightOffset + lineHeight;
                }
            }
            ITab_BellManager.listingStandard.End();
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00004AE4 File Offset: 0x00002CE4
        private void FillTabArea(Rect rect)
        {
            ITab_BellManager.LabelWithTooltip("Fighting Area", "Area in which prisoners will fight. Enclosed spaces recommended.", true);
            this.DoAreaRestriction(ITab_BellManager.listingStandard, this.SelectBell.Map.areaManager.Home, new Action<Area>(this.SetAreaRestriction), new Func<Area, string>(AreaUtility.AreaAllowedLabel_Area));
        }

        // Token: 0x06000085 RID: 133 RVA: 0x00004B3C File Offset: 0x00002D3C
        private static void LabelWithTooltip(string label, string tooltip, bool centered = false)
        {
            Rect rect = ITab_BellManager.listingStandard.GetRect(Text.CalcHeight(label, ITab_BellManager.listingStandard.ColumnWidth));
            Vector2 offset = default(Vector2);
            if (centered)
            {
                offset.x = rect.xMax / 2f - GUI.skin.label.CalcSize(new GUIContent(label)).x / 2f;
            }
            Rect rectText = rect;
            rectText.x = offset.x;
            Widgets.Label(rectText, label);
            ITab_BellManager.DoTooltip(rect, tooltip);
        }

        // Token: 0x06000086 RID: 134 RVA: 0x00004BC8 File Offset: 0x00002DC8
        private static void DrawButton(Action action, string text, Vector2 pos, string tooltip = null, bool active = true)
        {
            Rect rect = new Rect(pos.x, pos.y, ITab_BellManager.buttonSize.x, ITab_BellManager.buttonSize.y);
            bool flag = !tooltip.NullOrEmpty();
            if (flag)
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
            bool flag2 = Widgets.ButtonText(rect, text, true, false, Color.white, active);
            if (flag2)
            {
                SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera(null);
                action();
            }
        }

        // Token: 0x06000087 RID: 135 RVA: 0x00004C44 File Offset: 0x00002E44
        private static void centeredText(string label, Vector2 pos, int offX = 0, int offY = 0)
        {
            Rect rect = new Rect(pos.x, pos.y, ITab_BellManager.buttonSize.x + (float)offX, ITab_BellManager.buttonSize.y + (float)offY);
            Widgets.Label(rect, label);
        }

        // Token: 0x06000088 RID: 136 RVA: 0x00004C88 File Offset: 0x00002E88
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

        // Token: 0x06000089 RID: 137 RVA: 0x00004CC3 File Offset: 0x00002EC3
        private void SetAreaRestriction(Area area)
        {
            this.SelectBell.FightingArea = area;
        }

        // Token: 0x0600008A RID: 138 RVA: 0x00004CD4 File Offset: 0x00002ED4
        private void DoAreaRestriction(Listing_Standard listing, Area area, Action<Area> setArea, Func<Area, string> getLabel)
        {
            Rect areaRect = listing.GetRect(48f);
            this.DoAllowedAreaSelectors(areaRect, this.SelectBell, getLabel);
            Area newArea = this.SelectBell.FightingArea;
            this.SelectBell.FightingArea = null;
            Text.Anchor = 0;
            bool flag = newArea != area;
            if (flag)
            {
                setArea(newArea);
            }
        }

        // Token: 0x0600008B RID: 139 RVA: 0x00004D34 File Offset: 0x00002F34
        public void DoAllowedAreaSelectors(Rect rect, Building_Bell b, Func<Area, string> getLabel)
        {
            bool flag = Find.CurrentMap == null;
            if (!flag)
            {
                Area[] areas = ITab_BellManager.GetAreas().ToArray<Area>();
                int num = areas.Length + 1;
                float num2 = rect.width / (float)num;
                Text.WordWrap = false;
                Text.Font = GameFont.Tiny;
                Rect rect2 = new Rect(rect.x, rect.y, num2, rect.height);
                ITab_BellManager.DoAreaSelector(rect2, b, null, getLabel);
                int num3 = 1;
                foreach (Area a in areas)
                {
                    bool flag2 = a == this.SelectBell.Map.areaManager.Home;
                    if (!flag2)
                    {
                        float num4 = (float)num3 * num2;
                        Rect rect3 = new Rect(rect.x + num4, rect.y, num2, rect.height);
                        ITab_BellManager.DoAreaSelector(rect3, b, a, getLabel);
                        num3++;
                    }
                }
                Text.WordWrap = true;
                Text.Font = GameFont.Small;
            }
        }

        // Token: 0x0600008C RID: 140 RVA: 0x00004E34 File Offset: 0x00003034
        public static IEnumerable<Area> GetAreas()
        {
            return from a in Find.CurrentMap.areaManager.AllAreas
                   where a.AssignableAsAllowed()
                   select a;
        }

        // Token: 0x0600008D RID: 141 RVA: 0x00004E7C File Offset: 0x0000307C
        private static void DoAreaSelector(Rect rect, Building_Bell b, Area area, Func<Area, string> getLabel)
        {
            rect = GenUI.ContractedBy(rect, 1f);
            GUI.DrawTexture(rect, (area == null) ? BaseContent.GreyTex : area.ColorTexture);
            Text.Anchor = TextAnchor.MiddleLeft;
            string text = getLabel(area);
            Rect rect2 = rect;
            rect2.xMin += 3f;
            rect2.yMin += 2f;
            Widgets.Label(rect2, text);
            bool flag = b.FightingArea == area;
            if (flag)
            {
                Widgets.DrawBox(rect, 2);
            }
            bool flag2 = Mouse.IsOver(rect);
            if (flag2)
            {
                if (area != null)
                {
                    area.MarkForDraw();
                }
                bool flag3 = Input.GetMouseButton(0) && b.FightingArea != area;
                if (flag3)
                {
                    b.FightingArea = area;
                    SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera(null);
                }
            }
            Text.Anchor = 0;
            TooltipHandler.TipRegion(rect, text);
        }

        // Token: 0x0600008E RID: 142 RVA: 0x00004F68 File Offset: 0x00003168
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

        // Token: 0x0400002F RID: 47
        private static Listing_Standard listingStandard = new Listing_Standard();

        // Token: 0x04000030 RID: 48
        protected static readonly Vector2 buttonSize = new Vector2(120f, 30f);

        // Token: 0x04000031 RID: 49
        public static ITab_BellManager.ArenaCardTab tab = ITab_BellManager.ArenaCardTab.Manager;

        // Token: 0x02000029 RID: 41
        public enum ArenaCardTab : byte
        {
            // Token: 0x0400005E RID: 94
            Manager,
            // Token: 0x0400005F RID: 95
            Leaderboard
        }
    }
}
