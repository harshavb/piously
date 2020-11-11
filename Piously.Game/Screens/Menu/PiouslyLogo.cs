﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using Piously.Game.Graphics;
using Piously.Game.Graphics.Containers;
using Piously.Game.Graphics.Backgrounds;
using osuTK;
using System;

namespace Piously.Game.Screens.Menu
{
    public class PiouslyLogo : HexagonalContainer
    {
        public MenuState menuState;

        private const double transition_length = 300;

        private Sprite logo;

        private readonly Container colourAndHexagons;
        private readonly HexagonalContainer hexagonalContainer;
        private readonly Graphics.Shapes.Trapezoid trapezoid;
        private readonly Hexagons hexagons;

        public bool Hexagons
        {
            set => colourAndHexagons.FadeTo(value ? 1 : 0, transition_length, Easing.OutQuint);
        }

        public PiouslyLogo()
        {
            menuState = MenuState.Closed;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(800, 800);

            Children = new Drawable[]
            {
                hexagonalContainer = new HexagonalContainer()
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0.95f),
                    Child = colourAndHexagons = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            trapezoid = new Graphics.Shapes.Trapezoid
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = PiouslyColour.PiouslyLightYellow,
                            },
                            hexagons = new Hexagons
                            {
                                HexagonScale = 3,
                                ColourLight = PiouslyColour.PiouslyLighterYellow,
                                ColourDark = PiouslyColour.PiouslyYellow,
                                RelativeSizeAxes = Axes.Both,
                            }
                        }
                    }
                },
                logo = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            logo.Texture = textures.Get(@"Menu/logo");
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(1.15f, 25);
            this.ScaleTo(1.13f, 25);
            switch (menuState)
            {
                case MenuState.Opened:
                    menuState = MenuState.Closed;
                    break;
                case MenuState.Closed:
                    menuState = MenuState.Opened;
                    break;
                default:
                    Console.WriteLine("what");
                    break;
            }
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (IsHovered)
                this.ScaleTo(1.1f, 25);
            Console.WriteLine(trapezoid.DrawHeight);
            Console.WriteLine(trapezoid.DrawWidth);
            Console.WriteLine(trapezoid.X);
            Console.WriteLine(trapezoid.Y);
            Console.WriteLine(trapezoid.ConservativeScreenSpaceDrawQuad);
        }
        protected override bool OnHover(HoverEvent e)
        {
            this.ScaleTo(1.1f, 50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.ScaleTo(1.00f, 50);
        }
    }
}
