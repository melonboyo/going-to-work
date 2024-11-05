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
using System.Drawing;
using System.Text;
using System.IO;
using StorybrewCommon.Storyboarding.CommandValues;
using StorybrewScripts.Midori.Constants;
using System.Runtime.InteropServices;

namespace StorybrewScripts
{
    public class FakeGameplay : StoryboardObjectGenerator
    {
        public enum LineType {
            Horizontal = 0,
            Vertical = 1,
            Clockwise = 2,
            CClockwise = 3,
            WipeUp = 4,
            WipeDown = 5,
            WipeLeft = 6,
            WipeRight = 7
        }

        [Configurable] public bool ShowSectionHint = true;

        [Group("Sprite")]
        [Configurable] public string HitcirclePath = "sb/circles/hitcircle.png";
        [Configurable] public string HitcircleOverlayPath = "sb/circles/hitcircleoverlay.png";
        [Configurable] public string ArrowsOverlayPath = "sb/circles/arrows.png";
        [Configurable] public string SliderBody = "sb/circles/sliderbody.png";
        [Configurable] public string SliderEdge = "sb/circles/slideredge.png";

        [Group("Animation")]
        [Configurable] public double FrameDuration = 100.0;
        [Configurable] public int FrameCount = 6;
        [Configurable] public OsbLoopType LoopType = OsbLoopType.LoopForever;

        [Group("Timing")]
        [Configurable] public int StartTime = 0;
        [Configurable] public int EndTime = 0;
        [Configurable] public int BeatDivisor = 8;
        [Configurable] public int FadeTime = 200;
        [Configurable] public int ARTime = 600;
        [Configurable] public double SpriteScale = 1;
        [Configurable] public double HitFadeScale = 1.2;
        [Configurable] public double InactiveScaleRatio = 0.67;
        [Configurable] public double SliderScale = 1;
        [Configurable] public int HitCircleGap = 0;
        [Configurable] public int ActivateTransitionTime = 200;
        [Configurable] public double ActivateTimingRatioStart2 = 0.35;
        [Configurable] public double ActivateTimingRatioTransition2 = 0.38;
        [Configurable] public double ActivateTimingRatioStart4 = 0.35;
        [Configurable] public double ActivateTimingRatioTransition4 = 0.38;
        [Configurable] public double ActivateTimingRatioStart8 = 0.35;
        [Configurable] public double ActivateTimingRatioTransition8 = 0.38;
        [Configurable] public double ActivateTimingRatioStart16 = 0.35;
        [Configurable] public double ActivateTimingRatioTransition16 = 0.38;

        [Group("Colors")]
        [Configurable] public Color4 ClockColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 ClockColor2 = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 ClockDsColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 HoriColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 HoriColor2 = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 HoriDsColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 VertColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 VertColor2 = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 VertDsColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 WipeColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 WipeColor2 = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 WipeDsColor = new Color4(255, 255, 255, 255);

        public string[] num = new string[10];

        public class Section
        {
            public int startTime { get; set; }
            public int endTime { get; set;}
            public LineType type { get; set; }
            public int beatCount { get; set; }
            public float offset { get; set; }
            public bool flipEven { get; set; }
        }

