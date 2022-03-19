using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell;

public class Building_Bell : Building, IBillGiver
{
    public enum State
    {
        rest,

        preparation,

        fight,

        scheduled
    }

    public State currentState = State.rest;

    private bool destroyedFlag;

    public Fighter fighter1 = new Fighter();

    public Fighter fighter2 = new Fighter();

    private Area fightingArea_int;

    private bool firstReadyDebug = false;

    private bool secondReadyDebug = false;

    public bool toDeath;

    protected int wickTicksLeft = 0;

    public bool winnerGetsFreedom;

    public List<TaggedString> winners = new List<TaggedString>();

    public Area FightingArea
    {
        get
        {
            var result = fightingArea_int != null && fightingArea_int.Map != MapHeld ? null : fightingArea_int;
            return result;
        }
        set => fightingArea_int = value;
    }

    public IEnumerable<IntVec3> IngredientStackCells => throw new NotImplementedException();

    public BillStack BillStack => throw new NotImplementedException();

    public bool CurrentlyUsableForBills()
    {
        throw new NotImplementedException();
    }

    public bool UsableForBillsAfterFueling()
    {
        throw new NotImplementedException();
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref currentState, "currentState");
        Scribe_References.Look(ref fighter1.p, "fighter1p");
        Scribe_References.Look(ref fighter2.p, "fighter2p");
        Scribe_Values.Look(ref fighter1.isInFight, "fighter2f");
        Scribe_Values.Look(ref fighter2.isInFight, "fighter2f");
        Scribe_Values.Look(ref toDeath, "toDeath");
        Scribe_Values.Look(ref winnerGetsFreedom, "winnerGetsFreedom");
        Scribe_Collections.Look(ref winners, "winners", LookMode.Value);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        destroyedFlag = true;
        base.Destroy(mode);
    }

    public void brawl()
    {
        if (IsBusy())
        {
            var state = currentState;
            if (state == State.preparation)
            {
                Messages.Message("Brawl is already being held", MessageTypeDefOf.RejectInput);
                return;
            }

            if (state == State.fight)
            {
                Messages.Message("Brawl is already in the process", MessageTypeDefOf.RejectInput);
                return;
            }
        }

        if (fighter1.p == null || fighter2.p == null)
        {
            Messages.Message("Hey! Select two of them", MessageTypeDefOf.RejectInput);
        }
        else
        {
            if (fighter1.p == fighter2.p)
            {
                Messages.Message("Fighter can't be fighting themselves, select two different ones",
                    MessageTypeDefOf.RejectInput);
            }
            else
            {
                if (!fightCapable(fighter1.p))
                {
                    Messages.Message(fighter1.p.Name.ToStringShort + " can't move and won't be a good fighter.",
                        MessageTypeDefOf.RejectInput);
                }
                else
                {
                    if (!fightCapable(fighter2.p))
                    {
                        Messages.Message(fighter2.p.Name.ToStringShort + " can't move and won't be a good fighter.",
                            MessageTypeDefOf.RejectInput);
                    }
                    else
                    {
                        currentState = State.scheduled;
                    }
                }
            }
        }
    }

    public void TryCancelBrawl(string reason = "")
    {
        currentState = State.rest;
        Messages.Message("No brawl today laddies. " + reason, MessageTypeDefOf.NegativeEvent);
        fighter1 = new Fighter();
        fighter2 = new Fighter();
    }

    private void startFightingState(Fighter f)
    {
        f.p.mindState.enemyTarget = getOtherFighter(f).p;
        f.p.jobs.StopAll();
        f.p.mindState.mentalStateHandler.Reset();
        var mentalStateHandler = f.p.mindState.mentalStateHandler;
        var ArenaFighting = MentalStateDefOfArena.Fighter;
        var unused = f.p;
        var stateDef = ArenaFighting;
        var unused1 = getOtherFighter(f).p;
        mentalStateHandler.TryStartMentalState(stateDef, "", false, false, null, true);
        if (f.p.MentalState is not MentalState_Fighter mentalState)
        {
            return;
        }

        mentalState.otherPawn = getOtherFighter(f).p;
        mentalState.bellRef = this;
    }

    public void endBrawl(Pawn pawn = null, bool suspended = false)
    {
        currentState = State.rest;
        Pawn winner = null;
        Pawn loser = null;
        if (suspended)
        {
            Messages.Message("The brawl was suspended.", MessageTypeDefOf.RejectInput);
        }
        else
        {
            if (pawn != null)
            {
                Messages.Message("The winner is " + pawn.Name.ToStringShort + "!", MessageTypeDefOf.RejectInput);
                if (pawn == fighter1.p)
                {
                    winner = fighter1.p;
                    loser = fighter2.p;
                }
                else
                {
                    if (pawn == fighter2.p)
                    {
                        winner = fighter2.p;
                        loser = fighter1.p;
                    }
                }

                if (winnerGetsFreedom)
                {
                    if (winner != null && winner.IsPrisonerOfColony)
                    {
                        GenGuest.PrisonerRelease(winner);
                        Messages.Message(
                            pawn.NameFullColored +
                            " has won their freedom and will try to leave this place as soon as possible.",
                            MessageTypeDefOf.PositiveEvent);
                        //TryReleasePrisoner(winner);
                    }
                }
                else
                {
                    winner?.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOfArena.ArenaWinner);
                }
            }

            if (loser != null && !loser.Dead)
            {
                loser.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOfArena.ArenaLoser);
            }

            if (winner != null)
            {
                winners.Add(winner.NameFullColored);
            }
        }

        fighter1 = new Fighter();
        fighter2 = new Fighter();
    }

    private void DoTickerWork()
    {
    }

    public void TryReleasePrisoner(Pawn prisoner)
    {
        Pawn warden = null;
        foreach (var current in Map.mapPawns.FreeColonistsSpawned)
        {
            if (current.Dead)
            {
                continue;
            }

            if (!(current.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) &&
                  current.health.capacities.CapableOf(PawnCapacityDefOf.Moving)))
            {
                continue;
            }

            warden = current;
            break;
        }

        if (warden != null)
        {
            if (!RCellFinder.TryFindPrisonerReleaseCell(prisoner, warden, out var c))
            {
                Messages.Message("Could not find a suitable spot for releasing the prisoner",
                    MessageTypeDefOf.RejectInput);
                return;
            }

            Log.Message($"{warden.NameShortColored} will release {prisoner.NameShortColored}");
            var job = new Job(JobDefOf.ReleasePrisoner, prisoner, c);
            warden.jobs.EndCurrentJob(JobCondition.InterruptForced, false, false);
            Log.Message($"{warden.jobs.TryTakeOrderedJob(job)} result");
        }
        else
        {
            Messages.Message(
                "Not a warden in sight to give the winner their freedom, you need someone capable of releasing prisoners",
                MessageTypeDefOf.RejectInput);
        }
    }

    public void TryHaulPrisoners(Pawn prisoner)
    {
        Pawn warden = null;
        foreach (var current in Map.mapPawns.FreeColonistsSpawned)
        {
            if (current.Dead)
            {
                continue;
            }

            if (!(current.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) &&
                  current.health.capacities.CapableOf(PawnCapacityDefOf.Moving)))
            {
                continue;
            }

            warden = current;
            break;
        }

        if (warden != null)
        {
            StartHaulPrisoners(warden, prisoner);
        }
        else
        {
            Messages.Message(
                "Not a lad to bring the poor guy to the arena, you need someone capable of watching prisoners",
                MessageTypeDefOf.RejectInput);
        }
    }

    private void StartHaulPrisoners(Pawn warden, Pawn prisoner)
    {
        if (Destroyed || !Spawned)
        {
            TryCancelBrawl("Someone thrashed the bell!");
        }
        else
        {
            var job = new Job(JobDefOfArena.HaulingPrisoner, prisoner, this, getFighterStandPoint());
            warden.jobs.TryTakeOrderedJob(job);
        }
    }

    public void PrisonerDelievered(Pawn p)
    {
        getFighter(p).isInFight = true;
        startFightingState(getFighter(p));
        if (!(fighter1.isInFight && fighter2.isInFight))
        {
            return;
        }

        Messages.Message("The fight is starting", MessageTypeDefOf.RejectInput);
        currentState = State.fight;
        startFightingState(fighter1);
        startFightingState(fighter2);
    }

    public void startTheShow()
    {
        LordMaker.MakeNewLord(Faction, new LordJob_Joinable_FightingMatch(Position, this), Map);
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        stringBuilder.Append("Current state: ");
        stringBuilder.Append(currentState.ToString());
        return stringBuilder.ToString();
    }

    private bool fightCapable(Pawn p)
    {
        bool result;
        if (!p.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
        {
            result = false;
        }
        else
        {
            var inPainShock = p.health.InPainShock;
            result = !inPainShock;
        }

        return result;
    }

    private bool IsBusy()
    {
        return IsInFight() || IsPreparing();
    }

    private bool IsInFight()
    {
        return currentState == State.fight;
    }

    private bool IsPreparing()
    {
        return currentState == State.preparation;
    }

    public IntVec3 getFighterStandPoint()
    {
        var comp = GetComp<CompBell>();
        var unused = CellRect.CenteredOn(Position, 1).ExpandedBy(Mathf.RoundToInt(comp.radius - 1f)).Corners;

        var isInFight = fighter1.isInFight;
        IntVec3 result;
        var radius = 1f;

        if (isInFight)
        {
            while (comp.radius - radius > 0)
            {
                result = CellRect.CenteredOn(Position, 1).ExpandedBy(Mathf.RoundToInt(comp.radius - radius)).Corners
                    .First();
                if (result.GetRoom(Map) == Position.GetRoom(Map))
                {
                    return result;
                }

                radius = radius + 1f;
            }
        }
        else
        {
            while (comp.radius - radius > 0)
            {
                result = CellRect.CenteredOn(Position, 1).ExpandedBy(Mathf.RoundToInt(comp.radius - radius)).Corners
                    .Last();
                if (result.GetRoom(Map) == Position.GetRoom(Map))
                {
                    return result;
                }

                radius = radius + 1f;
            }
        }

        return Position;
    }

    private IntVec3 getCorner(bool nearest)
    {
        var result = nearest ? fightingArea_int.ActiveCells.First() : fightingArea_int.ActiveCells.Last();
        return result;
    }

    public Fighter getFighter(Pawn p)
    {
        Fighter result;
        if (p == fighter1.p)
        {
            result = fighter1;
        }
        else
        {
            result = p == fighter2.p ? fighter2 : null;
        }

        return result;
    }

    public Pawn getPrisonerForHaul()
    {
        var p = !fighter1.isInFight ? fighter1.p : fighter2.p;
        return p;
    }

    private Fighter getOtherFighter(Fighter f)
    {
        var result = f.p == fighter1.p ? fighter2 : fighter1;
        return result;
    }
}