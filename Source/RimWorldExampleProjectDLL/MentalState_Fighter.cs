using System;
using Verse;
using Verse.AI;

namespace ArenaBell
{
    // Token: 0x02000014 RID: 20
    public class MentalState_Fighter : MentalState
    {
        // Token: 0x06000048 RID: 72 RVA: 0x000034CB File Offset: 0x000016CB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Building_Bell>(ref this.bellRef, "bellRef", false);
            Scribe_References.Look<Pawn>(ref this.otherPawn, "otherPawn", false);
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000049 RID: 73 RVA: 0x000034FC File Offset: 0x000016FC
        private bool ShouldStop
        {
            get
            {
                if (this.bellRef.toDeath)
                    return this.otherPawn.Dead || this.bellRef.currentState == Building_Bell.State.rest;
                return this.otherPawn.Dead || this.otherPawn.Downed || this.bellRef.currentState == Building_Bell.State.rest;
            }
        }

        // Token: 0x0600004A RID: 74 RVA: 0x0000353C File Offset: 0x0000173C
        public override void MentalStateTick()
        {
            bool shouldStop = this.ShouldStop;
            if (shouldStop)
            {
                base.RecoverFromState();
                ThingWithComps thingWithComps;
                if (this.pawn.equipment.Primary != null)
                    this.pawn.equipment.TryDropEquipment(this.pawn.equipment.Primary, out thingWithComps, this.pawn.Position, true);
                bool flag = (!this.bellRef.toDeath && this.otherPawn.Downed) || this.otherPawn.Dead;
                if (flag)
                {
                    this.bellRef.endBrawl(this.pawn, false);
                }
            }
            else
            {
                base.MentalStateTick();
            }
        }

        // Token: 0x0400001D RID: 29
        public Pawn otherPawn;

        // Token: 0x0400001E RID: 30
        public Building_Bell bellRef;
    }
}