        public List<Section> getSections()
        {
            List<Section> sections = new List<Section>
            {
                new Section { startTime = 3581,     endTime = 18126,    type = LineType.Clockwise,  beatCount = 8, offset = 0f, flipEven = false },
                new Section { startTime = 18126,    endTime = 29762,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 29762,    endTime = 34490,    type = LineType.WipeDown,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 35581,    endTime = 41399,    type = LineType.WipeUp,     beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 41399,    endTime = 44308,    type = LineType.CClockwise,  beatCount = 4, offset = 0.5f, flipEven = false },
                new Section { startTime = 44308,    endTime = 47217,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 47217,    endTime = 48672,    type = LineType.CClockwise,  beatCount = 4, offset = 0.5f, flipEven = false },
                new Section { startTime = 48672,    endTime = 50368,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 50368,    endTime = 51581,    type = LineType.CClockwise,  beatCount = 4, offset = 0.5f, flipEven = false },
                new Section { startTime = 51581,    endTime = 54853,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 54853,    endTime = 56308,    type = LineType.CClockwise,  beatCount = 4, offset = 0.5f, flipEven = false },
                new Section { startTime = 56308,    endTime = 58853,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 58853,    endTime = 60308,    type = LineType.WipeLeft,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 60308,    endTime = 61339,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 61339,    endTime = 63217,    type = LineType.WipeLeft,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 63217,    endTime = 64671,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 64671,    endTime = 67581,    type = LineType.Clockwise,   beatCount = 2, offset = 0f, flipEven = false },
                new Section { startTime = 67581,    endTime = 69884,    type = LineType.Vertical,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 69884,    endTime = 82126,    type = LineType.Horizontal, beatCount = 2, offset = 0f, flipEven = false },
                new Section { startTime = 82126,    endTime = 82793,    type = LineType.WipeUp,     beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 82793,    endTime = 82914,    type = LineType.Clockwise,  beatCount = 4, offset = 0.125f, flipEven = false },
                new Section { startTime = 82914,    endTime = 84247,    type = LineType.WipeUp,     beatCount = 4, offset = 0f, flipEven = true },
                new Section { startTime = 84247,    endTime = 84368,    type = LineType.Clockwise,  beatCount = 4, offset = -0.125f, flipEven = false },
                new Section { startTime = 84368,    endTime = 85702,    type = LineType.WipeUp,     beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 85702,    endTime = 85884,    type = LineType.Clockwise,  beatCount = 4, offset = 0.125f, flipEven = false },
                new Section { startTime = 85884,    endTime = 86793,    type = LineType.WipeUp,     beatCount = 4, offset = 0f, flipEven = true },
                new Section { startTime = 86793,    endTime = 87156,    type = LineType.Clockwise,  beatCount = 4, offset = 0.5f+0.0625f, flipEven = false },
                new Section { startTime = 87156,    endTime = 87520,    type = LineType.Clockwise,  beatCount = 4, offset = 0.5f-0.125f, flipEven = false },
                new Section { startTime = 87520,    endTime = 87944,    type = LineType.Clockwise,  beatCount = 4, offset = 0.25f-0.0625f, flipEven = false },
                new Section { startTime = 87944,    endTime = 88611,    type = LineType.WipeDown,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 88611,    endTime = 88732,    type = LineType.Clockwise,  beatCount = 4, offset = 0.125f, flipEven = false },
                new Section { startTime = 88732,    endTime = 90065,    type = LineType.WipeDown,   beatCount = 4, offset = 0f, flipEven = true },
                new Section { startTime = 90065,    endTime = 90187,    type = LineType.Clockwise,  beatCount = 4, offset = -0.125f, flipEven = false },
                new Section { startTime = 90187,    endTime = 91520,    type = LineType.WipeDown,   beatCount = 4, offset = 0f, flipEven = false },
                new Section { startTime = 91520,    endTime = 91641,    type = LineType.Clockwise,  beatCount = 4, offset = 0.125f, flipEven = false },
                new Section { startTime = 91641,    endTime = 92611,    type = LineType.WipeDown,   beatCount = 4, offset = 0f, flipEven = true },
                new Section { startTime = 92611,    endTime = 92975,    type = LineType.Clockwise,  beatCount = 4, offset = 0.5f+0.0625f, flipEven = false },
                new Section { startTime = 92975,    endTime = 93338,    type = LineType.Clockwise,  beatCount = 4, offset = 0.5f-0.125f, flipEven = false },
                new Section { startTime = 93338,    endTime = 93762,    type = LineType.Clockwise,  beatCount = 4, offset = 0.25f-0.0625f, flipEven = false },
                new Section { startTime = 93762,    endTime = 95217,   type = LineType.Horizontal,   beatCount = 2, offset = 0.5f, flipEven = false },
                new Section { startTime = 95217,   endTime = 96671,   type = LineType.Vertical, beatCount = 4, offset = 0.25f, flipEven = false },
                new Section { startTime = 96671,    endTime = 98005,   type = LineType.Horizontal,   beatCount = 2, offset = 0.5f, flipEven = false },
                new Section { startTime = 98005,    endTime = 99217,   type = LineType.Vertical,   beatCount = 4, offset = 0.25f, flipEven = false },
                new Section { startTime = 99217,   endTime = 100671,   type = LineType.Horizontal, beatCount = 2, offset = 0.5f, flipEven = false },
                new Section { startTime = 100671,    endTime = 101884,   type = LineType.Vertical,   beatCount = 4, offset = 0.25f, flipEven = false },
                new Section { startTime = 101884,   endTime = 102732,   type = LineType.Horizontal, beatCount = 2, offset = 0.5f, flipEven = false },
                new Section { startTime = 102732,    endTime = 103581,   type = LineType.Vertical,   beatCount = 4, offset = 0.25f, flipEven = false },
                new Section { startTime = 103581,   endTime = 105399,   type = LineType.Horizontal, beatCount = 2, offset = 0.5f, flipEven = false },
                new Section { startTime = 105399,    endTime = 108065,   type = LineType.Vertical,   beatCount = 4, offset = 0.25f, flipEven = false },
                new Section { startTime = 108065,   endTime = 118611,   type = LineType.CClockwise, beatCount = 8, offset = 0f, flipEven = false },
                new Section { startTime = 118611,   endTime = 125762,   type = LineType.Vertical, beatCount = 2, offset = 0f, flipEven = false },
                new Section { startTime = 125762,   endTime = 128671,   type = LineType.WipeUp, beatCount = 2, offset = 0f, flipEven = true },
                new Section { startTime = 128671,   endTime = 130853,   type = LineType.Clockwise, beatCount = 2, offset = 0.25f, flipEven = false },
                new Section { startTime = 130853,   endTime = 131581,   type = LineType.Vertical, beatCount = 2, offset = 0f, flipEven = false },
                new Section { startTime = 131581,   endTime = 138035,   type = LineType.Horizontal, beatCount = 2, offset = 0f, flipEven = false },
            };
            return sections;
        }

