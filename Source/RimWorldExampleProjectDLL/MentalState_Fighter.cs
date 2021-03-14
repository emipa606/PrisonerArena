using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000014 RID: 20
    public class MentalState_Fighter : MentalState
    {
        // Token: 0x0400001E RID: 30
        public Building_Bell bellRef;

        // Token: 0x0400001D RID: 29
        public Pawn otherPawn;

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000049 RID: 73 RVA: 0x000034FC File Offset: 0x000016FC
        private bool ShouldStop
        {
            get
            {
                if (bellRef.toDeath)
                {
                    return otherPawn.Dead || bellRef.currentState == Building_Bell.State.rest;
                }

                return otherPawn.Dead || otherPawn.Downed || bellRef.currentState == Building_Bell.State.rest;
            }
        }

        // Token: 0x06000048 RID: 72 RVA: 0x000034CB File Offset: 0x000016CB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref bellRef, "bellRef");
            Scribe_References.Look(ref otherPawn, "otherPawn");
        }

        // Token: 0x0600004A RID: 74 RVA: 0x0000353C File Offset: 0x0000173C
        public override void MentalStateTick()
        {
            var shouldStop = ShouldStop;
            if (shouldStop)
            {
                RecoverFromState();
                ThingWithComps thingWithComps;
                if (pawn.equipment != null && pawn.equipment.Primary != null)
                {
                    pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out thingWithComps, pawn.Position);
                }

                var flag = !bellRef.toDeath && otherPawn.Downed || otherPawn.Dead;
                if (flag)
                {
                    bellRef.endBrawl(pawn);
                }
            }
            else
            {
                base.MentalStateTick();
            }
        }
    }
}