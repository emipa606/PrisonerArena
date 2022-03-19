using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ArenaBell;

public class LordJob_Joinable_FightingMatch : LordJob_VoluntarilyJoinable
{
    private Building_Bell bell;

    private IntVec3 spot;

    public LordJob_Joinable_FightingMatch()
    {
    }

    public LordJob_Joinable_FightingMatch(IntVec3 spot, Building_Bell bell)
    {
        this.spot = spot;
        this.bell = bell;
    }

    public override bool AllowStartNewGatherings => false;

    public override StateGraph CreateGraph()
    {
        var stateGraph = new StateGraph();
        var lordToilFightingMatch = new LordToil_FightingMatch(spot, bell);
        stateGraph.AddToil(lordToilFightingMatch);
        var lordToilEnd = new LordToil_End();
        stateGraph.AddToil(lordToilEnd);
        var transition = new Transition(lordToilFightingMatch, lordToilEnd);
        transition.AddTrigger(new Trigger_TickCondition(() => bell.currentState == Building_Bell.State.rest));
        stateGraph.AddTransition(transition);
        return stateGraph;
    }

    public override float VoluntaryJoinPriorityFor(Pawn p)
    {
        var result = IsInvited(p) ? VoluntarilyJoinableLordJobJoinPriorities.SocialGathering : 0f;

        return result;
    }

    public virtual string GetReport()
    {
        return "LordReportAttendingParty".Translate();
    }

    private bool IsInvited(Pawn p)
    {
        return lord.faction != null && p.Faction == lord.faction;
    }

    public override void ExposeData()
    {
        Scribe_References.Look(ref bell, "bell");
        Scribe_Values.Look(ref spot, "spot");
    }
}