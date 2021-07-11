using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell
{
    // Token: 0x02000016 RID: 22
    public class Building_Bell : Building, IBillGiver
    {
        // Token: 0x02000024 RID: 36
        public enum State
        {
            // Token: 0x04000053 RID: 83
            rest,

            // Token: 0x04000054 RID: 84
            preparation,

            // Token: 0x04000055 RID: 85
            fight,

            // Token: 0x04000056 RID: 86
            scheduled
        }

        // Token: 0x04000022 RID: 34
        public State currentState = State.rest;

        // Token: 0x0400002A RID: 42
        private bool destroyedFlag;

        // Token: 0x04000023 RID: 35
        public Fighter fighter1 = new Fighter();

        // Token: 0x04000024 RID: 36
        public Fighter fighter2 = new Fighter();

        // Token: 0x04000021 RID: 33
        private Area fightingArea_int;

        // Token: 0x04000025 RID: 37
        private bool firstReadyDebug = false;

        // Token: 0x04000026 RID: 38
        private bool secondReadyDebug = false;

        // Token: 0x04000029 RID: 41
        public bool toDeath;

        // Token: 0x04000027 RID: 39
        protected int wickTicksLeft = 0;

        // Token: 0x04000028 RID: 40
        public bool winnerGetsFreedom;

        public List<TaggedString> winners = new List<TaggedString>();

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600004D RID: 77 RVA: 0x000035E4 File Offset: 0x000017E4
        // (set) Token: 0x0600004E RID: 78 RVA: 0x00003625 File Offset: 0x00001825
        public Area FightingArea
        {
            get
            {
                var result = fightingArea_int != null && fightingArea_int.Map != MapHeld ? null : fightingArea_int;
                return result;
            }
            set => fightingArea_int = value;
        }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600004F RID: 79 RVA: 0x0000362F File Offset: 0x0000182F
        public IEnumerable<IntVec3> IngredientStackCells => throw new NotImplementedException();

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000050 RID: 80 RVA: 0x0000362F File Offset: 0x0000182F
        public BillStack BillStack => throw new NotImplementedException();

        // Token: 0x0600005E RID: 94 RVA: 0x00003C6B File Offset: 0x00001E6B
        public bool CurrentlyUsableForBills()
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00003C6B File Offset: 0x00001E6B
        public bool UsableForBillsAfterFueling()
        {
            throw new NotImplementedException();
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00003636 File Offset: 0x00001836

        // Token: 0x06000052 RID: 82 RVA: 0x00003644 File Offset: 0x00001844
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

        // Token: 0x06000053 RID: 83 RVA: 0x000036CA File Offset: 0x000018CA
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            destroyedFlag = true;
            base.Destroy(mode);
        }

        // Token: 0x06000054 RID: 84 RVA: 0x000036DC File Offset: 0x000018DC
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

        // Token: 0x06000055 RID: 85 RVA: 0x00003838 File Offset: 0x00001A38
        public void TryCancelBrawl(string reason = "")
        {
            currentState = State.rest;
            Messages.Message("No brawl today laddies. " + reason, MessageTypeDefOf.NegativeEvent);
            fighter1 = new Fighter();
            fighter2 = new Fighter();
        }

        // Token: 0x06000056 RID: 86 RVA: 0x00003870 File Offset: 0x00001A70
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

        // Token: 0x06000057 RID: 87 RVA: 0x00003930 File Offset: 0x00001B30
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

        // Token: 0x06000058 RID: 88 RVA: 0x00003A42 File Offset: 0x00001C42
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

        // Token: 0x06000059 RID: 89 RVA: 0x00003A48 File Offset: 0x00001C48
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

        // Token: 0x0600005A RID: 90 RVA: 0x00003B14 File Offset: 0x00001D14
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

        // Token: 0x0600005B RID: 91 RVA: 0x00003B7C File Offset: 0x00001D7C
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

        // Token: 0x0600005C RID: 92 RVA: 0x00003BF8 File Offset: 0x00001DF8
        public void startTheShow()
        {
            LordMaker.MakeNewLord(Faction, new LordJob_Joinable_FightingMatch(Position, this), Map);
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00003C1C File Offset: 0x00001E1C
        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            stringBuilder.Append("Current state: ");
            stringBuilder.Append(currentState.ToString());
            return stringBuilder.ToString();
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00003C74 File Offset: 0x00001E74
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

        // Token: 0x06000061 RID: 97 RVA: 0x00003CBA File Offset: 0x00001EBA
        private bool IsBusy()
        {
            return IsInFight() || IsPreparing();
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00003CCD File Offset: 0x00001ECD
        private bool IsInFight()
        {
            return currentState == State.fight;
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00003CD8 File Offset: 0x00001ED8
        private bool IsPreparing()
        {
            return currentState == State.preparation;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00003CE4 File Offset: 0x00001EE4
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

        // Token: 0x06000065 RID: 101 RVA: 0x00003D50 File Offset: 0x00001F50
        private IntVec3 getCorner(bool nearest)
        {
            var result = nearest ? fightingArea_int.ActiveCells.First() : fightingArea_int.ActiveCells.Last();
            return result;
        }

        // Token: 0x06000066 RID: 102 RVA: 0x00003D8C File Offset: 0x00001F8C
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

        // Token: 0x06000067 RID: 103 RVA: 0x00003DD8 File Offset: 0x00001FD8
        public Pawn getPrisonerForHaul()
        {
            var p = !fighter1.isInFight ? fighter1.p : fighter2.p;
            return p;
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00003E18 File Offset: 0x00002018
        private Fighter getOtherFighter(Fighter f)
        {
            var result = f.p == fighter1.p ? fighter2 : fighter1;
            return result;
        }
    }
}