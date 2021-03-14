﻿using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000010 RID: 16
    public class JobGiver_Fighter : JobGiver_AIFightEnemy
    {
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

        // Token: 0x06000031 RID: 49 RVA: 0x00002C44 File Offset: 0x00000E44
        protected override Job TryGiveJob(Pawn pawn)
        {
            var state = (MentalState_Fighter) pawn.MentalState;
            var pawnTarget = state.otherPawn;
            var bellRef = state.bellRef;
            Job result;
            var fighterTarget = bellRef.getFighter(pawnTarget);

            if (!fighterTarget.isInFight || Rand.Value < 0.5)
            {
                result = new Job(JobDefOf.Wait_Combat)
                {
                    expiryInterval = 10
                };
            }
            else
            {
                if (pawn.TryGetAttackVerb(null) == null)
                {
                    result = null;
                }
                else
                {
                    var primary = pawn.equipment?.Primary;
                    if (bellRef.currentState == Building_Bell.State.fight && pawn.equipment != null && primary == null)
                    {
                        var suitableWeapons = new List<Thing>();
                        foreach (var c in bellRef.GetComp<CompBell>().ValidCells)
                        {
                            var thingList = c.GetThingList(bellRef.Map);
                            foreach (var thing in thingList)
                            {
                                var flag8 = thing.def.IsWeapon && !suitableWeapons.Contains(thing) && thing.Spawned &&
                                            pawn.CanReserve(thing);
                                if (flag8)
                                {
                                    suitableWeapons.Add(thing);
                                }
                            }
                        }

                        Thing weapon = null;
                        var maxDistance = 9999;
                        foreach (var t2 in suitableWeapons)
                        {
                            if ((t2.Position - pawn.Position).LengthManhattan >= maxDistance)
                            {
                                continue;
                            }

                            weapon = t2;
                            maxDistance = (t2.Position - pawn.Position).LengthManhattan;
                        }

                        if (weapon != null)
                        {
                            return new Job(JobDefOf.Equip, weapon);
                        }
                    }

                    if (pawnTarget == null || !pawn.CanReach(pawnTarget, PathEndMode.ClosestTouch, Danger.Deadly))
                    {
                        result = new Job(JobDefOf.Wait);
                    }
                    else
                    {
                        pawn.mindState.enemyTarget = pawnTarget;
                        pawn.mindState.enemyTarget = pawnTarget;
                        UpdateEnemyTarget(pawn);
                        var enemyTarget = pawn.mindState.enemyTarget;

                        if (enemyTarget == null)
                        {
                            result = null;
                        }
                        else
                        {
                            var allowManualCastWeapons = !pawn.IsColonist;
                            var verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
                            if (verb == null)
                            {
                                result = null;
                            }
                            else
                            {
                                var isMeleeAttack = verb.verbProps.IsMeleeAttack;
                                if (isMeleeAttack || pawnTarget.Downed)
                                {
                                    result = MeleeAttackJob(enemyTarget);
                                    result.killIncappedTarget = bellRef.toDeath;
                                }
                                else
                                {
                                    if (CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) >
                                        0.01f
                                        && pawn.Position.Standable(pawn.Map)
                                        && verb.CanHitTarget(enemyTarget) || (pawn.Position - enemyTarget.Position)
                                        .LengthHorizontalSquared < 25
                                        && verb.CanHitTarget(enemyTarget))
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
                                        if (!TryFindShootingPosition(pawn, out var intVec))
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
                                                    expiryInterval = ExpiryInterval_ShooterSucceeded.RandomInRange,
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
            return (Pawn) AttackTargetFinder.BestAttackTarget(pawn,
                TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, x => x is Pawn, 0f, 40f, default,
                float.MaxValue, true);
        }

        // Token: 0x06000033 RID: 51 RVA: 0x000030C8 File Offset: 0x000012C8
        public bool InDistOfRect(IntVec3 pos, IntVec3 otherLoc, CellRect rect)
        {
            var num = (float) pos.x;
            var num2 = (float) pos.z;
            return num <= rect.maxX && num >= rect.minX && num2 <= rect.maxZ && num2 >= rect.minZ;
        }

        // Token: 0x06000034 RID: 52 RVA: 0x0000311C File Offset: 0x0000131C
        protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
        {
            var enemyTarget = pawn.mindState.enemyTarget;
            var allowManualCastWeapons = !pawn.IsColonist;
            var attackVerb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            var flag = attackVerb == null;
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
                    wantCoverFromTarget = attackVerb.verbProps.range > 5.0
                }, out dest);
            }

            return result;
        }

        // Token: 0x06000035 RID: 53 RVA: 0x000031C0 File Offset: 0x000013C0
        protected override void UpdateEnemyTarget(Pawn pawn)
        {
            var state = (MentalState_Fighter) pawn.MentalState;
            var pawnTarget = state.otherPawn;
            pawn.mindState.enemyTarget = pawnTarget;
            Find.TickManager.slower.SignalForceNormalSpeed();
        }
    }
}