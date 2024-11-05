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
    public class MovingLine : StoryboardObjectGenerator
    {
        [Configurable] public int StartTime = 0;
        [Configurable] public int ObjectStartTime = 0;
        [Configurable] public int EndTime = 0;
        [Configurable] public int StayTime = 0;
        [Configurable] public int FadeIn = 2;
        [Configurable] public int FadeOut = 2;
        [Configurable] public string Direction = "H";
        [Configurable] public string SpritePath = "sb/circles/line.png";
        [Configurable] public string GlowPath = "sb/circles/line_glow.png";
        [Configurable] public Vector2 StartPosition = new Vector2(0, 248);
        [Configurable] public Vector2 EndPosition = new Vector2(640, 248);
        [Configurable] public Vector2 StayPosition = new Vector2(640, 248);
        [Configurable] public float CharacterOffset = 0f;
        [Configurable] public float CharacterOffsetY = 0f;
        [Configurable] public double CharacterScale = 0.7;
        [Configurable] public int BeatARTime = 2;
        [Configurable] public double SpriteScale = 1;
        [Configurable] public double AngleDegrees = 0;
        [Configurable] public Color4 Color = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable] public Color4 FrameColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable] public Color4 DesaturatedColor = new Color4(1.0f, 0.28f, 0.3f, 1.0f);
        [Configurable] public bool Bounce = false;

        public override void Generate()
        {
            var positionOffset = new Vector2(0, 0);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }

            var lineLayer = GetLayer("Line");
            var frameLayer = GetLayer("LineFrame");
            // var cLayer = GetLayer("SectionCharacters");
            var spritePath = "sb/lines/eel.png";
            var cSpritePath = "sb/characters/horizmon_dance.png";
            var frameCount = 7;
            var cframeCount = 3;
            var frameTime = 103.86;
            var cFrameTime = 103.86;
            var characterOffset = (Math.Max(Math.Min(CharacterOffset, 1f), 0f) - 0.5f) * 2f;
            var characterOffsetVec = new Vector2(342f * characterOffset, CharacterOffsetY);
            // var characterOffsetVec2 = new Vector2(CharacterOffsetY, 240f * (2f - characterOffset));
            var characterOffsetVec2 = characterOffsetVec * -1f;
            var cExtraOffsetSize = 10f;
            var cExtraOffset = StartPosition.X > 0f ? new Vector2(0f, 1f) * cExtraOffsetSize : new Vector2(0f, 1f) * cExtraOffsetSize;
            if (Direction != "H") {
                spritePath = "sb/lines/pencil.png";
                cSpritePath = "sb/characters/vertimal_dance.png";
                cframeCount = 12;
                cFrameTime = 103.86 * 3 / 4;
                characterOffsetVec = new Vector2(CharacterOffsetY, 240f * characterOffset);
                characterOffsetVec2 = characterOffsetVec * -1f;
                cExtraOffset = StartPosition.Y > 0f ? new Vector2(1f, 0f) * cExtraOffsetSize : new Vector2(-1f, 0f) * cExtraOffsetSize;
                // cExtraOffset = new Vector2(0f, 0f);
            }

            var frameShadowSprite = frameLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameCenterSprite = frameLayer.CreateSprite("sb/circles/hitcircle.png", OsbOrigin.Centre, StartPosition + positionOffset);
            var frameSprite = frameLayer.CreateAnimation("sb/circles/hitcircleoverlay.png", 3, 160, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset);

            var dSprite = lineLayer.CreateAnimation(spritePath, frameCount, frameTime, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset);
            var fSprite = lineLayer.CreateSprite("sb/lines/line_glow.jpg", OsbOrigin.Centre, StartPosition + positionOffset);
            var cSprite = lineLayer.CreateAnimation(cSpritePath, cframeCount, cFrameTime, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset + characterOffsetVec);
            // var cSprite2 = lineLayer.CreateAnimation(cSpritePath, cframeCount, cFrameTime, OsbLoopType.LoopForever, OsbOrigin.Centre, StartPosition + positionOffset + characterOffsetVec2);
            // var cSprite = lineLayer.CreateAnimation(cSpritePath, cframeCount, cFrameTime, OsbLoopType.LoopForever, OsbOrigin.Centre, new Vector2(100f, 100f));
            var beat = Beatmap.GetTimingPointAt((int)StartTime).BeatDuration;
            var Angle = AngleDegrees / 180.0 * Math.PI;

            dSprite.ScaleVec(StartTime, SpriteScale, SpriteScale);

            dSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);

            cSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            dSprite.Rotate(OsbEasing.Out, StartTime, StartTime, Angle, Angle);

            fSprite.Additive(StartTime);
            // frameCenterSprite.Additive(StartTime);
            fSprite.ScaleVec(StartTime, SpriteScale, SpriteScale);
            fSprite.Rotate(OsbEasing.Out, StartTime, StartTime, Angle, Angle);
            fSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            fSprite.Color(StartTime, DesaturatedColor);
            // fSprite.Color(StartTime, Color4.LimeGreen);
            // fSprite.Color(StartTime, Color4.Black);

            frameShadowSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameSprite.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            frameCenterSprite.Color(StartTime, FrameColor);
            frameSprite.Color(StartTime, Color4.LightGray);
            frameShadowSprite.Scale(StartTime, 1.9 * CharacterScale * 1.2);
            frameCenterSprite.Scale(StartTime, 1.9 * CharacterScale);
            frameSprite.Scale(StartTime, 1.9 * CharacterScale);

            cSprite.ScaleVec(StartTime, CharacterScale, CharacterScale);

            var loopCount = (int)((EndTime - StartTime) / (BeatARTime * beat)) + 1;
            var halfTime = (int)((double)beat * (double)BeatARTime * 0.5);
            var fullTime = beat * BeatARTime;

            var easingA = OsbEasing.None;
            var easingB = OsbEasing.None;
            if (Bounce) {
                easingA = OsbEasing.OutSine;
                easingB = OsbEasing.InSine;
            }

            var endTime = EndTime;
            if (StayTime > 0) {
                endTime = StayTime;
                loopCount -= 1;
            }

            dSprite.StartLoopGroup(StartTime, loopCount);

            // if (Direction != "H") {
            //     dSprite.FlipH(0, halfTime);
            // } else {
            //     dSprite.FlipH(0, halfTime);
            // }
            dSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset, EndPosition + positionOffset);
            dSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset, StartPosition + positionOffset);

            dSprite.EndGroup();

            fSprite.StartLoopGroup(StartTime, loopCount);

            fSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset, EndPosition + positionOffset);
            fSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset, StartPosition + positionOffset);

            fSprite.EndGroup();

            cSprite.StartLoopGroup(StartTime, loopCount);

            cSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset + characterOffsetVec + cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
            cSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StartPosition + positionOffset + characterOffsetVec + cExtraOffset);

            cSprite.EndGroup();

            frameShadowSprite.StartLoopGroup(StartTime, loopCount);

            frameShadowSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset + characterOffsetVec + cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
            frameShadowSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StartPosition + positionOffset + characterOffsetVec + cExtraOffset);

            frameShadowSprite.EndGroup();

            frameCenterSprite.StartLoopGroup(StartTime, loopCount);

            frameCenterSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset + characterOffsetVec + cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
            frameCenterSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StartPosition + positionOffset + characterOffsetVec + cExtraOffset);

            frameCenterSprite.EndGroup();

            frameSprite.StartLoopGroup(StartTime, loopCount);

            frameSprite.Move(easingA, 0, halfTime, StartPosition + positionOffset + characterOffsetVec + cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
            frameSprite.Move(easingB, halfTime, fullTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StartPosition + positionOffset + characterOffsetVec + cExtraOffset);

            frameSprite.EndGroup();

            if (StayTime > 0) {
                var loopEndTime = StartTime + fullTime * loopCount;
                dSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset, EndPosition + positionOffset);
                dSprite.Move(easingB, loopEndTime + halfTime, EndTime, EndPosition + positionOffset, StayPosition + positionOffset);
                fSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset, EndPosition + positionOffset);
                fSprite.Move(easingB, loopEndTime + halfTime, EndTime, EndPosition + positionOffset, StayPosition + positionOffset);
                cSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset + characterOffsetVec - cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
                cSprite.Move(OsbEasing.InBack, loopEndTime + halfTime, EndTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StayPosition + positionOffset);
                
                frameShadowSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset + characterOffsetVec - cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
                frameShadowSprite.Move(OsbEasing.InBack, loopEndTime + halfTime, EndTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StayPosition + positionOffset);
                frameCenterSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset + characterOffsetVec - cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
                frameCenterSprite.Move(OsbEasing.InBack, loopEndTime + halfTime, EndTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StayPosition + positionOffset);
                frameSprite.Move(easingB, loopEndTime, loopEndTime + halfTime, StartPosition + positionOffset + characterOffsetVec - cExtraOffset, EndPosition + positionOffset + characterOffsetVec - cExtraOffset);
                frameSprite.Move(OsbEasing.InBack, loopEndTime + halfTime, EndTime, EndPosition + positionOffset + characterOffsetVec - cExtraOffset, StayPosition + positionOffset);
            }
            dSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            fSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            cSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);

            frameShadowSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            frameCenterSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            frameSprite.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            // dSprite.Color(StartTime, Color);

            // if (Direction == "V") {
            //     cSprite2.Fade(OsbEasing.InQuint, StartTime, StartTime + FadeIn * beat, 0, 1);
            //     cSprite2.FlipH(StartTime);
            //     cSprite2.ScaleVec(StartTime, CharacterScale, CharacterScale);
            //     cSprite2.StartLoopGroup(StartTime, loopCount);
            //     cSprite2.Move(easingA, 0, halfTime, StartPosition + positionOffset + characterOffsetVec2 + cExtraOffset, EndPosition + positionOffset + characterOffsetVec2 - cExtraOffset);
            //     cSprite2.Move(easingB, halfTime, fullTime, EndPosition + positionOffset + characterOffsetVec2 - cExtraOffset, StartPosition + positionOffset + characterOffsetVec2 + cExtraOffset);
            //     cSprite2.EndGroup();
            //     cSprite2.Fade(OsbEasing.In, endTime - FadeOut * beat, endTime, 1, 0);
            // }

            int hori_r = 148;
            int vert_r = 253;

            int check_color = (Direction == "H") ? hori_r : vert_r;
            var lastObjectTime = 999999.0;

            foreach (OsuHitObject hitobject in Beatmap.HitObjects.Reverse()) {
                if ((hitobject.EndTime + 5) < ObjectStartTime) {
                    break;
                }
                if ((hitobject.EndTime + 5) > EndTime || (int)(hitobject.Color.R * 255) != check_color) {
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
                dSprite.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.06);
                fSprite.ScaleVec(OsbEasing.In, start, hitobject.EndTime, SpriteScale * Constants.LinePulseSize, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.06);
                // dSprite.ScaleVec(OsbEasing.In, start, start, SpriteScale, SpriteScale, SpriteScale * Constants.LinePulseSize, SpriteScale);
                dSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.06, SpriteScale, SpriteScale);
                fSprite.ScaleVec(OsbEasing.Out, hitobject.EndTime, end, SpriteScale * Constants.LinePulseSize, SpriteScale * 1.06, SpriteScale, SpriteScale);

                fSprite.Color(OsbEasing.In, start, hitobject.EndTime, DesaturatedColor, Color);
                fSprite.Color(OsbEasing.Out, hitobject.EndTime, end, Color, DesaturatedColor);

                lastObjectTime = hitobject.StartTime;
            }
        }
    }
}
