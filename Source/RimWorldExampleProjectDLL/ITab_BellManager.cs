using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ArenaBell;

internal class ITab_BellManager : ITab
{
    private static readonly Listing_Standard listingStandard = new Listing_Standard();

    private static readonly Vector2 buttonSize = new Vector2(120f, 30f);

    private static ArenaCardTab tab = ArenaCardTab.Manager;

    public ITab_BellManager()
    {
        size = new Vector2(550f, 315f);
        labelKey = "TabManage";
    }

    private Building_Bell SelectBell => (Building_Bell)SelThing;

    protected override void FillTab()
    {
        var num = 50f;
        var rect = new Rect(0f, num, size.x, size.y).ContractedBy(5f);
        var pages = new List<TabRecord>();
        var item = new TabRecord("Manage", delegate { tab = ArenaCardTab.Manager; }, tab == ArenaCardTab.Manager);
        pages.Add(item);
        var item2 = new TabRecord("Leaderboard", delegate { tab = ArenaCardTab.Leaderboard; },
            tab == ArenaCardTab.Leaderboard);
        pages.Add(item2);
        TabDrawer.DrawTabs(rect, pages);
        var rectTabs = new Rect(0f, num, rect.width, rect.height - num);

        if (tab == ArenaCardTab.Manager)
        {
            FillTabManager(rectTabs);
        }
        else
        {
            if (tab == ArenaCardTab.Leaderboard)
            {
                FillTabLeaderboard(rectTabs);
            }
        }
    }

