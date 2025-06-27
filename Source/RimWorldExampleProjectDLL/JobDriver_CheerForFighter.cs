using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Random = System.Random;

namespace ArenaBell;

public class JobDriver_CheerForFighter : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return InteractToil();
        yield return new Toil
        {
            tickIntervalAction = delegate(int delta)
            {
                if (((Building_Bell)pawn.mindState.duty.focus.Thing).currentState ==
                    Building_Bell.State.fight)
                {
                    JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.None);
                }

                pawn.GainComfortFromCellIfPossible(delta);
                if (pawn.IsHashIntervalTick(100, delta))
                {
                    pawn.jobs.CheckForJobOverride();
                }
            },
            defaultCompleteMode = ToilCompleteMode.Never,
            handlingFacing = true
        };
    }

    private Toil InteractToil()
    {
        return Toils_General.Do(delegate
        {
            var rnd = new Random();
            var rand = rnd.Next(0, 2);
            var p = rand == 0
                ? ((Building_Bell)pawn.mindState.duty.focus.Thing).fighter1.p
                : ((Building_Bell)pawn.mindState.duty.focus.Thing).fighter2.p;
            var head = PortraitsCache.Get(p, ColonistBarColonistDrawer.PawnTextureSize, Rot4.South);
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
            var cheerIcon = InteractionDefOfArena.Cheer.GetSymbol();
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
            var merged = mergeTextures(_cheerIcon, headIcon);
            MoteMaker.MakeInteractionBubble(pawn, null, ThingDefOf.Mote_Speech, merged);
        });
    }

    private static Texture2D mergeTextures(Texture2D aBottom, Texture2D aTop)
    {
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
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