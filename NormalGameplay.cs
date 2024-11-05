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
    public class NormalGameplay : StoryboardObjectGenerator
    {
        [Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 0;

        [Configurable]
        public int BeatDivisor = 8;

        [Configurable]
        public int FadeTime = 200;

        [Configurable]
        public string HitcirclePath = "sb/circle.png";

        [Configurable]
        public string ApproachcirclePath = "sb/approachcircle.png";

        [Configurable]
        public string SliderBody = "sb/sliderbody.png";

        [Configurable]
        public string SliderEdge = "sb/slideredge.png";

        [Configurable]
        public int ARTime = 600;

        [Configurable]
        public double SpriteScale = 1;

        [Configurable]
        public double SliderScale = 1;

        [Configurable]
        public int HitCircleGap = 0;

        [Configurable]
        public Color4 Color = Color4.White;

        public string[] num = new string[10];

        public override void Generate()
        {
            var positionOffset = new Vector2(0, 0);
            if (Constants.AdjustHeightForMapping) {
                positionOffset = Constants.MappingOffset;
            }
            var hitobjectLayer = GetLayer("HitObjects");
            foreach (var hitobject in Beatmap.HitObjects.Reverse<OsuHitObject>())
            {
                if ((StartTime != 0 || EndTime != 0) && 
                    (hitobject.StartTime < StartTime - 5 || EndTime - 5 <= hitobject.StartTime))
                    continue;
                
                var lightingSprite = hitobjectLayer.CreateSprite("sb/circles/lighting.jpg", OsbOrigin.Centre, hitobject.Position + positionOffset);
                var shadowSprite = hitobjectLayer.CreateSprite("sb/circles/hitcircle_shadow.png", OsbOrigin.Centre, hitobject.Position + positionOffset);
                var hSprite = hitobjectLayer.CreateSprite(HitcirclePath, OsbOrigin.Centre, hitobject.Position + positionOffset);
                
                // var inactiveSprite = hitobjectLayer.CreateAnimation(inactiveSpritePath, inactiveFCount, inactiveFTime, OsbLoopType.LoopForever, OsbOrigin.Centre, hitobject.Position + positionOffset);
                // var activeSprite = hitobjectLayer.CreateAnimation(activeSpritePath, activeFCount, activeFTime, OsbLoopType.LoopForever, OsbOrigin.Centre, hitobject.Position + positionOffset);
                // var hitSprite = hitobjectLayer.CreateAnimation(hitSpritePath, hitFCount, hitFTime, OsbLoopType.LoopOnce, OsbOrigin.Centre, hitobject.Position + positionOffset);
                var overlaySprite = hitobjectLayer.CreateAnimation("sb/circles/hitcircleoverlay.png", 3, 100, OsbLoopType.LoopForever, OsbOrigin.Centre, hitobject.Position + positionOffset);
                
                hSprite.Scale(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.EndTime, SpriteScale, SpriteScale);
                hSprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale, SpriteScale * 1.2);
                hSprite.Fade(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.StartTime - ARTime*2/5, 0, 1);
                hSprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);
                //hSprite.Additive(hitobject.StartTime - ARTime, hitobject.EndTime + FadeTime);
                hSprite.Color(hitobject.StartTime - ARTime, Color);
                
                var shadowScale = 1.2;

                overlaySprite.Scale(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.EndTime, SpriteScale, SpriteScale);
                overlaySprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale, SpriteScale * 1.2);
                overlaySprite.Fade(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.StartTime - ARTime*2/5, 0, 1);
                overlaySprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);
                //overlaySprite.Additive(hitobject.StartTime - ARTime, hitobject.EndTime + FadeTime);
                // overlaySprite.Color(hitobject.StartTime - ARTime, Color);

                shadowSprite.Scale(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.EndTime, SpriteScale * shadowScale, SpriteScale * shadowScale);
                shadowSprite.Scale(OsbEasing.In, hitobject.EndTime, hitobject.EndTime + FadeTime, SpriteScale * shadowScale, SpriteScale * shadowScale * 1.2);
                shadowSprite.Fade(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.StartTime - ARTime*2/5, 0, 1);
                shadowSprite.Fade(OsbEasing.None, hitobject.EndTime, hitobject.EndTime + FadeTime, 1, 0);
                //shadowSprite.Additive(hitobject.StartTime - ARTime, hitobject.EndTime + FadeTime);
                // shadowSprite.Color(hitobject.StartTime - ARTime, Color);

                lightingSprite.Fade(OsbEasing.In, hitobject.StartTime, hitobject.StartTime + 50, 0, 0.7);
                lightingSprite.Fade(OsbEasing.None, hitobject.StartTime + 50, hitobject.StartTime + 50 + FadeTime * 1.3, 0.7, 0);
                lightingSprite.Color(hitobject.StartTime, Color);
                lightingSprite.Additive(hitobject.StartTime);
                lightingSprite.Scale(OsbEasing.Out, hitobject.StartTime, hitobject.StartTime + 50 + FadeTime * 2.0, SpriteScale * 0.9, SpriteScale * 0.9 * 1.2 * 1.1);

                var aSprite = hitobjectLayer.CreateSprite("sb/circles/approachcircle.png", OsbOrigin.Centre, hitobject.Position + positionOffset);

                aSprite.Scale(OsbEasing.None, hitobject.StartTime - ARTime, hitobject.StartTime, SpriteScale * 5.5, SpriteScale);
                aSprite.Fade(OsbEasing.In, hitobject.StartTime - ARTime, hitobject.StartTime - ARTime * 2 / 5, 0, 1);
                aSprite.Fade(OsbEasing.None, hitobject.StartTime, hitobject.StartTime + FadeTime, 1, 0);
                aSprite.Additive(hitobject.StartTime - ARTime, hitobject.EndTime + FadeTime);
                aSprite.Color(hitobject.StartTime - ARTime, Color);
            }
        }
    }
}
