using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell;

public class LordToil_FightingMatch : LordToil
{
    public static readonly IntVec3 OtherFianceNoMarriageSpotCellOffset = new IntVec3(-1, 0, 0);

    private readonly Building_Bell bellRef;

    private IntVec3 spot;

    public LordToil_FightingMatch(IntVec3 spot, Building_Bell _bell)
    {
        this.spot = spot;
        bellRef = _bell;
    }


    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        return DutyDefOfArena.SpectateFight.hook;
    }

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

    private CellRect CalculateSpectateRect()
    {
        return CellRect.CenteredOn(bellRef.Position, Mathf.RoundToInt(bellRef.GetComp<CompBell>().radius));
    }
}