using Verse;
using Verse.AI;

namespace ArenaBell;

public class MentalState_Fighter : MentalState
{
    public Building_Bell bellRef;

    public Pawn otherPawn;

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

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref bellRef, "bellRef");
        Scribe_References.Look(ref otherPawn, "otherPawn");
    }

    public override void MentalStateTick(int delta)
    {
        var shouldStop = ShouldStop;
        if (shouldStop)
        {
            RecoverFromState();
            if (pawn.equipment?.Primary != null)
            {
                pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out _, pawn.Position);
            }

            if (!bellRef.toDeath && otherPawn.Downed || otherPawn.Dead)
            {
                bellRef.EndBrawl(pawn);
            }
        }
        else
        {
            base.MentalStateTick(delta);
        }
    }
}