        public override void Generate()
        {
            // for (int i = 0; i < 10; i++)
            // {
            //     num[i] = ("sb/circles/default-" + i.ToString() + ".png");
            // }
            var hitobjectLayer = GetLayer("HitObjects");
            var hintLayer = GetLayer("Hint");
            // var sliderLayer = GetLayer("HitObjects");
            // var characterLayer = GetLayer("Characters");

            var beat = Beatmap.GetTimingPointAt((int)StartTime).BeatDuration;
            List<Section> sections = getSections();

            // var hori_r = 94;
            // var clock_r = 249;
            // var vert_r = 253;
            // var wipe_r = 148;

            List<List<OsuHitObject>> sectionObjects = new List<List<OsuHitObject>>{ new List<OsuHitObject>() };

            var sId = 0;
            Section currentSection = sections[sId];
            foreach (OsuHitObject hitobject in Beatmap.HitObjects) {
                if (hitobject.StartTime >= currentSection.endTime - 5) {
                    sId += 1;
                    sectionObjects.Add(new List<OsuHitObject>());
                }
                currentSection = sections[sId];
                if (hitobject.StartTime > currentSection.startTime - 5 && hitobject.EndTime < currentSection.endTime - 5) {
                    sectionObjects[sId].Add(hitobject);
                }
            }

            var songStartTimes = new List<int> {
                119944,
                108308,
                93762,
                70490,
                55944,
                44308,
                18126,
                671,
            };

            var positionOffset = new Vector2(0, 0);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }

            var sI = 0;
            var songStartTime = songStartTimes[sI];

