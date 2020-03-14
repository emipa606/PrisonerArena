using System;
using RimWorld;

namespace ArenaBell
{
	// Token: 0x0200000C RID: 12
	[DefOf]
	public static class ThoughtDefOfArena
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002B0B File Offset: 0x00000D0B
		static ThoughtDefOfArena()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
		}

		// Token: 0x0400000E RID: 14
		public static ThoughtDef ArenaWinner;

		// Token: 0x0400000F RID: 15
		public static ThoughtDef ArenaLoser;
	}
}
