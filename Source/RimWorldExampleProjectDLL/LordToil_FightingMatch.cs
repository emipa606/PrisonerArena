using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell
{
    // Token: 0x02000012 RID: 18
    public class LordToil_FightingMatch : LordToil
    {
        // Token: 0x04000018 RID: 24
        public static readonly IntVec3 OtherFianceNoMarriageSpotCellOffset = new(-1, 0, 0);

        // Token: 0x0400001A RID: 26
        private readonly Building_Bell bellRef;

        // Token: 0x04000019 RID: 25
        private IntVec3 spot;

        // Token: 0x06000039 RID: 57 RVA: 0x00003255 File Offset: 0x00001455
        public LordToil_FightingMatch(IntVec3 spot, Building_Bell _bell)
        {
            this.spot = spot;
            bellRef = _bell;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x0000326D File Offset: 0x0000146D

        // Token: 0x0600003B RID: 59 RVA: 0x00003278 File Offset: 0x00001478
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            return DutyDefOfArena.SpectateFight.hook;
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00003294 File Offset: 0x00001494
        public override void UpdateAllDuties()
        {
            foreach (var ownedPawn in lord.ownedPawns)
            {
                ownedPawn.mindState.duty = new PawnDuty(DutyDefOfArena.SpectateFight)
                {
                    spectateRect = CalculateSpectateRect(),
                    focus = bellRef
                };
            }
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00003308 File Offset: 0x00001508
        private CellRect CalculateSpectateRect()
        {
            return CellRect.CenteredOn(bellRef.Position, Mathf.RoundToInt(bellRef.GetComp<CompBell>().radius));
        }
    }
}