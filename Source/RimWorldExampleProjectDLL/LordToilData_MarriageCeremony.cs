using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ArenaBell
{
    // Token: 0x02000011 RID: 17
    public class LordToilData_MarriageCeremony : LordToilData
    {
        // Token: 0x04000017 RID: 23
        public CellRect spectateRect;

        // Token: 0x04000016 RID: 22
        public SpectateRectSide spectateRectAllowedSides = SpectateRectSide.All;

        // Token: 0x06000037 RID: 55 RVA: 0x00003208 File Offset: 0x00001408
        public override void ExposeData()
        {
            Scribe_Values.Look(ref spectateRect, "spectateRect");
            Scribe_Values.Look(ref spectateRectAllowedSides, "spectateRectAllowedSides");
        }
    }
}