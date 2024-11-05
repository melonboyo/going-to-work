using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.CommandValues;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using StorybrewScripts.Midori.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class RotatingLine : StoryboardObjectGenerator
    {
        [Configurable]
        public bool ClockWise = true;
        [Configurable]
        public bool InSteps = false;
        [Configurable]
        public int SpinSteps = 16;
        [Configurable]
        public int TotalSteps = 24;
        [Configurable]
        public int LoopOffset = 0;
        [Configurable]
        public int StartTime = 0;
        [Configurable]
        public int ObjectStartTime = 0;
        [Configurable]
        public int EndTime = 0;
        [Configurable]
        public Color4 Color = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable]
        public Color4 FrameColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable]
        public Color4 DesaturatedColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable]
        public int BeatDivisor = 8;
        [Configurable]
        public int FadeIn = 2;
        [Configurable]
        public int FadeOut = 2;
        [Configurable]
        public string SpritePath = "sb/circles/halfline.png";
        [Configurable]
        public double StartAngle = 0;
        [Configurable]
        public double SpriteScale = 1;
        [Configurable]
        public Vector2 StartPosition = new Vector2(320, 248);

        public override void Generate()
        {
            var positionOffset = new Vector2(0, 0);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }

            var frameLayer = GetLayer("LineFrame");
            var lineLayer = GetLayer("Line");

            var frameShadowSprite = frameLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameCenterSprite = frameLayer.CreateSprite("sb/circles/hitcircle.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameSprite = frameLayer.CreateAnimation("sb/circles/hitcircleoverlay.png", 3, 160, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset);

            var dSprite = lineLayer.CreateAnimation("sb/lines/clockhand.png", 3, 150, OsbLoopType.LoopForever, OsbOrigin.BottomCentre, StartPosition + positionOffset);
            var fSprite = lineLayer.CreateSprite("sb/lines/line_glow.jpg", OsbOrigin.BottomCentre, StartPosition + positionOffset);
            var cSprite = lineLayer.CreateAnimation("sb/characters/clockington_dance.png", 16, 103.86, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset);
            // var fSprite = lineLayer.CreateSprite(GlowPath, OsbOrigin.Centre, StartPosition);
            var beat = Beatmap.GetTimingPointAt((int)StartTime).BeatDuration;
            var rotationCount = (int)((EndTime - StartTime) / (beat * BeatDivisor));
            var angle = StartAngle / 180.0 * Math.PI;
            // if (StartAngle != 0.0) {
            //     angle += Math.PI * 0.006;
            // }
            // Color4 NewGray = new Color4(0.3f, 0.3f, 0.3f, 1.0f);
            var fSpriteScale = 0.65;
            var characterScale = 0.68;
            dSprite.ScaleVec(StartTime, SpriteScale, SpriteScale);
            fSprite.ScaleVec(StartTime, SpriteScale, SpriteScale * fSpriteScale);
            cSprite.ScaleVec(StartTime, 0.8*characterScale, 0.8*characterScale);

            dSprite.Fade(OsbEasing.InOutQuad, StartTime, StartTime + FadeIn * beat, 0, 1);
            fSprite.Fade(OsbEasing.InOutQuad, StartTime, StartTime + FadeIn * beat, 0, 1);
            cSprite.Fade(OsbEasing.InOutQuad, StartTime, StartTime + FadeIn * beat, 0, 1);

            frameShadowSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Color(StartTime, FrameColor);
            frameSprite.Color(StartTime, Color4.LightGray);
            frameShadowSprite.Scale(StartTime, characterScale * 1.5 * 1.2);
            frameCenterSprite.Scale(StartTime, characterScale * 1.5);
            frameSprite.Scale(StartTime, characterScale * 1.5);

            if (!ClockWise) cSprite.FlipH(StartTime);

            var turn = 2.0 * Math.PI;
            if (!ClockWise) turn = -2.0 * Math.PI;

            var loopDuration = EndTime - StartTime;

            // var loopCount = (int)((EndTime - (StartTime - LoopOffset)) / loopDuration);
  
            var spinLoopCount = (int)((EndTime - (StartTime - LoopOffset)) / loopDuration);
            if (InSteps) {
                var stepSize = 2.0f*Math.PI / ((float)SpinSteps);
                if (!ClockWise) {
                    stepSize = -stepSize;
                }
                var lastRotation = angle;
                var startTime = (double)StartTime;
                var timeStep = BeatDivisor * beat / ((float)SpinSteps);
                for (int i = 1; i <= TotalSteps; i++) {
                    var newRotation = lastRotation + stepSize;
                    dSprite.Rotate(OsbEasing.InOutQuart, startTime, startTime + timeStep, lastRotation, newRotation);
                    fSprite.Rotate(OsbEasing.InOutQuart, startTime, startTime + timeStep, lastRotation, newRotation);

                    startTime = startTime + timeStep;
                    lastRotation = newRotation;
                }
            } else {

                dSprite.StartLoopGroup(StartTime, rotationCount + 1);

                dSprite.Rotate(OsbEasing.None, 0, beat * (double)BeatDivisor, angle, angle + turn);

                dSprite.EndGroup();

                fSprite.StartLoopGroup(StartTime, rotationCount + 1);

                fSprite.Rotate(OsbEasing.None, 0, beat * (double)BeatDivisor, angle, angle + turn);

                fSprite.EndGroup();
            }

            // dSprite.Move(OsbEasing.None, StartTime, EndTime, StartPosition, EndPosition);
            dSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            // dSprite.Color(StartTime, Color);
            fSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            fSprite.Color(StartTime, Color);
            fSprite.Additive(StartTime);
            cSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);

            frameShadowSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            frameCenterSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);
            frameSprite.Fade(OsbEasing.In, EndTime - FadeOut * beat, EndTime, 1, 0);


            int clock_r = 249;
            var lastObjectTime = 999999.0;

            foreach (OsuHitObject hitobject in Beatmap.HitObjects.Reverse()) {
                if ((hitobject.EndTime + 5) < ObjectStartTime) {
                    break;
                }
                if ((hitobject.EndTime + 5) > EndTime || (int)(hitobject.Color.R * 255) != clock_r) {
                    continue;
                }
                double timeBetween = lastObjectTime - hitobject.EndTime;
                if (timeBetween > 200.0) {
                    timeBetween = 200.0;
                }
                timeBetween -= 4.0;
                // dSprite.ScaleVec(OsbEasing.In, hitobject.StartTime - timeBetween/2, hitobject.StartTime - 3, SpriteScale, SpriteScale, SpriteScale * 2.5, SpriteScale);
                // dSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime + 3, hitobject.EndTime + timeBetween/2, SpriteScale * 2.5, SpriteScale, SpriteScale, SpriteScale);
                var start = hitobject.StartTime;
                var mid1 = hitobject.StartTime + timeBetween/6;
                var mid2 = hitobject.EndTime + timeBetween/6;
                var end = hitobject.EndTime + timeBetween;
                // Log("start: " + start + ", mid: " + mid1 + ", end: " + end);
                dSprite.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032);
                fSprite.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale * fSpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032 * fSpriteScale);
                // dSprite.ScaleVec(OsbEasing.In, start, start, SpriteScale, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale);
                dSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032, SpriteScale, SpriteScale);
                fSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.032 * fSpriteScale, SpriteScale, SpriteScale * fSpriteScale);

                fSprite.Color(OsbEasing.In, start, hitobject.EndTime, DesaturatedColor, Color);
                fSprite.Color(OsbEasing.Out, hitobject.EndTime, end, Color, DesaturatedColor);

                lastObjectTime = hitobject.StartTime;
            }
        }
    }
}
