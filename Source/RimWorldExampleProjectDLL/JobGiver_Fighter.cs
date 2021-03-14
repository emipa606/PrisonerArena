using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
	// Token: 0x02000010 RID: 16
	public class JobGiver_Fighter : JobGiver_AIFightEnemy
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00002C44 File Offset: 0x00000E44
		protected override Job TryGiveJob(Pawn pawn)
		{
			MentalState_Fighter state = (MentalState_Fighter)pawn.MentalState;
			Pawn pawnTarget = state.otherPawn;
			Building_Bell bellRef = state.bellRef;
			Job result;
			Fighter fighterTarget = bellRef.getFighter(pawnTarget);

			if (!fighterTarget.isInFight || (double)Rand.Value < 0.5)
			{
				result = new Job(JobDefOf.Wait_Combat)
				{
					expiryInterval = 10
				};
			}
			else
			{
				if (pawn.TryGetAttackVerb(null, false) == null)
				{
					result = null;
				}
				else
				{
					ThingWithComps primary = pawn.equipment != null ? pawn.equipment.Primary : null;
					if (bellRef.currentState == Building_Bell.State.fight && pawn.equipment != null && primary == null)
					{
						List<Thing> suitableWeapons = new List<Thing>();
						foreach (IntVec3 c in bellRef.GetComp<CompBell>().ValidCells)
						{
							List<Thing> thingList = c.GetThingList(bellRef.Map);
							for (int i = 0; i < thingList.Count; i++)
							{
								Thing t = thingList[i];
								bool flag8 = t.def.IsWeapon && !suitableWeapons.Contains(t) && t.Spawned && pawn.CanReserve(t, 1, -1, null, false);
								if (flag8)
								{
									suitableWeapons.Add(t);
								}
							}
						}
						Thing weapon = null;
						int maxDistance = 9999;
						foreach (Thing t2 in suitableWeapons)
						{
							if ((t2.Position - pawn.Position).LengthManhattan < maxDistance)
							{
								weapon = t2;
								maxDistance = (t2.Position - pawn.Position).LengthManhattan;
							}
						}
						if (weapon != null)
						{
							return new Job(JobDefOf.Equip, weapon);
						}
					}
					if (pawnTarget == null || !pawn.CanReach(pawnTarget, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						result = new Job(JobDefOf.Wait);
					}
					else
					{
						pawn.mindState.enemyTarget = pawnTarget;
						pawn.mindState.enemyTarget = pawnTarget;
						this.UpdateEnemyTarget(pawn);
						Thing enemyTarget = pawn.mindState.enemyTarget;

						if (enemyTarget == null)
						{
							result = null;
						}
						else
						{
							bool allowManualCastWeapons = !pawn.IsColonist;
							Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
							if (verb == null)
							{
								result = null;
							}
							else
							{
								bool isMeleeAttack = verb.verbProps.IsMeleeAttack;
								if (isMeleeAttack || pawnTarget.Downed)
								{
									result = this.MeleeAttackJob(enemyTarget);
                                    result.killIncappedTarget = bellRef.toDeath;
								}
								else
								{
									if ((
											CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) > 0.01f
											&& pawn.Position.Standable(pawn.Map)
											&& verb.CanHitTarget(enemyTarget)
										) || (
											(pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25
											&& verb.CanHitTarget(enemyTarget)
										))
									{
										result = new Job(JobDefOf.AttackStatic, pawnTarget)
										{
											maxNumMeleeAttacks = 1,
											expiryInterval = Rand.Range(420, 900),
											canBash = true
										};
									}
									else
									{
										IntVec3 intVec;
										if (!this.TryFindShootingPosition(pawn, out intVec))
										{
											result = null;
										}
										else
										{
											if (intVec == pawn.Position)
											{
												result = new Job(JobDefOf.AttackStatic, pawnTarget)
												{
													maxNumMeleeAttacks = 1,
													expiryInterval = Rand.Range(420, 900),
													canBash = true
												};
											}
											else
											{
												result = new Job(JobDefOf.Goto, intVec)
												{
													expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
													checkOverrideOnExpire = true
												};
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003070 File Offset: 0x00001270
		private Pawn FindPawnTarget(Pawn pawn)
		{
			return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, (Thing x) => x is Pawn, 0f, 40f, default(IntVec3), float.MaxValue, true, true);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000030C8 File Offset: 0x000012C8
		public bool InDistOfRect(IntVec3 pos, IntVec3 otherLoc, CellRect rect)
		{
			float num = (float)pos.x;
			float num2 = (float)pos.z;
			return num <= (float)rect.maxX && num >= (float)rect.minX && num2 <= (float)rect.maxZ && num2 >= (float)rect.minZ;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000311C File Offset: 0x0000131C
		protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Thing enemyTarget = pawn.mindState.enemyTarget;
			bool allowManualCastWeapons = !pawn.IsColonist;
			Verb attackVerb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
			bool flag = attackVerb == null;
			bool result;
			if (flag)
			{
				dest = IntVec3.Invalid;
				result = false;
			}
			else
			{
				result = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
				{
					caster = pawn,
					target = enemyTarget,
					verb = attackVerb,
					maxRangeFromTarget = attackVerb.verbProps.range,
					wantCoverFromTarget = ((double)attackVerb.verbProps.range > 5.0)
				}, out dest);
			}
			return result;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000031C0 File Offset: 0x000013C0
		protected override void UpdateEnemyTarget(Pawn pawn)
		{
			MentalState_Fighter state = (MentalState_Fighter)pawn.MentalState;
			Pawn pawnTarget = state.otherPawn;
			pawn.mindState.enemyTarget = pawnTarget;
			Find.TickManager.slower.SignalForceNormalSpeed();
		}

		// Token: 0x04000011 RID: 17
		private const float MaxAttackDistance = 900f;

		// Token: 0x04000012 RID: 18
		private const float WaitChance = 0.5f;

		// Token: 0x04000013 RID: 19
		private const int WaitTicks = 1;

		// Token: 0x04000014 RID: 20
		private const int MinMeleeChaseTicks = 420;

		// Token: 0x04000015 RID: 21
		private const int MaxMeleeChaseTicks = 900;
	}
}
