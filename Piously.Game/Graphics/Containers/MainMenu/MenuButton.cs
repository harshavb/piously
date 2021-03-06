﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Piously.Game.Graphics.Sprites;

namespace Piously.Game.Graphics.Containers.MainMenu
{
    public class MenuButton : Container
    {
        public SpriteText Label;
        public MainMenuContainer parentLogo;
        public MenuButtonSprite menuButtonSprite { get; private set; }
        public Action clickAction;
        public Colour4 triangleColour;
        public string titleText;
        public bool textIsUpsideDown;

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
                    parentLogo = parentLogo,
                    clickAction = clickAction,
                    Colour = PiouslyColour.Gray(40),
                },
                new EquilateralTriangle
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0f, 4f),
                    Size = new Vector2(0.97f),
                    Colour = triangleColour,
                },
                new SpriteText
                {
                    Text = titleText,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0f, textIsUpsideDown ? -78f : -46f),
                    Rotation = textIsUpsideDown ? 0 : 180,
                    Colour = Colour4.White,
                    Font = new FontUsage(size: 32), 
                },
            };
        }
    }
}
