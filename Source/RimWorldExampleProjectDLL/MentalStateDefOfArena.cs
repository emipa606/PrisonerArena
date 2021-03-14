using RimWorld;
using Verse;

namespace ArenaBell
{
    // Token: 0x0200000D RID: 13
    [DefOf]
    public static class MentalStateDefOfArena
    {
        // Token: 0x04000010 RID: 16
        public static MentalStateDef Fighter;

        // Token: 0x0600002C RID: 44 RVA: 0x00002B1E File Offset: 0x00000D1E
        static MentalStateDefOfArena()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MentalStateDefOf));
        }
    }
}