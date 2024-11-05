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
    namespace Midori
    {
        namespace Constants
        {
            public class Constants
            {
                public enum LineType {
                    Horizontal = 0,
                    Vertical = 1,
                    Clock = 2,
                    Wipe = 3,
                }
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                static public bool AdjustHeightForMapping = false; // !!!!
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                static public Vector2 MappingOffset = new Vector2(0, 16);
                static public double LinePulseSize = 1.8;
                
                public Constants()
                {
                    return;
                }
            }
        }
    }
}