    private void FillTabManager(Rect rect)
    {
        listingStandard.Begin(rect);
        var offset0 = GUI.skin.label.CalcSize(new GUIContent("Welcome_to_the_arena!")).x / 2f;
        centeredText("Welcome to the arena!", new Vector2((rect.xMax / 2f) - offset0 - 2f, 10f), 100);
        Widgets.DrawLineHorizontal(rect.x - 10f, 35f, rect.width - 15f);
        var label0 = FighterLabel(SelectBell, 0);
        DrawButton(delegate { ITab_BellManagerUtility.OpenActor1SelectMenu(SelectBell); }, label0,
            new Vector2(rect.xMax - buttonSize.x - 100f, 75f), "Fighter #2");
        var label = FighterLabel(SelectBell, 1);
        DrawButton(delegate { ITab_BellManagerUtility.OpenActor2SelectMenu(SelectBell); }, label,
            new Vector2(rect.xMin + 100f, 75f), "Fighter #1");
        var offset = GUI.skin.label.CalcSize(new GUIContent("Vs.")).x / 2f;
        centeredText("Vs.", new Vector2((rect.xMax / 2f) - offset, 75f));
        if (SelectBell.currentState == Building_Bell.State.rest)
        {
            DrawButton(delegate { SelectBell.brawl(); }, "Brawl!",
                new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Let the brawl begin!");
        }
        else
        {
            if (SelectBell.currentState == Building_Bell.State.preparation ||
                SelectBell.currentState == Building_Bell.State.scheduled)
            {
                DrawButton(delegate { SelectBell.TryCancelBrawl(); }, "Cancel",
                    new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Cancel the brawl");
            }
            else
            {
                if (SelectBell.currentState == Building_Bell.State.fight)
                {
                    DrawButton(delegate { SelectBell.endBrawl(null, true); }, "Suspend the brawl",
                        new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Suspend the brawl");
                }
            }
        }

        var currentChoice = "No killing!";
        if (SelectBell.toDeath)
        {
            currentChoice = "To the Death!";
        }

        DrawButton(delegate { ITab_BellManagerUtility.OpenFightTypeMenu(SelectBell); }, currentChoice,
            new Vector2(rect.xMin + 100f, 195f), "Win condition");
        currentChoice = "For Glory!";
        if (SelectBell.winnerGetsFreedom)
        {
            currentChoice = "For Freedom!";
        }

        offset = GUI.skin.label.CalcSize(new GUIContent("Rules")).x / 2f;
        centeredText("Rules", new Vector2((rect.xMax / 2f) - offset, 185f));
        DrawButton(delegate { ITab_BellManagerUtility.OpenRewardTypeMenu(SelectBell); }, currentChoice,
            new Vector2(rect.xMax - buttonSize.x - 100f, 195f), "Winner reward");
        listingStandard.Gap();
        listingStandard.End();
    }

    private void FillTabLeaderboard(Rect rect)
    {
        listingStandard.Begin(rect);
        var widthOffset = GUI.skin.label.CalcSize(new GUIContent("Winners")).x / 2f;
        centeredText("Winners", new Vector2((rect.xMax / 2f) - widthOffset, 10f));
        Widgets.DrawLineHorizontal(rect.x - 0f, 35f, rect.width - 15f);
        if (SelectBell.winners.Count == 0)
        {
            widthOffset = GUI.skin.label.CalcSize(new GUIContent("No winners yet")).x / 2f;
            centeredText("No winners yet", new Vector2((rect.xMax / 2f) - widthOffset, 135f));
        }
        else
        {
            var heightOffset = 40f;
            var lineHeight = 25f;
            var g = SelectBell.winners.GroupBy(i => i).OrderByDescending(group => group.Count());
            foreach (var grp in g)
            {
                var currentWinner = $"{grp.Key} {grp.Count()} time";
                if (grp.Count() > 1)
                {
                    currentWinner += "s";
                }

                widthOffset = GUI.skin.label.CalcSize(new GUIContent(currentWinner)).x / 2f;
                var row = new Rect((rect.xMax / 2f) - widthOffset, heightOffset, rect.xMax, lineHeight);
                Widgets.Label(row, currentWinner);
                heightOffset += lineHeight;
            }
        }

        listingStandard.End();
    }

    private void FillTabArea()
    {
        LabelWithTooltip("Fighting Area", "Area in which prisoners will fight. Enclosed spaces recommended.", true);
        DoAreaRestriction(listingStandard, SelectBell.Map.areaManager.Home, SetAreaRestriction,
            AreaUtility.AreaAllowedLabel_Area);
    }

    private static void LabelWithTooltip(string label, string tooltip, bool centered = false)
    {
        var rect = listingStandard.GetRect(Text.CalcHeight(label, listingStandard.ColumnWidth));
        var offset = default(Vector2);
        if (centered)
        {
            offset.x = (rect.xMax / 2f) - (GUI.skin.label.CalcSize(new GUIContent(label)).x / 2f);
        }

        var rectText = rect;
        rectText.x = offset.x;
        Widgets.Label(rectText, label);
        DoTooltip(rect, tooltip);
    }

    private static void DrawButton(Action action, string text, Vector2 pos, string tooltip = null,
        bool active = true)
    {
        var rect = new Rect(pos.x, pos.y, buttonSize.x, buttonSize.y);
        if (!tooltip.NullOrEmpty())
        {
            TooltipHandler.TipRegion(rect, tooltip);
        }

        if (!Widgets.ButtonText(rect, text, true, false, Color.white, active))
        {
            return;
        }

        SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
        action();
    }

    private static void centeredText(string label, Vector2 pos, int offX = 0, int offY = 0)
    {
        var rect = new Rect(pos.x, pos.y, buttonSize.x + offX, buttonSize.y + offY);
        Widgets.Label(rect, label);
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

    private void SetAreaRestriction(Area area)
    {
        SelectBell.FightingArea = area;
    }

    private void DoAreaRestriction(Listing_Standard listing, Area area, Action<Area> setArea,
        Func<Area, string> getLabel)
    {
        var areaRect = listing.GetRect(48f);
        DoAllowedAreaSelectors(areaRect, SelectBell, getLabel);
        var newArea = SelectBell.FightingArea;
        SelectBell.FightingArea = null;
        Text.Anchor = 0;
        if (newArea != area)
        {
            setArea(newArea);
        }
    }

    private void DoAllowedAreaSelectors(Rect rect, Building_Bell b, Func<Area, string> getLabel)
    {
        if (Find.CurrentMap == null)
        {
            return;
        }

        var areas = GetAreas().ToArray();
        var num = areas.Length + 1;
        var num2 = rect.width / num;
        Text.WordWrap = false;
        Text.Font = GameFont.Tiny;
        var rect2 = new Rect(rect.x, rect.y, num2, rect.height);
        DoAreaSelector(rect2, b, null, getLabel);
        var num3 = 1;
        foreach (var a in areas)
        {
            if (a == SelectBell.Map.areaManager.Home)
            {
                continue;
            }

            var num4 = num3 * num2;
            var rect3 = new Rect(rect.x + num4, rect.y, num2, rect.height);
            DoAreaSelector(rect3, b, a, getLabel);
            num3++;
        }

        Text.WordWrap = true;
        Text.Font = GameFont.Small;
    }

    private static IEnumerable<Area> GetAreas()
    {
        return from a in Find.CurrentMap.areaManager.AllAreas
            where a.AssignableAsAllowed()
            select a;
    }

    private static void DoAreaSelector(Rect rect, Building_Bell b, Area area, Func<Area, string> getLabel)
    {
        rect = rect.ContractedBy(1f);
        GUI.DrawTexture(rect, area == null ? BaseContent.GreyTex : area.ColorTexture);
        Text.Anchor = TextAnchor.MiddleLeft;
        var text = getLabel(area);
        var rect2 = rect;
        rect2.xMin += 3f;
        rect2.yMin += 2f;
        Widgets.Label(rect2, text);
        if (b.FightingArea == area)
        {
            Widgets.DrawBox(rect, 2);
        }

        if (Mouse.IsOver(rect))
        {
            area?.MarkForDraw();
            if (Input.GetMouseButton(0) && b.FightingArea != area)
            {
                b.FightingArea = area;
                SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
            }
        }

        Text.Anchor = 0;
        TooltipHandler.TipRegion(rect, text);
    }

    private static string FighterLabel(Building_Bell bell, int index)
    {
        if (index == 0)
        {
            if (bell.fighter1.p != null)
            {
                return bell.fighter1.p.Name.ToStringShort;
            }
        }

        if (index != 1)
        {
            return "PA.Select".Translate();
        }

        if (bell.fighter2.p != null)
        {
            return bell.fighter2.p.Name.ToStringShort;
        }

        return "PA.Select".Translate();
    }

    private enum ArenaCardTab : byte
    {
        Manager,

        Leaderboard
    }
}