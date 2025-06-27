using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell;

public class LordToil_FightingMatch(IntVec3 spot, Building_Bell _bell) : LordToil
{
    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        return DutyDefOfArena.SpectateFight.hook;
    }

    public override void UpdateAllDuties()
    {
        foreach (var ownedPawn in lord.ownedPawns)
        {
            var duty = new PawnDuty(DutyDefOfArena.SpectateFight)
            {
                spectateRect = calculateSpectateRect(),
                focus = _bell
            };
            var bellComp = _bell.GetComp<CompBell>();
            if (bellComp.useCircle)
            {
                duty.spectateDistance = new IntRange(
                    (int)(bellComp.radius + CompBell.circleAddition + 1),
                    (int)(bellComp.radius + CompBell.circleAddition + bellComp.audience + 1));
            }

            ownedPawn.mindState.duty = duty;
        }
    }

    private CellRect calculateSpectateRect()
    {
        return CellRect.CenteredOn(_bell.Position, Mathf.RoundToInt(_bell.GetComp<CompBell>().radius));
    }
}