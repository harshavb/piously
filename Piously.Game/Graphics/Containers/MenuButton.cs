﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using Piously.Game.Graphics.Sprites;

namespace Piously.Game.Graphics.Containers
{
    public class MenuButton : Container
    {
        public SpriteText Label;
        public MenuLogo parentLogo;
        public MenuButtonSprite menuButtonSprite { get; private set; }
        public Action clickAction;
        public Colour4 triangleColour;
        public string titleText;

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = false;
            Children = new Drawable[]
            {
                menuButtonSprite = new MenuButtonSprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Size = new Vector2(1f),
                    parentLogo = parentLogo,
                    clickAction = clickAction,
                    Colour = triangleColour,
                },
                new SpriteText
                {
                    Text = titleText,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                    //OriginPosition = new Vector2(0, -52f),
                    Position = new Vector2(0f, -60f),
                    Rotation = 180,
                    Colour = Colour4.White,
                    Font = new FontUsage(null, 48, null, false, false),
                    
                },
            };
        }

        
    }
}
