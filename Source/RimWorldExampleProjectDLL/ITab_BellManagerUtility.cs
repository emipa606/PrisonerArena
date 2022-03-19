using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ArenaBell;

internal class ITab_BellManagerUtility
{
    private static float buttonSize = 40f;

    private static float spacingOffset = 15f;

    private static float columnSize = 245f;

    public static string FighterLabel(int index, Building_Bell bell)
    {
        string result;
        if (bell.fighter1.p == null)
        {
            result = "Select";
        }
        else
        {
            if (index == 0)
            {
                result = bell.fighter1.p.Name.ToStringShort;
                if (bell.fighter1.p.AnimalOrWildMan())
                {
                    result += $" ({bell.fighter1.p.def.race.AnyPawnKind.label})";
                }
            }
            else
            {
                if (index == 1)
                {
                    result = bell.fighter2.p.Name.ToStringShort;
                    if (bell.fighter2.p.AnimalOrWildMan())
                    {
                        result += $" ({bell.fighter2.p.def.race.AnyPawnKind.label})";
                    }
                }
                else
                {
                    result = "error";
                }
            }
        }

        return result;
    }

    public static void OpenFightTypeMenu(Building_Bell bell)
    {
        var list = new List<FloatMenuOption>();

        void DeathAction()
        {
            bell.toDeath = true;
        }

        void DownedAction()
        {
            bell.toDeath = false;
        }

        list.Add(new FloatMenuOption("No killing!", DownedAction));
        list.Add(new FloatMenuOption("To the Death!", DeathAction));
        Find.WindowStack.Add(new FloatMenu(list));
    }

    public static void OpenRewardTypeMenu(Building_Bell bell)
    {
        var list = new List<FloatMenuOption>();

        void GloryAction()
        {
            bell.winnerGetsFreedom = false;
        }

        void FreedomAction()
        {
            bell.winnerGetsFreedom = true;
        }

        list.Add(new FloatMenuOption("For Glory!", GloryAction));
        list.Add(new FloatMenuOption("For Freedom!", FreedomAction));
        Find.WindowStack.Add(new FloatMenu(list));
    }

    public static void OpenActor1SelectMenu(Building_Bell bell)
    {
        var actorList = new List<Pawn>();

        if (bell.Map.mapPawns.PrisonersOfColonySpawned == null ||
            bell.Map.mapPawns.PrisonersOfColonySpawnedCount <= 0)
        {
            Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput);
        }
        else
        {
            foreach (var candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
            {
                actorList.Add(candidate);
            }

            foreach (var candidate in bell.Map.mapPawns.AllPawns)
            {
                if (candidate.Faction != null && candidate.Faction.IsPlayer && candidate.RaceProps.Animal)
                {
                    actorList.Add(candidate);
                }
            }

            if (actorList.Count <= 0)
            {
                Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput);
                return;
            }

            var list = new List<FloatMenuOption>();
            foreach (var actor in actorList)
            {
                var localCol = actor;

                void Action()
                {
                    if (localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                    {
                        bell.fighter1.p = localCol;
                    }
                    else
                    {
                        Messages.Message(localCol.Name.ToStringShort + " can't move and won't be a good fighter.",
                            MessageTypeDefOf.RejectInput);
                    }
                }

                var label = localCol.LabelShort;

                if (localCol.AnimalOrWildMan())
                {
                    label += $" ({localCol.def.race.AnyPawnKind.label})";
                }

                list.Add(new FloatMenuOption(label, Action));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }

    public static void OpenActor2SelectMenu(Building_Bell bell)
    {
        var actorList = new List<Pawn>();

        if (bell.Map.mapPawns.PrisonersOfColonySpawned == null ||
            bell.Map.mapPawns.PrisonersOfColonySpawnedCount <= 0)
        {
            Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput);
        }
        else
        {
            foreach (var candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
            {
                actorList.Add(candidate);
            }

            foreach (var candidate in bell.Map.mapPawns.AllPawns)
            {
                if (candidate.Faction != null && candidate.Faction.IsPlayer && candidate.RaceProps.Animal)
                {
                    actorList.Add(candidate);
                }
            }

            if (actorList.Count <= 0)
            {
                Messages.Message("No prisoners available.", MessageTypeDefOf.RejectInput);
                return;
            }

            var list = new List<FloatMenuOption>();
            foreach (var actor in actorList)
            {
                var localCol = actor;

                void Action()
                {
                    if (localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                    {
                        bell.fighter2.p = localCol;
                    }
                    else
                    {
                        Messages.Message(localCol.Name.ToStringShort + " can't move and won't be a good fighter.",
                            MessageTypeDefOf.RejectInput);
                    }
                }

                var label = localCol.LabelShort;

                if (localCol.AnimalOrWildMan())
                {
                    label += $" ({localCol.def.race.AnyPawnKind.label})";
                }

                list.Add(new FloatMenuOption(label, Action));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}