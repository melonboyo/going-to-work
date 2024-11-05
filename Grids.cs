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
using StorybrewScripts.Midori.Constants;

namespace StorybrewScripts
{
    public class Grids : StoryboardObjectGenerator
    {
        [Configurable] public double Opacity = 1.0;
        [Configurable] public Vector2 WipeUpPosition = new Vector2(320f, 56f);
        [Configurable] public Vector2 WipeDownPosition = new Vector2(320f, 440f);
        [Configurable] public Vector2 MiddlePosition = new Vector2(320f, 440f);
        [Configurable] public Vector2 ClockMiddle = new Vector2(320f, 248f);

        [Group("Timing")]
        [Configurable] public int FadeInTime = 600;
        [Configurable] public int FadeOutTime = 600;

        [Group("Color")]
        [Configurable] public Color4 ClockColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 VertColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 HoriColor = new Color4(255, 255, 255, 255);
        [Configurable] public Color4 WipeColor = new Color4(255, 255, 255, 255);

        public enum GridType {
            R2,
            R4,
            R8,
            R16,
            R4L,
            R4U,
            R4D,
            R2U,
            V2,
            V4,
            H,
        }

        public class Grid
        {
            public int startTime { get; set; }
            public int endTime { get; set;}
            public GridType type { get; set; }
        }

        public override void Generate()
        {
            var positionOffset = new Vector2(0f, 0f);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }

            var pathR2 = "sb/radial_grid_2.jpg";
            var pathR2U = "sb/radial_grid_2_up.jpg";
            var pathR4 = "sb/radial_grid_4.jpg";
            var pathR8 = "sb/radial_grid_8_square.jpg";
            var pathR16 = "sb/radial_grid_16.jpg";
            var pathV2 = "sb/vert_grid_2.jpg";
            var pathV4 = "sb/vert_grid.jpg";
            var pathH = "sb/horiz_grid.jpg";
            var pathR4L = "sb/radial_grid_4_left.jpg";

            List<Grid> grids = new List<Grid>
            {
                new Grid { startTime = 2853, endTime = 17762, type = GridType.R8 },
                new Grid { startTime = 17762, endTime = 29399, type = GridType.V4 },
                new Grid { startTime = 29399, endTime = 41035, type = GridType.R4D },
                new Grid { startTime = 41035, endTime = 55944, type = GridType.R4 },
                new Grid { startTime = 43944, endTime = 69762, type = GridType.V4 },
                new Grid { startTime = 58126, endTime = 63217, type = GridType.R4L },
                new Grid { startTime = 63944, endTime = 67459, type = GridType.R2 },
                new Grid { startTime = 69035, endTime = 81399, type = GridType.H },
                new Grid { startTime = 81399, endTime = 92308, type = GridType.R4U },
                new Grid { startTime = 81399, endTime = 93399, type = GridType.R16 },
                new Grid { startTime = 93035, endTime = 104490, type = GridType.H },
                new Grid { startTime = 93035, endTime = 107944, type = GridType.V4 },
                new Grid { startTime = 107944, endTime = 118368, type = GridType.R8 },
                new Grid { startTime = 118490, endTime = 125399, type = GridType.V2 },
                new Grid { startTime = 125520, endTime = 128550, type = GridType.R2U },
                new Grid { startTime = 128550, endTime = 130732, type = GridType.R2 },
                new Grid { startTime = 131459, endTime = 136853, type = GridType.H },
            };

            foreach (Grid g in grids) {
                var path = pathR4;
                var bitmap = GetMapsetBitmap(path);
                var color = Color4.White;
                var position = new Vector2(320f, 240f);
                var scale = 480.0f / bitmap.Height;
                switch (g.type)
                {
                    case GridType.R2:
                        path = pathR2;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = ClockColor;
                        // position = ClockMiddle;
                        break;
                    case GridType.R4:
                        path = pathR4;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = ClockColor;
                        // position = ClockMiddle;
                        break;
                    case GridType.R8:
                        path = pathR8;
                        bitmap = GetMapsetBitmap(path);
                        scale = 1.1f * 640.0f / bitmap.Width;
                        color = ClockColor;
                        position = ClockMiddle;
                        break;
                    case GridType.R16:
                        path = pathR16;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = ClockColor;
                        // position = ClockMiddle;
                        break;
                    case GridType.R4L:
                        path = pathR4L;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = WipeColor;
                        // position = ClockMiddle;
                        break;
                    case GridType.R4U:
                        path = pathR8;
                        bitmap = GetMapsetBitmap(path);
                        scale = 1.28f * 640.0f / bitmap.Width;
                        color = WipeColor;
                        position = WipeUpPosition;
                        break;
                    case GridType.R2U:
                        path = pathR2U;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = WipeColor;
                        // position = WipeUpPosition;
                        break;
                    case GridType.R4D:
                        path = pathR8;
                        bitmap = GetMapsetBitmap(path);
                        scale = 1.28f * 640.0f / bitmap.Width;
                        color = WipeColor;
                        position = WipeDownPosition;
                        break;
                    case GridType.V2:
                        path = pathV2;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = VertColor;
                        break;
                    case GridType.V4:
                        path = pathV4;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = VertColor;
                        break;
                    case GridType.H:
                        path = pathH;
                        bitmap = GetMapsetBitmap(path);
                        scale = 480.0f / bitmap.Height;
                        color = HoriColor;
                        break;
                    default:
                        break;
                }
                position += positionOffset;
                var sprite = GetLayer("Grids").CreateSprite(path, OsbOrigin.Centre, position);

                sprite.Scale(g.startTime, scale);
                sprite.Color(g.startTime, color);
                sprite.Additive(g.startTime);
                sprite.Fade(g.startTime, g.startTime + FadeInTime, 0, Opacity);
                sprite.Fade(g.endTime, g.endTime + FadeOutTime, Opacity, 0);

                if (g.type == GridType.R4D) {
                    sprite.Move(OsbEasing.InOutQuad, 34126, 35581, position, WipeUpPosition + positionOffset);
                }
                if (g.type == GridType.R4U) {
                    sprite.Move(OsbEasing.InOutQuad, 86490, 87944, position, WipeDownPosition + positionOffset);
                }
            }
        }
    }
}
