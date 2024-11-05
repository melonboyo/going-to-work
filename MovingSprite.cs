using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class MovingSprite : StoryboardObjectGenerator
    {
        [Group("Timing")]
        [Configurable] public int StartTime = 0;
        [Configurable] public int EndTime = 0;
        [Configurable] public int FadeInTime = 0;
        [Configurable] public int FadeOutTime = 0;
        [Configurable] public OsbEasing FadeInEasing = OsbEasing.None;
        [Configurable] public OsbEasing FadeOutEasing = OsbEasing.None;
        
        [Group("Sprite")]
        [Configurable] public bool IsAnimated = false;
        [Description("Leave empty to automatically use the map's background.")]
        [Configurable] public string SpritePath = "";
        [Configurable] public double Opacity = 1.0;
        [Configurable] public bool FillScreen = false;
        [Configurable] public bool FillScreenX = false;
        [Configurable] public bool Additive = false;
        [Configurable] public bool FlipH = false;
        [Configurable] public bool FlipV = false;


        [Group("Animation")]
        [Configurable] public double FrameDuration = 100.0;
        [Configurable] public int FrameCount = 6;
        [Configurable] public OsbLoopType LoopType = OsbLoopType.LoopForever;

        [Group("Color")]
        [Configurable] public OsbEasing ColorEasing = OsbEasing.None;
        [Configurable] public Color4 StartColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 EndColor = new Color4(255, 255, 255, 255);

        [Group("Move")]
        [Configurable] public OsbOrigin Origin = OsbOrigin.Centre;
        [Configurable] public Vector2 StartPosition = new Vector2(0.0f, 0.0f);
        [Configurable] public Vector2 EndPosition = new Vector2(0.0f, 0.0f);
        [Configurable] public OsbEasing MoveEasing = OsbEasing.None;

        [Group("Rotate")]
        [Configurable] public double StartRotation = 0.0;
        [Configurable] public double EndRotation = 0.0;
        [Configurable] public OsbEasing RotationEasing = OsbEasing.None;
        [Configurable] public bool DoOnBeatRotation = false;
        [Configurable] public int DoOnBeatRotationOffset = 0;
        [Configurable] public int BeatRotationLoopCount = 6;
        [Configurable] public Color4 BeatRotationColorOne = new Color4(0, 120, 255, 255);
        [Configurable] public Color4 BeatRotationColorTwo = new Color4(120, 255, 0, 255);

        [Group("Scale")]
        [Configurable] public Vector2 StartScale = new Vector2(1.0f, 1.0f);
        [Configurable] public Vector2 EndScale = new Vector2(1.0f, 1.0f);
        [Configurable] public OsbEasing ScaleEasing = OsbEasing.None;

        [Group("Shake")]
        [Configurable] public bool DoShake = false;
        [Configurable] public int ShakeOffset = 0;
        [Configurable] public int ShakeDuration = 200;
        [Configurable] public int ShakePeriod = 625;
        [Configurable] public int ShakeLoopCount = 7;
        [Configurable] public Vector2 ShakeSize = new Vector2(30f, 30f);

        public override void Generate()
        {
            var spritePath = SpritePath;
            if (spritePath == "") {
                IsAnimated = false;
            }
            if (IsAnimated) {
                string[] remove = { ".jpg", ".png", ".jpeg" };
                string inputText = spritePath;
                string postfix = "";

                foreach (string item in remove) {
                    if (inputText.EndsWith(item))
                    {
                        inputText = inputText.Substring(0, inputText.LastIndexOf(item));
                        postfix = item;
                        break;
                    }
                }
                spritePath = inputText + "0" + postfix;
            }
            if (spritePath == "") spritePath = Beatmap.BackgroundPath ?? string.Empty;
            if (StartTime == EndTime) EndTime = (int)(Beatmap.HitObjects.LastOrDefault()?.EndTime ?? AudioDuration);

            var startPosition = StartPosition + new Vector2(320.0f, 240.0f);
            var endPosition = EndPosition + new Vector2(320.0f, 240.0f);

            var bitmap = GetMapsetBitmap(spritePath);
            var defScale = 1.0f;
            if (FillScreen) {
                if (FillScreenX) {
                    defScale = 640.0f / bitmap.Width;
                } else {
                    defScale = 480.0f / bitmap.Height;
                }
            }

            int loopCount = 1;
            if (DoOnBeatRotation) {
                loopCount = 3;
            }

            for (int i = 0; i < loopCount; i++) {
                var sprite = GetLayer("").CreateSprite(spritePath, Origin, startPosition);

                if (IsAnimated) {
                    sprite = GetLayer("").CreateAnimation(SpritePath, FrameCount, FrameDuration, LoopType, Origin, startPosition);
                }

                if (DoOnBeatRotation && i > 0) {
                    var beatDuration = 625;
                    sprite.Additive(StartTime, EndTime);
                    var rotation = EndRotation;
                    var color = BeatRotationColorOne;
                    if (i == 2) {
                        rotation = -EndRotation;
                        color = BeatRotationColorTwo;
                    }

                    sprite.StartLoopGroup(StartTime - 1 + DoOnBeatRotationOffset, BeatRotationLoopCount);

                    // Try jittery rotation

                    sprite.Rotate(RotationEasing, beatDuration, beatDuration * 2, 0.0, rotation);
                    sprite.Color(0, color);
                    sprite.Fade(0, 0, 0, 0);
                    sprite.Fade(beatDuration, beatDuration * 2, Opacity, 0);

                    sprite.EndGroup();
                } 

                if (i == 0) {
                    var startTime = StartTime;
                    var endTime = EndTime;

                    // Normal
                    if (FadeInTime >= 0) {
                        sprite.Fade(FadeInEasing, StartTime, StartTime + FadeInTime, 0, Opacity);
                    } else {
                        sprite.Fade(FadeInEasing, StartTime + FadeInTime, StartTime, 0, Opacity);
                    }
                    if (FadeOutTime >= 0) {
                        sprite.Fade(FadeOutEasing, EndTime - FadeOutTime, EndTime, Opacity, 0);
                    } else {
                        sprite.Fade(FadeOutEasing, EndTime, EndTime - FadeOutTime, Opacity, 0);
                    }
                    sprite.Fade(FadeInEasing, StartTime + FadeInTime, StartTime, 0, Opacity);
                    sprite.Fade(FadeOutEasing, EndTime, EndTime - FadeOutTime, Opacity, 0);
                    if (Additive) sprite.Additive(StartTime, EndTime);
                    sprite.Color(ColorEasing, StartTime, EndTime, StartColor, EndColor);
                }

                if (!DoOnBeatRotation) {
                    sprite.Rotate(RotationEasing, StartTime, EndTime, StartRotation, EndRotation);
                }

                // if (StartScale.X == EndScale.X && StartScale.Y == EndScale.Y) {
                //     sprite.ScaleVec(StartTime, EndTime, defScale * StartScale);
                // } else {
                sprite.ScaleVec(ScaleEasing, StartTime, EndTime, defScale * StartScale, defScale * EndScale);
                // }

                if (DoShake) {
                    if (ShakeLoopCount == 1) {
                        sprite.Move(OsbEasing.OutQuad, StartTime + ShakeOffset, StartTime + ShakeOffset + (int)(ShakeDuration * 0.5), startPosition, startPosition + ShakeSize);
                        sprite.Move(OsbEasing.InQuad, StartTime + ShakeOffset + (int)(ShakeDuration * 0.5), StartTime + ShakeOffset + ShakeDuration, startPosition + ShakeSize, startPosition);
                    } else if (ShakeLoopCount > 1) {
                        sprite.StartLoopGroup(StartTime + ShakeOffset, ShakeLoopCount);

                        sprite.Move(OsbEasing.OutQuad, 0, (int)(ShakeDuration * 0.5), startPosition, startPosition + ShakeSize);
                        sprite.Move(OsbEasing.InQuad, (int)(ShakeDuration * 0.5), ShakeDuration, startPosition + ShakeSize, startPosition);
                        sprite.Fade(0, ShakePeriod, 1, 1);

                        sprite.EndGroup();
                    }
                } else {
                    if (startPosition != endPosition) {
                        sprite.Move(MoveEasing, StartTime, EndTime, startPosition, endPosition);
                    }
                }

                if (FlipH) {
                    sprite.FlipH(StartTime, EndTime);
                }
                if (FlipV) {
                    sprite.FlipV(StartTime, EndTime);
                }
            }
        }
    }
}
