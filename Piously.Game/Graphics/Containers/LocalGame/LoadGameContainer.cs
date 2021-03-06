﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using Piously.Game.Graphics.Containers.LocalGame.LoadGame;

namespace Piously.Game.Graphics.Containers.LocalGame
{
    public class LoadGameContainer : VisibilityContainer
    {
        public bool isVisible = false;

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Size = new Vector2(550, 575);
            Position = new Vector2(0, 0);
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
                    Size = new Vector2(1f),
                    Colour = new Colour4(0.2f, 0.2f, 0.2f, 0.4f),
                },

                // SaveNameText
                new SpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(0f, 0.025f),
                    Font = new FontUsage("Aller", 48, "Bold", false, false),
                    Text = "No save game loaded",
                },

                // SaveStatusText
                new SpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(0f, 0.125f),
                    Font = new FontUsage("Aller", 32, null, false, false),
                    Text = "Select a save file to load a game",
                },

                // SaveFileListContainer
                new SaveFileListContainer(),

                // TwoPlayerContainer
                new TwoPlayerContainer(),

                // StartButtonContainer
                new ResumeButtonContainer(),
            };
        }

        protected override void PopIn()
        {
            this.MoveTo(new Vector2(0, 0), 200, Easing.OutQuad);
            isVisible = true;
        }

        protected override void PopOut()
        {
            this.MoveTo(new Vector2(1000, 0), 200, Easing.OutQuad);
            isVisible = false;
        }
    }
}
