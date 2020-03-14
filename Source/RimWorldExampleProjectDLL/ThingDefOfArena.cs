using System;
using RimWorld;
using Verse;

namespace ArenaBell
{
	// Token: 0x02000006 RID: 6
	[DefOf]
	public static class ThingDefOfArena
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002307 File Offset: 0x00000507
		static ThingDefOfArena()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
		}

		// Token: 0x04000005 RID: 5
		public static ThingDef Building_ArenaBell;
	}
}
