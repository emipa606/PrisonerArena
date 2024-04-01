using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArenaBell;

public class LordToil_FightingMatch(IntVec3 spot, Building_Bell _bell) : LordToil
{
    public static readonly IntVec3 OtherFianceNoMarriageSpotCellOffset = new IntVec3(-1, 0, 0);

    private IntVec3 spot = spot;


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
                spectateRect = CalculateSpectateRect(),
                focus = _bell
            };

            if (_bell.GetComp<CompBell>().useCircle)
            {
                duty.spectateDistance = new IntRange((int)_bell.GetComp<CompBell>().radius + 2,
                    (int)(_bell.GetComp<CompBell>().radius + 3));
            }

            ownedPawn.mindState.duty = duty;
        }
    }

    private CellRect CalculateSpectateRect()
    {
        return CellRect.CenteredOn(_bell.Position, Mathf.RoundToInt(_bell.GetComp<CompBell>().radius));
    }
}