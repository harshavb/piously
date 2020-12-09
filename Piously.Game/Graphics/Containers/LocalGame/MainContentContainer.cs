﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using Piously.Game.Graphics.Containers.LocalGame.CreateGame;
using osuTK;

namespace Piously.Game.Graphics.Containers.LocalGame
{
    public class MainContentContainer : Container
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Size = new Vector2(0.45f, 0.85f);
            Position = new Vector2(0.5f, 0.1f);
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = new PiouslyColour().Gray1,
                Radius = 10,
                Roundness = 0.6f,
            };
            Children = new Drawable[]
            {
                // Background
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Size = new Vector2(1f),
                    Colour = new Colour4(0.2f, 0.2f, 0.2f, 0.4f),
                },

                // GameRules
                new SpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(0f, 0.025f),
                    Font = new FontUsage("Aller", 48, "Bold", false, false),
                    Text = "Game Rules",
                },

                // TimerContainer
                new TimerContainer(),

                // SaveNameContainer
                new SaveNameContainer(),

                // TwoPlayerContainer
                new CreateGame.TwoPlayerContainer(),

                // StartButtonContainer
                new StartButtonContainer(),
            };
        }
    }
}
