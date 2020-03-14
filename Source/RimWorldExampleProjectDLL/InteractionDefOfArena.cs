using System;
using RimWorld;

namespace ArenaBell
{
	// Token: 0x02000005 RID: 5
	[DefOf]
	public static class InteractionDefOfArena
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000022F4 File Offset: 0x000004F4
		static InteractionDefOfArena()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(InteractionDefOf));
		}

		// Token: 0x04000004 RID: 4
		public static InteractionDef Cheer;
	}
}
