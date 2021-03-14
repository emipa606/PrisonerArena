using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Random = System.Random;

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
            yield return InteractToil();
            yield return new Toil
            {
                tickAction = delegate
                {
                    var flag = ((Building_Bell) pawn.mindState.duty.focus.Thing).currentState ==
                               Building_Bell.State.fight;
                    if (flag)
                    {
                        JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None);
                    }

                    pawn.GainComfortFromCellIfPossible();
                    var flag2 = pawn.IsHashIntervalTick(100);
                    if (flag2)
                    {
                        pawn.jobs.CheckForJobOverride();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never,
                handlingFacing = true
            };
        }

        // Token: 0x06000016 RID: 22 RVA: 0x000023AC File Offset: 0x000005AC
        private Toil InteractToil()
        {
            return Toils_General.Do(delegate
            {
                var rnd = new Random();
                var rand = rnd.Next(0, 2);
                var flag = rand == 0;
                var _p = flag
                    ? ((Building_Bell) pawn.mindState.duty.focus.Thing).fighter1.p
                    : ((Building_Bell) pawn.mindState.duty.focus.Thing).fighter2.p;
                var head = PortraitsCache.Get(_p, ColonistBarColonistDrawer.PawnTextureSize);
                var headIcon = new Texture2D(75, 75);
                for (var i = 0; i < 75; i++)
                {
                    for (var j = 0; j < 75; j++)
                    {
                        headIcon.SetPixel(i, j, Color.clear);
                    }
                }

                RenderTexture.active = head;
                headIcon.ReadPixels(new Rect(-10f, 0f, 50f, head.height), 0, 0);
                headIcon.Apply();
                var cheerIcon = InteractionDefOfArena.Cheer.Symbol;
                var tmp = RenderTexture.GetTemporary(cheerIcon.width, cheerIcon.height, 0, RenderTextureFormat.Default,
                    RenderTextureReadWrite.Default, 1);
                Graphics.Blit(cheerIcon, tmp);
                var previous = RenderTexture.active;
                RenderTexture.active = tmp;
                var _cheerIcon = new Texture2D(cheerIcon.width, cheerIcon.height);
                _cheerIcon.ReadPixels(new Rect(0f, 0f, tmp.width, tmp.height), 0, 0);
                _cheerIcon.Apply();
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(tmp);
                var merged = MergeTextures(_cheerIcon, headIcon);
                MoteMaker.MakeInteractionBubble(pawn, null, ThingDefOf.Mote_Speech, merged);
            });
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000023D0 File Offset: 0x000005D0
        private static Texture2D MergeTextures(Texture2D aBottom, Texture2D aTop)
        {
            var flag = aBottom.width != aTop.width || aBottom.height != aTop.height;
            if (flag)
            {
                throw new InvalidOperationException("AlphaBlend only works with two equal sized images");
            }

            var bData = aBottom.GetPixels();
            var tData = aTop.GetPixels();
            var count = bData.Length;
            var rData = new Color[count];
            for (var i = 0; i < count; i++)
            {
                var B = bData[i];
                var T = tData[i];
                var srcF = T.a;
                var destF = 1f - T.a;
                var alpha = srcF + (destF * B.a);
                var R = ((T * srcF) + (B * B.a * destF)) / alpha;
                R.a = alpha;
                rData[i] = R;
            }

            var res = new Texture2D(aTop.width, aTop.height);
            res.SetPixels(rData);
            res.Apply();
            return res;
        }
    }
}