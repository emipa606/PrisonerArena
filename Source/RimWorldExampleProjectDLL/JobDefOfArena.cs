using System;
using RimWorld;
using Verse;

namespace ArenaBell
{
	// Token: 0x0200000B RID: 11
	[DefOf]
	public static class JobDefOfArena
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002AF8 File Offset: 0x00000CF8
		static JobDefOfArena()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
		}

		// Token: 0x0400000B RID: 11
		public static JobDef HaulingPrisoner;

		// Token: 0x0400000C RID: 12
		public static JobDef SpectateFightingMatch;

		// Token: 0x0400000D RID: 13
		public static JobDef Cheer;
	}
}