            var hintShadowSprite = hintLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, new Vector2(-80, 460));
            var hintSprite = hintLayer.CreateSprite("sb/circles/sliderbody.png", OsbOrigin.Centre, new Vector2(-80, 460));
            hintSprite.Fade(sections[0].startTime - 40, sections[0].startTime, 0, 0.8);
            hintShadowSprite.Fade(sections[0].startTime - 40, sections[0].startTime, 0, 0.8);
            hintSprite.Fade(sections[sections.Count - 1].endTime, sections[sections.Count - 1].endTime + 200, 0.8, 0);
            hintShadowSprite.Fade(sections[sections.Count - 1].endTime, sections[sections.Count - 1].endTime + 200, 0.8, 0);
            hintSprite.Scale(sections[0].startTime, 0.2);
            hintShadowSprite.Scale(sections[0].startTime, 0.21);
            Color4 prevColor = ClockColor;

            for(int i = sectionObjects.Count - 1; i >= 0; i--)
            {
                var section = sections[i];
                var objects = sectionObjects[i];
                var groupTime = beat * section.beatCount;
                var evenGroup = false;
                var prevSnappedTime = -1.0;

                Color4 ComboColor = new Color4();
                Color4 DesaturatedComboColor = new Color4();
                if (section.type == LineType.Clockwise || section.type == LineType.CClockwise) {
                    ComboColor = ClockColor;
                    DesaturatedComboColor = ClockDsColor;
                } else if (section.type == LineType.Horizontal) {
                    ComboColor = HoriColor;
                    DesaturatedComboColor = HoriDsColor;
                } else if (section.type == LineType.Vertical) {
                    ComboColor = VertColor;
                    DesaturatedComboColor = VertDsColor;
                } else {
                    ComboColor = WipeColor;
                    DesaturatedComboColor = WipeDsColor;
                }

                if (ShowSectionHint) {
                    if (i == sectionObjects.Count - 1) {
                        hintSprite.Color(section.startTime, ComboColor);
                    } else {
                        hintSprite.Color(section.startTime - 40, section.startTime, prevColor, ComboColor);
                    }
                    prevColor = ComboColor;
                }

                var activateTimingRatioStart = ActivateTimingRatioStart4;
                var activateTimingRatioTransition = ActivateTimingRatioTransition4;
                if (section.beatCount == 2) {
                    activateTimingRatioStart = ActivateTimingRatioStart2;
                    activateTimingRatioTransition = ActivateTimingRatioTransition2;
                } else if (section.beatCount == 8) {
                    activateTimingRatioStart = ActivateTimingRatioStart8;
                    activateTimingRatioTransition = ActivateTimingRatioTransition8;
                } else if (section.beatCount == 16) {
                    activateTimingRatioStart = ActivateTimingRatioStart16;
                    activateTimingRatioTransition = ActivateTimingRatioTransition16;
                }
                
                var a = 0.0;
                if (((section.type == LineType.Horizontal) || (section.type == LineType.Vertical)) && Math.Abs(section.offset) > 0.0f) {
                    a = section.offset * groupTime;
                }

                // Circles
                foreach (OsuHitObject hitobject in objects.Reverse<OsuHitObject>()) 
                {
                    // if (section.type == LineType.CClockwise) {
                    //     Log("left: " + (hitobject.StartTime - 32.0*beat));
                    //     Log("right: " + songStartTimes[sI]);
                    // }
                    if ((hitobject.StartTime - 32.0*beat) < songStartTimes[sI]) {
                        sI++; 
                        sI = Math.Min(sI, songStartTimes.Count-1);
                        songStartTime = songStartTimes[sI];
                    }
                    var snappedTime = (int)((float)(hitobject.StartTime + 5 - songStartTime + a) / (float)groupTime) * groupTime + songStartTime - a;
                    if (prevSnappedTime != -1.0 && prevSnappedTime != snappedTime && !(((section.type == LineType.Horizontal) || (section.type == LineType.Vertical)) && Math.Abs(section.offset) > 0.0f)) {
                        evenGroup = !evenGroup;
                    }
                    var groupStartTime = (double)(snappedTime - groupTime);
                    var relativeTime = 1.0 - ((hitobject.StartTime + 5 - groupStartTime) / groupTime);

                    var activateTime = groupStartTime + groupTime - groupTime * activateTimingRatioStart - groupTime * activateTimingRatioTransition * relativeTime;
                    // if (section.type == LineType.CClockwise) {
                    //     Log("songstart: " + songStartTime);
                    //     Log("hitstart: " + hitobject.StartTime);
                    //     Log("groupcount: " + (int)((float)(hitobject.StartTime + 5 - songStartTime + a) / (float)groupTime));
                    // }

                    if (hitobject is OsuSlider) {
                        var timestep = Beatmap.GetTimingPointAt((int)activateTime).BeatDuration / (BeatDivisor * 4 / section.beatCount);
                        var startTime = hitobject.StartTime;
                        var transitionTime = 0.0;
                        
                        while (true)
                        {
                            var sliderEdgeSprite = hitobjectLayer.CreateSprite(SliderEdge, OsbOrigin.Centre, hitobject.PositionAtTime(startTime) + positionOffset);

                            var endTime = startTime + timestep;
                            var complete = hitobject.EndTime - endTime < -timestep;

                            sliderEdgeSprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime + transitionTime, 0, 1);
                            // sliderEdgeSprite.Fade(OsbEasing.In, activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, 0.8, 1.0);
                            sliderEdgeSprite.Fade(OsbEasing.In, startTime, startTime, 1, 0);

                            sliderEdgeSprite.Color(groupStartTime - FadeTime, Color4.DarkGray);
                            sliderEdgeSprite.Color(activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, Color4.DarkGray, Color4.White);

                            sliderEdgeSprite.Scale(groupStartTime - FadeTime, activateTime + transitionTime, SpriteScale * SliderScale * InactiveScaleRatio, SpriteScale * SliderScale * InactiveScaleRatio);
                            sliderEdgeSprite.Scale(OsbEasing.InOutQuad, activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, SpriteScale * SliderScale * InactiveScaleRatio, SpriteScale * SliderScale);
                            sliderEdgeSprite.Scale(OsbEasing.InOutQuad, activateTime + ActivateTransitionTime + transitionTime, hitobject.EndTime + FadeTime, SpriteScale * SliderScale, SpriteScale * SliderScale);

                            if (complete) break;
                            startTime += timestep;
                            transitionTime += timestep;
                        }
                        startTime = hitobject.StartTime;
                        transitionTime = 0.0;
                        while (true)
                        {
                            var sliderBodySprite = hitobjectLayer.CreateSprite(SliderBody, OsbOrigin.Centre, hitobject.PositionAtTime(startTime) + positionOffset);

                            var endTime = startTime + timestep;
                            var complete = hitobject.EndTime - endTime < -timestep;

                            sliderBodySprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime + transitionTime, 0, 1);
                            // sliderBodySprite.Fade(OsbEasing.In, activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, 0.8, 1.0);
                            sliderBodySprite.Fade(OsbEasing.In, startTime, startTime, 1, 0);

                            sliderBodySprite.Color(groupStartTime - FadeTime, DesaturatedComboColor);
                            sliderBodySprite.Color(activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, DesaturatedComboColor, ComboColor);

                            sliderBodySprite.Scale(groupStartTime - FadeTime, activateTime + transitionTime, SpriteScale * SliderScale * InactiveScaleRatio, SpriteScale * SliderScale * InactiveScaleRatio);
                            sliderBodySprite.Scale(OsbEasing.InOutQuad, activateTime + transitionTime, activateTime + ActivateTransitionTime + transitionTime, SpriteScale * SliderScale * InactiveScaleRatio, SpriteScale * SliderScale);
                            sliderBodySprite.Scale(OsbEasing.InOutQuad, activateTime + ActivateTransitionTime + transitionTime, hitobject.EndTime + FadeTime, SpriteScale * SliderScale, SpriteScale * SliderScale);

                            if (complete) break;
                            startTime += timestep;
                            transitionTime += timestep;
                        }
                    }
                    
