using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ArenaBell;

public class Building_Bell : Building //, IBillGiver
{
    public enum State
    {
        rest,

        preparation,

        fight,

        scheduled
    }

    public State currentState = State.rest;

    public Fighter fighter1 = new();

    public Fighter fighter2 = new();

    private Area fightingArea_int;

    public bool toDeath;

    public bool winnerGetsFreedom;

    public List<TaggedString> winners = [];

    public Area FightingArea
    {
        get
        {
            var result = fightingArea_int != null && fightingArea_int.Map != MapHeld ? null : fightingArea_int;
            return result;
        }
        set => fightingArea_int = value;
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

    public void Brawl()
    {
        if (isBusy())
        {
            var state = currentState;
            switch (state)
            {
                case State.preparation:
                    Messages.Message("PA.PlannedBrawl".Translate(), MessageTypeDefOf.RejectInput);
                    return;
                case State.fight:
                    Messages.Message("PA.ActiveBrawl".Translate(), MessageTypeDefOf.RejectInput);
                    return;
            }
        }

        if (fighter1.p == null || fighter2.p == null)
        {
            Messages.Message("PA.SelectTwo".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        if (fighter1.p == fighter2.p)
        {
            Messages.Message("PA.SelectDifferent".Translate(),
                MessageTypeDefOf.RejectInput);
            return;
        }

        if (!fightCapable(fighter1.p))
        {
            Messages.Message("PA.CantMove".Translate(fighter1.p.Name.ToStringShort),
                MessageTypeDefOf.RejectInput);
        }

        if (!fightCapable(fighter2.p))
        {
            Messages.Message("PA.CantMove".Translate(fighter2.p.Name.ToStringShort),
                MessageTypeDefOf.RejectInput);
        }

        currentState = State.scheduled;
    }

    public void TryCancelBrawl(string reason = "")
    {
        currentState = State.rest;
        Messages.Message("PA.NoBrawl".Translate(reason), MessageTypeDefOf.NegativeEvent);
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
        _ = f.p;
        _ = getOtherFighter(f).p;
        mentalStateHandler.TryStartMentalState(ArenaFighting, "", false, false, false, null, true);
        if (f.p.MentalState is not MentalState_Fighter mentalState)
        {
            return;
        }

        mentalState.otherPawn = getOtherFighter(f).p;
        mentalState.bellRef = this;
    }

    public void EndBrawl(Pawn pawn = null, bool suspended = false)
    {
        currentState = State.rest;
        Pawn winner = null;
        Pawn loser = null;
        if (suspended)
        {
            Messages.Message("PA.BrawlPause".Translate(), MessageTypeDefOf.RejectInput);
        }
        else
        {
            if (pawn != null)
            {
                Messages.Message("PA.Winner".Translate(pawn.Name.ToStringShort), MessageTypeDefOf.RejectInput);
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

                winner?.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOfArena.ArenaWinner);

                if (winnerGetsFreedom)
                {
                    if (winner is { IsPrisonerOfColony: true })
                    {
                        GenGuest.PrisonerRelease(winner);
                        Messages.Message("PA.WonFreedom".Translate(pawn.NameFullColored),
                            MessageTypeDefOf.PositiveEvent);
                    }

                    if (winner is { IsSlaveOfColony: true })
                    {
                        GenGuest.SlaveRelease(winner);
                        if (winner.IsWildMan())
                        {
                            winner.mindState.WildManEverReachedOutside = false;
                        }

                        Messages.Message("PA.WonFreedom".Translate(pawn.NameFullColored),
                            MessageTypeDefOf.PositiveEvent);
                    }
                }
            }

            if (loser is { Dead: false })
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

    public void PrisonerDelivered(Pawn p)
    {
        GetFighter(p).isInFight = true;
        startFightingState(GetFighter(p));
        if (!(fighter1.isInFight && fighter2.isInFight))
        {
            return;
        }

        Messages.Message("PA.FightStarting".Translate(), MessageTypeDefOf.RejectInput);
        currentState = State.fight;
        startFightingState(fighter1);
        startFightingState(fighter2);
    }

    public void StartTheShow()
    {
        LordMaker.MakeNewLord(Faction, new LordJob_Joinable_FightingMatch(Position, this), Map);
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        stringBuilder.Append("PA.CurrentState".Translate(currentState.ToString()));
        return stringBuilder.ToString();
    }

    private static bool fightCapable(Pawn p)
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

    private bool isBusy()
    {
        return isInFight() || isPreparing();
    }

    private bool isInFight()
    {
        return currentState == State.fight;
    }

    private bool isPreparing()
    {
        return currentState == State.preparation;
    }

    public IntVec3 GetFighterStandPoint()
    {
        var comp = GetComp<CompBell>();
        var isInFight = fighter1.isInFight;
        var orderedCells = comp.ValidCells.OrderBy(vec3 => vec3.x + vec3.z);

        return isInFight ? orderedCells.First() : orderedCells.Last();
    }

    public Fighter GetFighter(Pawn p)
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

    public Pawn GetPrisonerForHaul()
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