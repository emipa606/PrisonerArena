using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
	// Token: 0x02000009 RID: 9
	public class JobDriver_SpectateMatch : JobDriver
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002740 File Offset: 0x00000940
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.job.GetTarget(TargetIndex.A);
			Job job = this.job;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002779 File Offset: 0x00000979
		protected override IEnumerable<Toil> MakeNewToils()
		{
			bool haveChair = this.job.GetTarget(TargetIndex.A).HasThing;
			bool flag = haveChair;
			if (flag)
			{
				this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
			}
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			yield return new Toil
			{
				tickAction = delegate()
				{
					bool flag2 = ((Building_Bell)this.pawn.mindState.duty.focus.Thing).currentState == Building_Bell.State.fight;
					if (flag2)
					{
						JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.None, 1f, null);
					}
					this.pawn.rotationTracker.FaceCell(this.job.GetTarget(TargetIndex.B).Cell);
					PawnUtility.GainComfortFromCellIfPossible(this.pawn);
					bool flag3 = this.pawn.IsHashIntervalTick(100);
					if (flag3)
					{
						this.pawn.jobs.CheckForJobOverride();
					}
				},
				defaultCompleteMode = ToilCompleteMode.Never,
				handlingFacing = true
			};
			yield break;
		}

		// Token: 0x04000006 RID: 6
		private const TargetIndex MySpotOrChairInd = TargetIndex.A;

		// Token: 0x04000007 RID: 7
		private const TargetIndex WatchTargetInd = TargetIndex.B;
	}
}