                    var lightingSpritePath = "sb/circles/lighting.jpg";
                    var lightingSprite = hitobjectLayer.CreateSprite(lightingSpritePath, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var shadowSprite = hitobjectLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var hSprite = hitobjectLayer.CreateSprite(HitcirclePath, OsbOrigin.Centre, hitobject.Position + positionOffset);

                    var characterPath = "horizmon";
                    var inactiveFCount = 3;
                    var inactiveFTime = 120;
                    var activeFCount = 8;
                    var activeFTime = 70;
                    var hitFCount = 6;
                    var hitFTime = 60;
                    // var danceFCount = 3;
                    // var danceFTime = 100;

                    var rotation = 0.0f;
                    var progress = ((float)hitobject.StartTime - (float)snappedTime) / (float)groupTime + section.offset;

                    if (section.type == LineType.Clockwise || section.type == LineType.CClockwise) {
                        characterPath = "clockington";
                        inactiveFCount = 3;
                        inactiveFTime = 140;
                        activeFCount = 4;
                        activeFTime = 100;
                        hitFCount = 6;
                        hitFTime = 50;
                    }
                    if (section.type == LineType.WipeDown || section.type == LineType.WipeLeft || section.type == LineType.WipeRight || section.type == LineType.WipeUp) {
                        characterPath = "wypenyo";
                        inactiveFCount = 3;
                        inactiveFTime = 140;
                        activeFCount = 8;
                        activeFTime = 80;
                        hitFCount = 8;
                        hitFTime = 60;
                    }

                    if (section.type == LineType.Clockwise) {
                        rotation = progress * 2.0f * (float)Math.PI;
                    } else if (section.type == LineType.CClockwise) {
                        rotation = -progress * 2.0f * (float)Math.PI;
                    } else if (section.type == LineType.Vertical) {
                        characterPath = "vertimal";
                        inactiveFCount = 3;
                        inactiveFTime = 120;
                        activeFCount = 7;
                        activeFTime = 120;
                        hitFCount = 8;
                        hitFTime = 50;
                        // danceFCount = 12;
                        // danceFTime = 100;
                    } else if (section.type == LineType.Horizontal) {
                        rotation = 0.5f * (float)Math.PI;
                    } else if (section.type == LineType.WipeUp) {
                        rotation = 0.5f * (float)Math.PI - progress * (float)Math.PI;
                        if (!evenGroup ^ section.flipEven) {
                            rotation = -rotation;
                        }
                    } else if (section.type == LineType.WipeDown) {
                        rotation = 0.5f * (float)Math.PI - progress * (float)Math.PI;
                        if (!evenGroup ^ section.flipEven) {
                            rotation = -rotation;
                        }
                    } else if (section.type == LineType.WipeLeft) {
                        rotation = 0.5f * (float)Math.PI - progress * (float)Math.PI + 0.5f*(float)Math.PI;
                        if (!evenGroup ^ section.flipEven) {
                            rotation = -rotation;
                        }
                    }

                    var inactiveSpritePath = "sb/characters/" + characterPath + "_inactive.png";
                    var activeSpritePath = "sb/characters/" + characterPath + "_active.png";
                    var hitSpritePath = "sb/characters/" + characterPath + "_hit.png";

                    var inactiveSprite = hitobjectLayer.CreateAnimation(inactiveSpritePath, inactiveFCount, inactiveFTime, OsbLoopType.LoopForever, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var activeSprite = hitobjectLayer.CreateAnimation(activeSpritePath, activeFCount, activeFTime, OsbLoopType.LoopForever, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var hitSprite = hitobjectLayer.CreateAnimation(hitSpritePath, hitFCount, hitFTime, OsbLoopType.LoopOnce, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var overlaySprite = hitobjectLayer.CreateAnimation(HitcircleOverlayPath, FrameCount, FrameDuration, LoopType, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    var arrowSprite = hitobjectLayer.CreateSprite(ArrowsOverlayPath, OsbOrigin.Centre, hitobject.Position + positionOffset);
                    
                    if (hitobject is OsuSlider) {
                        var timestep = Beatmap.GetTimingPointAt((int)activateTime).BeatDuration / BeatDivisor;
                        var startTime = hitobject.StartTime;
                        while (true)
                        {
                            var endTime = startTime + timestep;
                            var complete = hitobject.EndTime - endTime < timestep;
                            if (complete) endTime = hitobject.EndTime;

                            var startPosition = hSprite.PositionAt(startTime);
                            // var startPosition = hSprite.PositionAt(startTime) + new CommandPosition(positionOffset);
                            hSprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime) + positionOffset);
                            activeSprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime) + positionOffset);
                            shadowSprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime) + positionOffset);
                            overlaySprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime) + positionOffset);
                            // arrowSprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime) + positionOffset);

                            if (complete) {
                                hitSprite.Move(hitobject.EndTime, hitobject.PositionAtTime(endTime) + positionOffset);
                                break;
                            }
                            startTime += timestep;
                        }
                    }

                    Random random = new Random((int)hitobject.StartTime);
                    bool flip = random.Next(2) == 0;

                    if (flip) {
                        hitSprite.FlipH(hitobject.EndTime);
                        activeSprite.FlipH(hitobject.EndTime);
                    }
                    var characterScale = 0.9;

                    hSprite.Scale(groupStartTime - FadeTime, activateTime, SpriteScale * InactiveScaleRatio, SpriteScale * InactiveScaleRatio);
                    hSprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime, 0, 1);
                    hSprite.Color(groupStartTime, DesaturatedComboColor);
                    hSprite.Color(activateTime, activateTime + ActivateTransitionTime, DesaturatedComboColor, ComboColor);

                    shadowSprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime, 0, 0.7);
                    shadowSprite.Fade(OsbEasing.In, activateTime, activateTime + ActivateTransitionTime, 0.7, 1);
                    shadowSprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);
                    overlaySprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime, 0, 0.7);
                    overlaySprite.Fade(OsbEasing.In, activateTime, activateTime + ActivateTransitionTime, 0.7, 1);
                    overlaySprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);
                    // arrowSprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime, 0, 0.7);
                    // arrowSprite.Fade(OsbEasing.In, activateTime, activateTime + ActivateTransitionTime, 0.7, 1);
                    arrowSprite.Fade(OsbEasing.In, activateTime, activateTime + ActivateTransitionTime, 0, 1);
                    arrowSprite.Fade(OsbEasing.None, hitobject.StartTime, hitobject.StartTime + FadeTime, 1, 0);

                    lightingSprite.Fade(OsbEasing.In, hitobject.StartTime, hitobject.StartTime + 50, 0, 0.7);
                    lightingSprite.Fade(OsbEasing.None, hitobject.StartTime + 50, hitobject.StartTime + 50 + FadeTime * 1.3, 0.7, 0);

                    inactiveSprite.Fade(OsbEasing.In, groupStartTime - FadeTime, groupStartTime, 0, 0.7);
                    inactiveSprite.Fade(activateTime, activateTime, 0.7, 0);
                    activeSprite.Fade(OsbEasing.In, activateTime, activateTime + ActivateTransitionTime, 0.7, 1);
                    activeSprite.Fade(hitobject.EndTime, hitobject.EndTime, 1, 0);
                    hitSprite.Fade(hitobject.EndTime, hitobject.EndTime, 0, 1);

                    // Hit fade + scale
                    hitSprite.Fade(OsbEasing.Out, hitobject.EndTime + FadeTime*0.8, hitobject.EndTime + FadeTime*2.0, 1, 0);
                    hitSprite.Scale(hitobject.EndTime, SpriteScale * characterScale);

                    arrowSprite.Rotate(hitobject.StartTime, rotation);

                    // arrowSprite.Color(groupStartTime - FadeTime, DesaturatedComboColor);
                    arrowSprite.Color(activateTime, activateTime + ActivateTransitionTime, DesaturatedComboColor, ComboColor);
                    arrowSprite.Additive(groupStartTime - FadeTime);
                    lightingSprite.Color(hitobject.StartTime, ComboColor);
                    lightingSprite.Additive(hitobject.StartTime);
                    overlaySprite.Color(groupStartTime - FadeTime, Color4.DarkGray);
                    overlaySprite.Color(activateTime, activateTime + ActivateTransitionTime, Color4.DarkGray, Color4.White);

                    shadowSprite.Scale(OsbEasing.InOutQuad, activateTime, activateTime + ActivateTransitionTime, SpriteScale * 1.25 * InactiveScaleRatio, SpriteScale * 1.25);
                    shadowSprite.Scale(activateTime + ActivateTransitionTime, hitobject.StartTime, SpriteScale * 1.25, SpriteScale * 1.25);
                    shadowSprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale * 1.25, SpriteScale * 1.25 * HitFadeScale);
                    overlaySprite.Scale(groupStartTime - FadeTime, activateTime, SpriteScale * InactiveScaleRatio, SpriteScale * InactiveScaleRatio);
                    overlaySprite.Scale(OsbEasing.InOutQuad, activateTime, activateTime + ActivateTransitionTime, SpriteScale * InactiveScaleRatio, SpriteScale);
                    overlaySprite.Scale(activateTime + ActivateTransitionTime, hitobject.EndTime, SpriteScale, SpriteScale);
                    overlaySprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale, SpriteScale * HitFadeScale);

                    // overlaySprite.Additive(hitobject.EndTime, hitobject.EndTime + FadeTime);
                    hSprite.Additive(hitobject.EndTime, hitobject.EndTime + FadeTime);
                    // arrowSprite.Scale(groupStartTime - FadeTime, activateTime, SpriteScale * InactiveScaleRatio, SpriteScale * InactiveScaleRatio);
                    arrowSprite.Scale(OsbEasing.InOutQuad, activateTime, activateTime + ActivateTransitionTime, SpriteScale * InactiveScaleRatio, SpriteScale);
                    arrowSprite.Scale(activateTime + ActivateTransitionTime, hitobject.StartTime, SpriteScale, SpriteScale);
                    arrowSprite.Scale(OsbEasing.Out, hitobject.StartTime, hitobject.StartTime + FadeTime, SpriteScale, SpriteScale * HitFadeScale * 1.13);

                    hSprite.Scale(OsbEasing.InOutQuad, activateTime, activateTime + ActivateTransitionTime, SpriteScale * InactiveScaleRatio, SpriteScale);
                    hSprite.Scale(activateTime + ActivateTransitionTime, hitobject.EndTime, SpriteScale, SpriteScale);

                    inactiveSprite.Scale(groupStartTime - FadeTime, activateTime, SpriteScale * characterScale * InactiveScaleRatio, SpriteScale * characterScale * InactiveScaleRatio);
                    activeSprite.Scale(OsbEasing.InOutQuad, activateTime, activateTime + ActivateTransitionTime, SpriteScale * characterScale * InactiveScaleRatio, SpriteScale * characterScale);
                    activeSprite.Scale(activateTime + ActivateTransitionTime, hitobject.EndTime, SpriteScale * characterScale, SpriteScale * characterScale);

                    hSprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale, SpriteScale * HitFadeScale);
                    hSprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);

                    lightingSprite.Scale(OsbEasing.Out, hitobject.StartTime, hitobject.StartTime + 50 + FadeTime * 2.0, SpriteScale * 0.9, SpriteScale * 0.9 * HitFadeScale * 1.1);
                    
                    // overlaySprite.Additive(hitobject.StartTime - ARTime, hitobject.EndTime + FadeTime);
                    // overlaySprite.Color(hitobject.StartTime - ARTime, hitobject.Color);

                    prevSnappedTime = snappedTime;
                }
            }
        }
    }
}