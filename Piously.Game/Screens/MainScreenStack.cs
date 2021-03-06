﻿using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using Piously.Game.Screens.Backgrounds;
using osuTK;

namespace Piously.Game.Screens
{
    public class MainScreenStack : ScreenStack
    {
        public MainScreenStack()
            : base(false)
        {
            Scale = new Vector2(1.06f);
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        public MainScreenStack(IScreen baseScreen)
            : base(baseScreen, false)
        {
            Scale = new Vector2(1.06f);
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        //public float ParallaxAmount { set => parallax.ParallaxAmount = ParallaxContainer.DEFAULT_PARALLAX_AMOUNT * value; }

        public void Push(BackgroundScreen screen)
        {
            if (screen == null)
                return;

            if (EqualityComparer<BackgroundScreen>.Default.Equals((BackgroundScreen)CurrentScreen, screen))
                return;

            base.Push(screen);
        }
    }
}
