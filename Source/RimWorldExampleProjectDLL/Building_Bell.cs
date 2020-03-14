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
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000035E4 File Offset: 0x000017E4
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00003625 File Offset: 0x00001825
		public Area FightingArea
		{
			get
			{
				bool flag = this.fightingArea_int != null && this.fightingArea_int.Map != base.MapHeld;
				Area result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.fightingArea_int;
				}
				return result;
			}
			set
			{
				this.fightingArea_int = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004F RID: 79 RVA: 0x0000362F File Offset: 0x0000182F
		public IEnumerable<IntVec3> IngredientStackCells
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000050 RID: 80 RVA: 0x0000362F File Offset: 0x0000182F
		public BillStack BillStack
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003636 File Offset: 0x00001836
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003644 File Offset: 0x00001844
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Building_Bell.State>(ref this.currentState, "currentState", Building_Bell.State.rest, false);
			Scribe_References.Look<Pawn>(ref this.fighter1.p, "fighter1p", false);
			Scribe_References.Look<Pawn>(ref this.fighter2.p, "fighter2p", false);
			Scribe_Values.Look<bool>(ref this.fighter1.isInFight, "fighter2f", false, false);
			Scribe_Values.Look<bool>(ref this.fighter2.isInFight, "fighter2f", false, false);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000036CA File Offset: 0x000018CA
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			this.destroyedFlag = true;
			base.Destroy(mode);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000036DC File Offset: 0x000018DC
		public void brawl()
		{
			bool flag = this.IsBusy();
			if (flag)
			{
				Building_Bell.State state = this.currentState;
				if (state == Building_Bell.State.preparation)
				{
					Messages.Message("Brawl is already being held", MessageTypeDefOf.RejectInput, true);
					return;
				}
				if (state == Building_Bell.State.fight)
				{
					Messages.Message("Brawl is already in the process", MessageTypeDefOf.RejectInput, true);
					return;
				}
			}
			bool flag2 = this.fighter1.p == null || this.fighter2.p == null;
			if (flag2)
			{
				Messages.Message("Hey! Select two of them", MessageTypeDefOf.RejectInput, true);
			}
			else
			{
				bool flag3 = this.fighter1.p == this.fighter2.p;
				if (flag3)
				{
					Messages.Message("Fighter can't be fighting themselves, select two different ones", MessageTypeDefOf.RejectInput, true);
				}
				else
				{
					bool flag4 = !this.fightCapable(this.fighter1.p);
					if (flag4)
					{
						Messages.Message(this.fighter1.p.Name.ToStringShort + " can't move and won't be a good fighter.", MessageTypeDefOf.RejectInput, true);
					}
					else
					{
						bool flag5 = !this.fightCapable(this.fighter2.p);
						if (flag5)
						{
							Messages.Message(this.fighter2.p.Name.ToStringShort + " can't move and won't be a good fighter.", MessageTypeDefOf.RejectInput, true);
						}
						else
						{
							this.currentState = Building_Bell.State.scheduled;
						}
					}
				}
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003838 File Offset: 0x00001A38
		public void TryCancelBrawl(string reason = "")
		{
			this.currentState = Building_Bell.State.rest;
			Messages.Message("No brawl today laddies. " + reason, MessageTypeDefOf.NegativeEvent, true);
			this.fighter1 = new Fighter();
			this.fighter2 = new Fighter();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003870 File Offset: 0x00001A70
		private void startFightingState(Fighter f)
		{
			f.p.mindState.enemyTarget = this.getOtherFighter(f).p;
			f.p.jobs.StopAll(false);
			f.p.mindState.mentalStateHandler.Reset();
			MentalStateHandler mentalStateHandler = f.p.mindState.mentalStateHandler;
			MentalStateDef ArenaFighting = MentalStateDefOfArena.Fighter;
			Pawn pawn = f.p;
			MentalStateDef stateDef = ArenaFighting;
			Pawn otherPawn = this.getOtherFighter(f).p;
			mentalStateHandler.TryStartMentalState(stateDef, "", false, false, null, true);
			MentalState_Fighter mentalState = f.p.MentalState as MentalState_Fighter;
			mentalState.otherPawn = this.getOtherFighter(f).p;
			mentalState.bellRef = this;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003930 File Offset: 0x00001B30
		public void endBrawl(Pawn pawn = null, bool suspended = false)
		{
			this.currentState = Building_Bell.State.rest;
			Pawn winner = null;
			Pawn loser = null;
			if (suspended)
			{
				Messages.Message("The brawl was suspended.", MessageTypeDefOf.RejectInput, true);
			}
			else
			{
				Messages.Message("The winner is " + pawn.Name.ToStringShort + "!", MessageTypeDefOf.RejectInput, true);
				bool flag = pawn == this.fighter1.p;
				if (flag)
				{
					winner = this.fighter1.p;
					loser = this.fighter2.p;
				}
				else
				{
					bool flag2 = pawn == this.fighter2.p;
					if (flag2)
					{
						winner = this.fighter2.p;
						loser = this.fighter1.p;
					}
				}
				winner.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfArena.ArenaWinner, null);
				loser.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfArena.ArenaLoser, null);
			}
			this.fighter1 = new Fighter();
			this.fighter2 = new Fighter();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003A42 File Offset: 0x00001C42
		private void DoTickerWork(int tickerAmount)
		{
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003A48 File Offset: 0x00001C48
		public void TryHaulPrisoners(Pawn prisoner)
		{
			Pawn warden = null;
			foreach (Pawn current in base.Map.mapPawns.FreeColonistsSpawned)
			{
				bool flag = !current.Dead;
				if (flag)
				{
					bool flag2 = current.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && current.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
					if (flag2)
					{
						warden = current;
						break;
					}
				}
			}
			bool flag3 = warden != null;
			if (flag3)
			{
				this.StartHaulPrisoners(warden, prisoner);
			}
			else
			{
				Messages.Message("Not a lad to bring the poor guy to the arena, you need someone capable of watching prisoners", MessageTypeDefOf.RejectInput, true);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003B14 File Offset: 0x00001D14
		private void StartHaulPrisoners(Pawn warden, Pawn prisoner)
		{
			bool flag = base.Destroyed || !base.Spawned;
			if (flag)
			{
				this.TryCancelBrawl("Someone thrashed the bell!");
			}
			else
			{
				Job job = new Job(JobDefOfArena.HaulingPrisoner, prisoner, this, this.getFighterStandPoint());
				warden.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003B7C File Offset: 0x00001D7C
		public void PrisonerDelievered(Pawn p)
		{
			this.getFighter(p).isInFight = true;
			this.startFightingState(this.getFighter(p));
			bool flag = this.fighter1.isInFight && this.fighter2.isInFight;
			if (flag)
			{
				Messages.Message("The fight is starting", MessageTypeDefOf.RejectInput, true);
				this.currentState = Building_Bell.State.fight;
				this.startFightingState(this.fighter1);
				this.startFightingState(this.fighter2);
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003BF8 File Offset: 0x00001DF8
		public void startTheShow()
		{
			LordMaker.MakeNewLord(base.Faction, new LordJob_Joinable_FightingMatch(base.Position, this), base.Map, null);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003C1C File Offset: 0x00001E1C
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.Append("Current state: ");
			stringBuilder.Append(this.currentState.ToString());
			return stringBuilder.ToString();
		}

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

		// Token: 0x06000060 RID: 96 RVA: 0x00003C74 File Offset: 0x00001E74
		private bool fightCapable(Pawn p)
		{
			bool flag = !p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool inPainShock = p.health.InPainShock;
				result = !inPainShock;
			}
			return result;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003CBA File Offset: 0x00001EBA
		private bool IsBusy()
		{
			return this.IsInFight() || this.IsPreparing();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003CCD File Offset: 0x00001ECD
		private bool IsInFight()
		{
			return this.currentState == Building_Bell.State.fight;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003CD8 File Offset: 0x00001ED8
		private bool IsPreparing()
		{
			return this.currentState == Building_Bell.State.preparation;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003CE4 File Offset: 0x00001EE4
		public IntVec3 getFighterStandPoint()
		{
			CompBell comp = base.GetComp<CompBell>();
			IEnumerable<IntVec3> fighterSpots = CellRect.CenteredOn(base.Position, 1).ExpandedBy(Mathf.RoundToInt(comp.radius - 1f)).Corners;
			bool isInFight = this.fighter1.isInFight;
			IntVec3 result;
			if (isInFight)
			{
				result = fighterSpots.First<IntVec3>();
			}
			else
			{
				result = fighterSpots.Last<IntVec3>();
			}
			return result;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003D50 File Offset: 0x00001F50
		private IntVec3 getCorner(bool nearest)
		{
			IntVec3 result;
			if (nearest)
			{
				result = this.fightingArea_int.ActiveCells.First<IntVec3>();
			}
			else
			{
				result = this.fightingArea_int.ActiveCells.Last<IntVec3>();
			}
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003D8C File Offset: 0x00001F8C
		private Fighter getFighter(Pawn p)
		{
			bool flag = p == this.fighter1.p;
			Fighter result;
			if (flag)
			{
				result = this.fighter1;
			}
			else
			{
				bool flag2 = p == this.fighter2.p;
				if (flag2)
				{
					result = this.fighter2;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003DD8 File Offset: 0x00001FD8
		public Pawn getPrisonerForHaul()
		{
			bool flag = !this.fighter1.isInFight;
			Pawn p;
			if (flag)
			{
				p = this.fighter1.p;
			}
			else
			{
				p = this.fighter2.p;
			}
			return p;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003E18 File Offset: 0x00002018
		public Fighter getOtherFighter(Fighter f)
		{
			bool flag = f.p == this.fighter1.p;
			Fighter result;
			if (flag)
			{
				result = this.fighter2;
			}
			else
			{
				result = this.fighter1;
			}
			return result;
		}

		// Token: 0x04000021 RID: 33
		private Area fightingArea_int;

		// Token: 0x04000022 RID: 34
		public Building_Bell.State currentState = Building_Bell.State.rest;

		// Token: 0x04000023 RID: 35
		public Fighter fighter1 = new Fighter();

		// Token: 0x04000024 RID: 36
		public Fighter fighter2 = new Fighter();

		// Token: 0x04000025 RID: 37
		private bool firstReadyDebug = false;

		// Token: 0x04000026 RID: 38
		private bool secondReadyDebug = false;

		// Token: 0x04000027 RID: 39
		protected int wickTicksLeft = 0;

		// Token: 0x04000028 RID: 40
		public bool winnerGetsFreedom = false;

		// Token: 0x04000029 RID: 41
		public bool toDeath = false;

		// Token: 0x0400002A RID: 42
		private bool destroyedFlag = false;

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
	}
}
