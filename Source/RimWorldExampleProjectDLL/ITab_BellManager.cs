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
    private static readonly Listing_Standard listingStandard = new();

    private static readonly Vector2 buttonSize = new(120f, 30f);

    private static ArenaCardTab tab = ArenaCardTab.Manager;

    public ITab_BellManager()
    {
        size = new Vector2(550f, 315f);
        labelKey = "TabManage";
    }

    private Building_Bell SelectBell => (Building_Bell)SelThing;

    protected override void FillTab()
    {
        const float num = 50f;
        var rect = new Rect(0f, num, size.x, size.y).ContractedBy(5f);
        var pages = new List<TabRecord>();
        var item = new TabRecord("Manage", delegate { tab = ArenaCardTab.Manager; }, tab == ArenaCardTab.Manager);
        pages.Add(item);
        var item2 = new TabRecord("Leaderboard", delegate { tab = ArenaCardTab.Leaderboard; },
            tab == ArenaCardTab.Leaderboard);
        pages.Add(item2);
        TabDrawer.DrawTabs(rect, pages);
        var rectTabs = new Rect(0f, num, rect.width, rect.height - num);

        switch (tab)
        {
            case ArenaCardTab.Manager:
                fillTabManager(rectTabs);
                break;
            case ArenaCardTab.Leaderboard:
                fillTabLeaderboard(rectTabs);
                break;
        }
    }

    private void fillTabManager(Rect rect)
    {
        listingStandard.Begin(rect);
        var offset0 = GUI.skin.label.CalcSize(new GUIContent("Welcome_to_the_arena!")).x / 2f;
        centeredText("Welcome to the arena!", new Vector2((rect.xMax / 2f) - offset0 - 2f, 10f), 100);
        Widgets.DrawLineHorizontal(rect.x - 10f, 35f, rect.width - 15f);
        var label0 = FighterLabel(SelectBell, 0);
        drawButton(delegate { ITab_BellManagerUtility.OpenActor1SelectMenu(SelectBell); }, label0,
            new Vector2(rect.xMax - buttonSize.x - 100f, 75f), "Fighter #2");
        var label = FighterLabel(SelectBell, 1);
        drawButton(delegate { ITab_BellManagerUtility.OpenActor2SelectMenu(SelectBell); }, label,
            new Vector2(rect.xMin + 100f, 75f), "Fighter #1");
        var offset = GUI.skin.label.CalcSize(new GUIContent("Vs.")).x / 2f;
        centeredText("Vs.", new Vector2((rect.xMax / 2f) - offset, 75f));
        switch (SelectBell.currentState)
        {
            case Building_Bell.State.rest:
                drawButton(delegate { SelectBell.Brawl(); }, "Brawl!",
                    new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Let the brawl begin!");
                break;
            case Building_Bell.State.preparation:
            case Building_Bell.State.scheduled:
                drawButton(delegate { SelectBell.TryCancelBrawl(); }, "Cancel",
                    new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Cancel the brawl");
                break;
            case Building_Bell.State.fight:
                drawButton(delegate { SelectBell.EndBrawl(null, true); }, "Suspend the brawl",
                    new Vector2((rect.xMax / 2f) - (buttonSize.x / 2f), 135f), "Suspend the brawl");
                break;
        }

        var currentChoice = "No killing!";
        if (SelectBell.toDeath)
        {
            currentChoice = "To the Death!";
        }

        drawButton(delegate { ITab_BellManagerUtility.OpenFightTypeMenu(SelectBell); }, currentChoice,
            new Vector2(rect.xMin + 100f, 195f), "Win condition");
        currentChoice = "For Glory!";
        if (SelectBell.winnerGetsFreedom)
        {
            currentChoice = "For Freedom!";
        }

        offset = GUI.skin.label.CalcSize(new GUIContent("Rules")).x / 2f;
        centeredText("Rules", new Vector2((rect.xMax / 2f) - offset, 185f));
        drawButton(delegate { ITab_BellManagerUtility.OpenRewardTypeMenu(SelectBell); }, currentChoice,
            new Vector2(rect.xMax - buttonSize.x - 100f, 195f), "Winner reward");
        listingStandard.Gap();
        listingStandard.End();
    }

    private void fillTabLeaderboard(Rect rect)
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
            const float lineHeight = 25f;
            var g = SelectBell.winners.GroupBy(taggedString => taggedString.RawText)
                .OrderByDescending(group => group.Count());
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

    private static void drawButton(Action action, string text, Vector2 pos, string tooltip = null,
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