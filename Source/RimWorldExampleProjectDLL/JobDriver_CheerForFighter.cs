using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArenaBell
{
	// Token: 0x02000008 RID: 8
	public class JobDriver_CheerForFighter : JobDriver
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002388 File Offset: 0x00000588
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000239B File Offset: 0x0000059B
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return this.InteractToil();
			yield return new Toil
			{
				tickAction = delegate()
				{
					bool flag = ((Building_Bell)this.pawn.mindState.duty.focus.Thing).currentState == Building_Bell.State.fight;
					if (flag)
					{
						JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.None, 1f, null);
					}
					PawnUtility.GainComfortFromCellIfPossible(this.pawn);
					bool flag2 = this.pawn.IsHashIntervalTick(100);
					if (flag2)
					{
						this.pawn.jobs.CheckForJobOverride();
					}
				},
				defaultCompleteMode = ToilCompleteMode.Never,
				handlingFacing = true
			};
			yield break;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023AC File Offset: 0x000005AC
		private Toil InteractToil()
		{
			return Toils_General.Do(delegate
			{
                System.Random rnd = new System.Random();
				int rand = rnd.Next(0, 2);
				bool flag = rand == 0;
				Pawn _p;
				if (flag)
				{
					_p = ((Building_Bell)this.pawn.mindState.duty.focus.Thing).fighter1.p;
				}
				else
				{
					_p = ((Building_Bell)this.pawn.mindState.duty.focus.Thing).fighter2.p;
				}
				RenderTexture head = PortraitsCache.Get(_p, ColonistBarColonistDrawer.PawnTextureSize, default(Vector3), 1f);
				Texture2D headIcon = new Texture2D(75, 75);
				for (int i = 0; i < 75; i++)
				{
					for (int j = 0; j < 75; j++)
					{
						headIcon.SetPixel(i, j, Color.clear);
					}
				}
				RenderTexture.active = head;
				headIcon.ReadPixels(new Rect(-10f, 0f, 50f, (float)head.height), 0, 0);
				headIcon.Apply();
				Texture2D cheerIcon = InteractionDefOfArena.Cheer.Symbol;
				RenderTexture tmp = RenderTexture.GetTemporary(cheerIcon.width, cheerIcon.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
				Graphics.Blit(cheerIcon, tmp);
				RenderTexture previous = RenderTexture.active;
				RenderTexture.active = tmp;
				Texture2D _cheerIcon = new Texture2D(cheerIcon.width, cheerIcon.height);
				_cheerIcon.ReadPixels(new Rect(0f, 0f, (float)tmp.width, (float)tmp.height), 0, 0);
				_cheerIcon.Apply();
				RenderTexture.active = previous;
				RenderTexture.ReleaseTemporary(tmp);
				Texture2D merged = JobDriver_CheerForFighter.MergeTextures(_cheerIcon, headIcon);
				MoteMaker.MakeInteractionBubble(this.pawn, null, ThingDefOf.Mote_Speech, merged);
			});
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000023D0 File Offset: 0x000005D0
		public static Texture2D MergeTextures(Texture2D aBottom, Texture2D aTop)
		{
			bool flag = aBottom.width != aTop.width || aBottom.height != aTop.height;
			if (flag)
			{
				throw new InvalidOperationException("AlphaBlend only works with two equal sized images");
			}
			Color[] bData = aBottom.GetPixels();
			Color[] tData = aTop.GetPixels();
			int count = bData.Length;
			Color[] rData = new Color[count];
			for (int i = 0; i < count; i++)
			{
				Color B = bData[i];
				Color T = tData[i];
				float srcF = T.a;
				float destF = 1f - T.a;
				float alpha = srcF + destF * B.a;
				Color R = (T * srcF + B * B.a * destF) / alpha;
				R.a = alpha;
				rData[i] = R;
			}
			Texture2D res = new Texture2D(aTop.width, aTop.height);
			res.SetPixels(rData);
			res.Apply();
			return res;
		}
	}
}
