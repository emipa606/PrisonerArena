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

    private static List<Pawn> allValidPawnsOnMap(Building_Bell bell)
    {
        var validActors = bell.Map.mapPawns.SlavesAndPrisonersOfColonySpawned;
        validActors.AddRange(bell.Map.mapPawns.SpawnedColonyAnimals);
        validActors.AddRange(bell.Map.mapPawns.SpawnedColonyMechs);
        validActors.AddRange(bell.Map.mapPawns.SpawnedColonyMutantsPlayerControlled);
        validActors.AddRange(bell.Map.mapPawns.FreeColonistsSpawned);
        return validActors;
    }

    public static void OpenActor1SelectMenu(Building_Bell bell)
    {
        var actorList = new List<Pawn>();
        var validActors = allValidPawnsOnMap(bell);

        if (!validActors.Any())
        {
            Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        foreach (var candidate in validActors)
        {
            if (bell.fighter2.p == candidate)
            {
                continue;
            }

            //if (!candidate.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            //{
            //    continue;
            //}

            actorList.Add(candidate);
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

            if (localCol.AnimalOrWildMan() || localCol.IsColonyMech)
            {
                label += $" ({localCol.def.race.AnyPawnKind.label})";
            }

            list.Add(new FloatMenuOption(label, Action));
            continue;

            void Action()
            {
                bell.fighter1.p = localCol;
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }

    public static void OpenActor2SelectMenu(Building_Bell bell)
    {
        var actorList = new List<Pawn>();
        var validActors = allValidPawnsOnMap(bell);

        if (!validActors.Any())
        {
            Messages.Message("PA.NoFighters".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        foreach (var candidate in validActors)
        {
            if (bell.fighter1.p == candidate)
            {
                continue;
            }

            //if (!candidate.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            //{
            //    continue;
            //}

            actorList.Add(candidate);
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

            if (localCol.AnimalOrWildMan() || localCol.IsColonyMech)
            {
                label += $" ({localCol.def.race.AnyPawnKind.label})";
            }

            list.Add(new FloatMenuOption(label, Action));
            continue;

            void Action()
            {
                bell.fighter2.p = localCol;
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }
}