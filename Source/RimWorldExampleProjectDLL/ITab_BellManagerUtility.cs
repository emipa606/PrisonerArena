using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ArenaBell;

internal class ITab_BellManagerUtility
{
    public static string FighterLabel(int index, Building_Bell bell)
    {
        string result;
        if (bell.fighter1.p == null)
        {
            result = "PA.Select".Translate();
        }
        else
        {
            switch (index)
            {
                case 0:
                {
                    result = bell.fighter1.p.Name.ToStringShort;
                    if (bell.fighter1.p.AnimalOrWildMan())
                    {
                        result += $" ({bell.fighter1.p.def.race.AnyPawnKind.label})";
                    }

                    break;
                }
                case 1:
                {
                    result = bell.fighter2.p.Name.ToStringShort;
                    if (bell.fighter2.p.AnimalOrWildMan())
                    {
                        result += $" ({bell.fighter2.p.def.race.AnyPawnKind.label})";
                    }

                    break;
                }
                default:
                    result = "error";
                    break;
            }
        }

        return result;
    }

    public static void OpenFightTypeMenu(Building_Bell bell)
    {
        var list = new List<FloatMenuOption>
        {
            new FloatMenuOption("PA.NoKilling".Translate(), DownedAction),
            new FloatMenuOption("PA.Death".Translate(), DeathAction)
        };

        Find.WindowStack.Add(new FloatMenu(list));
        return;

        void DeathAction()
        {
            bell.toDeath = true;
        }

        void DownedAction()
        {
            bell.toDeath = false;
        }
    }

    public static void OpenRewardTypeMenu(Building_Bell bell)
    {
        var list = new List<FloatMenuOption>
        {
            new FloatMenuOption("PA.Glory".Translate(), GloryAction),
            new FloatMenuOption("PA.Freedom".Translate(), FreedomAction)
        };

        Find.WindowStack.Add(new FloatMenu(list));
        return;

        void GloryAction()
        {
            bell.winnerGetsFreedom = false;
        }

        void FreedomAction()
        {
            bell.winnerGetsFreedom = true;
        }
    }

    public static void OpenActor1SelectMenu(Building_Bell bell)
    {
        var actorList = new List<Pawn>();

        if (bell.Map.mapPawns.PrisonersOfColonySpawned == null ||
            bell.Map.mapPawns.PrisonersOfColonySpawnedCount <= 0)
        {
            Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
        }
        else
        {
            foreach (var candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
            {
                actorList.Add(candidate);
            }

            foreach (var candidate in bell.Map.mapPawns.AllPawns)
            {
                if (candidate.Faction is { IsPlayer: true } && candidate.RaceProps.Animal)
                {
                    actorList.Add(candidate);
                }
            }

            if (actorList.Count <= 0)
            {
                Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            var list = new List<FloatMenuOption>();
            foreach (var actor in actorList)
            {
                var localCol = actor;

                var label = localCol.LabelShort;

                if (localCol.AnimalOrWildMan())
                {
                    label += $" ({localCol.def.race.AnyPawnKind.label})";
                }

                list.Add(new FloatMenuOption(label, Action));
                continue;

                void Action()
                {
                    if (localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                    {
                        bell.fighter1.p = localCol;
                    }
                    else
                    {
                        Messages.Message("PA.CantMove".Translate(localCol.Name.ToStringShort),
                            MessageTypeDefOf.RejectInput);
                    }
                }
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
            Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
        }
        else
        {
            foreach (var candidate in bell.Map.mapPawns.PrisonersOfColonySpawned)
            {
                actorList.Add(candidate);
            }

            foreach (var candidate in bell.Map.mapPawns.AllPawns)
            {
                if (candidate.Faction is { IsPlayer: true } && candidate.RaceProps.Animal)
                {
                    actorList.Add(candidate);
                }
            }

            if (actorList.Count <= 0)
            {
                Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            var list = new List<FloatMenuOption>();
            foreach (var actor in actorList)
            {
                var localCol = actor;

                var label = localCol.LabelShort;

                if (localCol.AnimalOrWildMan())
                {
                    label += $" ({localCol.def.race.AnyPawnKind.label})";
                }

                list.Add(new FloatMenuOption(label, Action));
                continue;

                void Action()
                {
                    if (localCol.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                    {
                        bell.fighter2.p = localCol;
                    }
                    else
                    {
                        Messages.Message("PA.CantMove".Translate(localCol.Name.ToStringShort),
                            MessageTypeDefOf.RejectInput);
                    }
                }
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}