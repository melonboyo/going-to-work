using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using StorybrewScripts.Midori.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class WipingLine : StoryboardObjectGenerator
    {
        [Configurable]
        public int StartTime = 0;
        [Configurable]
        public int ObjectStartTime = 0;
        [Configurable]
        public int EndTime = 0;
        [Configurable]
        public Vector2 StartPosition = new Vector2(320, 456);
        [Configurable]
        public bool UseSecondPosition = false;
        [Configurable]
        public Vector2 SecondPosition = new Vector2(320, 24);
        [Configurable]
        public int MoveToSecondPositionTime = 0;
        [Configurable]
        public double StartAngle = -90.0;
        [Configurable]
        public double EndAngle = 90.0;
        [Configurable]
        public double SecondStartAngle = 90.0;
        [Configurable]
        public double SecondEndAngle = 270.0;
        [Configurable]
        public OsbOrigin Origin = OsbOrigin.BottomCentre;
        [Configurable]
        public int BeatDivisor = 8;
        [Configurable]
        public int FadeIn = 2;
        [Configurable]
        public int FadeOut = 2;
        [Configurable]
        public bool Top = false;
        [Configurable]
        public bool Left = false;
        [Configurable]
        public string SpritePath = "sb/circles/line.png";
        [Configurable]
        public string GlowPath = "sb/circles/line_glow.png";
        [Configurable]
        public int BeatARTime = 2;
        [Configurable]
        public int BeatARFlash = 2;
        [Configurable]
        public double SpriteScale = 1;
        [Configurable]
        public double SliderScale = 1;
        [Configurable]
        public double VectorScale = 1;
        [Configurable]
        public Color4 Color = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable]
        public Color4 FrameColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable]
        public Color4 DesaturatedColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);

        public override void Generate()
        {
            var positionOffset = new Vector2(0, 0);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }
            var cStartPosition = StartPosition + new Vector2(0f, -12f);

            var frameLayer = GetLayer("LineFrame");
            var lineLayer = GetLayer("Line");
            var cSecondPosition = SecondPosition + new Vector2(0f, -12f);

            var frameShadowSprite = frameLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameCenterSprite = frameLayer.CreateSprite("sb/circles/hitcircle.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameSprite = frameLayer.CreateAnimation("sb/circles/hitcircleoverlay.png", 3, 160, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset);

            var dSprite1 = lineLayer.CreateAnimation("sb/lines/sword.png", 10, 82.688, OsbLoopType.LoopForever, OsbOrigin.BottomCentre, StartPosition + positionOffset);
            var dSprite2 = lineLayer.CreateAnimation("sb/lines/sword.png", 10, 82.688, OsbLoopType.LoopForever, OsbOrigin.BottomCentre, StartPosition + positionOffset);
            var fSprite = lineLayer.CreateSprite("sb/lines/line_glow.jpg", OsbOrigin.Centre, StartPosition + positionOffset);
            var cSprite = frameLayer.CreateAnimation("sb/characters/wypenyo_dance.png", 8, 103.86, OsbLoopType.LoopForever, OsbOrigin.Centre, cStartPosition + positionOffset);
            
            var beat = Beatmap.GetTimingPointAt((int)StartTime).BeatDuration;
            dSprite1.ScaleVec(StartTime, SpriteScale, SpriteScale * VectorScale);
            dSprite2.ScaleVec(StartTime, SpriteScale, SpriteScale * VectorScale);
            var fSpriteScale = 2.12;
            fSprite.ScaleVec(StartTime, SpriteScale, fSpriteScale * SpriteScale * VectorScale);
            cSprite.ScaleVec(StartTime, 0.9, 0.9);

            // dSprite.StartLoopGroup(StartTime - BeatARFlash * beat, BeatARFlash);
            // dSprite.Fade(OsbEasing.In, 0, beat, 1, 0);
            // dSprite.EndGroup();

            var startAngle = StartAngle / 180.0 * Math.PI;
            var secondStartAngle = SecondStartAngle / 180.0 * Math.PI;
            var endAngle = EndAngle / 180.0 * Math.PI;
            var secondEndAngle = SecondEndAngle / 180.0 * Math.PI;

            dSprite1.Fade(OsbEasing.InQuad, StartTime, StartTime + beat * FadeIn, 0, 1);
            dSprite1.Rotate(OsbEasing.None, StartTime, StartTime, startAngle, startAngle);
            dSprite2.Fade(OsbEasing.InQuad, StartTime, StartTime + beat * FadeIn, 0, 1);
            dSprite2.Rotate(OsbEasing.None, StartTime, StartTime, startAngle + Math.PI, startAngle + Math.PI);
            fSprite.Fade(OsbEasing.InQuad, StartTime, StartTime + beat * FadeIn, 0, 1);
            fSprite.Rotate(OsbEasing.None, StartTime, StartTime, startAngle, startAngle);
            cSprite.Fade(OsbEasing.InQuad, StartTime, StartTime + beat * FadeIn, 0, 1);

            frameShadowSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Color(StartTime, FrameColor);
            frameSprite.Color(StartTime, Color4.LightGray);
            frameShadowSprite.Scale(StartTime, 1.4 * 1.2);
            frameCenterSprite.Scale(StartTime, 1.4);
            frameSprite.Scale(StartTime, 1.4);

            // dSprite2.FlipH(StartTime);

            var loopCount = (int)((EndTime - StartTime) / (BeatARTime * beat));
            var halfTime = (int)((double)beat * (double)BeatARTime * 0.5);
            var fullTime = beat * BeatARTime;

            dSprite1.StartLoopGroup(StartTime, loopCount + 1);

            dSprite1.Rotate(OsbEasing.None, 0, halfTime, startAngle, endAngle);
            dSprite1.Rotate(OsbEasing.None, halfTime, fullTime, endAngle, startAngle);

            dSprite1.EndGroup();

            fSprite.StartLoopGroup(StartTime, loopCount + 1);

            fSprite.Rotate(OsbEasing.None, 0, halfTime, startAngle, endAngle);
            fSprite.Rotate(OsbEasing.None, halfTime, fullTime, endAngle, startAngle);

            fSprite.EndGroup();

            dSprite2.StartLoopGroup(StartTime, loopCount + 1);

            dSprite2.Rotate(OsbEasing.None, 0, halfTime, startAngle + Math.PI, endAngle + Math.PI);
            dSprite2.Rotate(OsbEasing.None, halfTime, fullTime, endAngle + Math.PI, startAngle + Math.PI);

            dSprite2.EndGroup();

            if (MoveToSecondPositionTime != 0) {
                dSprite1.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, StartPosition + positionOffset, SecondPosition + positionOffset);
                dSprite2.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, StartPosition + positionOffset, SecondPosition + positionOffset);
                fSprite.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, StartPosition + positionOffset, SecondPosition + positionOffset);
                cSprite.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, cStartPosition + positionOffset, cSecondPosition + positionOffset);

                frameShadowSprite.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, cStartPosition + positionOffset, cSecondPosition + positionOffset);
                frameCenterSprite.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, cStartPosition + positionOffset, cSecondPosition + positionOffset);
                frameSprite.Move(OsbEasing.InOutQuad, MoveToSecondPositionTime - beat * 4, MoveToSecondPositionTime, cStartPosition + positionOffset, cSecondPosition + positionOffset);
            }

            // dSprite1.StartLoopGroup(MoveToSecondPositionTime - beat * 8, loopCount / 2 + 1);

            // dSprite1.Rotate(OsbEasing.None, 0, halfTime, secondStartAngle, secondEndAngle);
            // dSprite1.Rotate(OsbEasing.None, halfTime, fullTime, secondEndAngle, secondStartAngle);

            // dSprite1.EndGroup();

            dSprite1.Fade(OsbEasing.None, EndTime - FadeOut * beat, EndTime, 1, 0);
            // dSprite1.Color(StartTime, Color);
            dSprite2.Fade(OsbEasing.None, EndTime - FadeOut * beat, EndTime, 1, 0);
            cSprite.Fade(OsbEasing.None, EndTime - FadeOut * beat, EndTime, 1, 0);
            fSprite.Fade(OsbEasing.None, EndTime - FadeOut * beat, EndTime, 1, 0);
            fSprite.Color(StartTime, Color);
            fSprite.Additive(StartTime);

            frameShadowSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            frameCenterSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            frameSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);

            int wipe_r = 94;
            var lastObjectTime = 999999.0;

            foreach (OsuHitObject hitobject in Beatmap.HitObjects.Reverse()) {
                if ((hitobject.EndTime + 5) < ObjectStartTime) {
                    break;
                }
                if ((hitobject.EndTime + 5) > EndTime || (int)(hitobject.Color.R * 255) != wipe_r) {
                    continue;
                }
                double timeBetween = lastObjectTime - hitobject.EndTime;
                if (timeBetween > 200.0) {
                    timeBetween = 200.0;
                }
                timeBetween -= 4.0;
                // dSprite1.ScaleVec(OsbEasing.In, hitobject.StartTime - timeBetween/2, hitobject.StartTime - 3, SpriteScale, SpriteScale, SpriteScale * 2.5, SpriteScale);
                // dSprite1.ScaleVec(OsbEasing.Out, hitobject.EndTime + 3, hitobject.EndTime + timeBetween/2, SpriteScale * 2.5, SpriteScale, SpriteScale, SpriteScale);
                var start = hitobject.StartTime;
                var mid1 = hitobject.StartTime + timeBetween/6;
                var mid2 = hitobject.EndTime + timeBetween/6;
                var end = hitobject.EndTime + timeBetween;
                // Log("start: " + start + ", mid: " + mid1 + ", end: " + end);
                dSprite1.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032);
                dSprite2.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032);
                fSprite.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, fSpriteScale * SpriteScale, SpriteScale * Constants.LinePulseSize, fSpriteScale * SpriteScale * 1.032);
                // dSprite1.ScaleVec(OsbEasing.In, start, start, SpriteScale, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale);
                dSprite1.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032, SpriteScale, SpriteScale);
                dSprite2.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032, SpriteScale, SpriteScale);
                fSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, fSpriteScale * SpriteScale * 1.032, SpriteScale, fSpriteScale * SpriteScale);

                fSprite.Color(OsbEasing.In, start, hitobject.EndTime, DesaturatedColor, Color);
                fSprite.Color(OsbEasing.Out, hitobject.EndTime, end, Color, DesaturatedColor);

                lastObjectTime = hitobject.StartTime;
            }
        }
    }
}
