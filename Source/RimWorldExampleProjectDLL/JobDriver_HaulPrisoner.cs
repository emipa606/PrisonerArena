using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
	// Token: 0x0200000A RID: 10
	public class JobDriver_HaulPrisoner : JobDriver
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002830 File Offset: 0x00000A30
		protected Pawn Takee
		{
			get
			{
				return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002858 File Offset: 0x00000A58
		protected Building_Bell BellRef
		{
			get
			{
				return (Building_Bell)this.job.GetTarget(TargetIndex.B).Thing;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000287E File Offset: 0x00000A7E
		protected IntVec3 DropPosition
		{
			get
			{
				return (IntVec3)this.job.GetTarget(TargetIndex.C);
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002891 File Offset: 0x00000A91
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnDestroyedOrNull(TargetIndex.B);
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
			this.FailOn(delegate()
			{
				IBillGiver billGiver = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
				return this.BellRef.currentState == Building_Bell.State.rest;
			});
			base.AddFinishAction(delegate
			{
				bool flag = this.Takee == this.BellRef.fighter1.p;
				if (flag)
				{
					bool isInFight = this.BellRef.fighter1.isInFight;
					if (!isInFight)
					{
						this.BellRef.TryCancelBrawl("");
					}
				}
				else
				{
					bool flag2 = this.Takee == this.BellRef.fighter2.p;
					if (flag2)
					{
						bool isInFight2 = this.BellRef.fighter2.isInFight;
						if (!isInFight2)
						{
							this.BellRef.TryCancelBrawl("");
						}
					}
				}
			});
			yield return new Toil
			{
				initAction = delegate()
				{
					bool flag = this.BellRef.currentState == Building_Bell.State.scheduled;
					if (flag)
					{
						this.BellRef.currentState = Building_Bell.State.preparation;
						this.BellRef.startTheShow();
					}
				}
			};
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
			yield return Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.Touch);
			yield return new Toil
			{
				initAction = delegate()
				{
					IntVec3 position = this.DropPosition;
					Thing thing;
					this.pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out thing, null);
					bool flag = !this.BellRef.Destroyed;
					if (flag)
					{
						this.HaulFinished();
						this.BellRef.PrisonerDelievered(this.Takee);
						bool flag2 = this.BellRef.currentState != Building_Bell.State.fight;
						if (flag2)
						{
							Job carryPrisonerJob = new Job(JobDefOfArena.HaulingPrisoner, this.BellRef.getPrisonerForHaul(), this.BellRef, this.BellRef.getFighterStandPoint())
							{
								count = 1
							};
							this.pawn.jobs.TryTakeOrderedJob(carryPrisonerJob, JobTag.Misc);
						}
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000028A1 File Offset: 0x00000AA1
		private void HaulFinished()
		{
			this.Takee.Position = this.DropPosition;
			this.Takee.Notify_Teleported(false, true);
			this.Takee.stances.CancelBusyStanceHard();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000028D8 File Offset: 0x00000AD8
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.Takee;
			Job job = this.job;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x04000008 RID: 8
		private const TargetIndex TakeeIndex = TargetIndex.A;

		// Token: 0x04000009 RID: 9
		private const TargetIndex BellIndex = TargetIndex.B;

		// Token: 0x0400000A RID: 10
		private const TargetIndex DropIndex = TargetIndex.C;
	}
}
