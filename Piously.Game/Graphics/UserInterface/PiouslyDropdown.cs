﻿using System.Linq;
using osuTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using Piously.Game.Graphics.Containers;
using osuTK;

namespace Piously.Game.Graphics.UserInterface
{
    public class PiouslyDropdown<T> : Dropdown<T>, IHasAccentColour
    {
        private const float corner_radius = 4;

        private Color4 accentColour;

        public Color4 AccentColour
        {
            get => accentColour;
            set
            {
                accentColour = value;
                updateAccentColour();
            }
        }

        [BackgroundDependencyLoader]
        private void load(PiouslyColour colours)
        {
            if (accentColour == default)
                accentColour = colours.PinkDarker;
            updateAccentColour();
        }

        private void updateAccentColour()
        {
            if (Header is IHasAccentColour header) header.AccentColour = accentColour;

            if (Menu is IHasAccentColour menu) menu.AccentColour = accentColour;
        }

        protected override DropdownHeader CreateHeader() => new PiouslyDropdownHeader();

        protected override DropdownMenu CreateMenu() => new PiouslyDropdownMenu();

        #region OsuDropdownMenu

        protected class PiouslyDropdownMenu : DropdownMenu, IHasAccentColour
        {
            public override bool HandleNonPositionalInput => State == MenuState.Open;

            // todo: this uses the same styling as PiouslyMenu. hopefully we can just use PiouslyMenu in the future with some refactoring
            public PiouslyDropdownMenu()
            {
                CornerRadius = corner_radius;
                BackgroundColour = Color4.Black.Opacity(0.5f);

                MaskingContainer.CornerRadius = corner_radius;

                // todo: this uses the same styling as PiouslyMenu. hopefully we can just use PiouslyMenu in the future with some refactoring
                ItemsContainer.Padding = new MarginPadding(5);
            }

            // todo: this uses the same styling as PiouslyMenu. hopefully we can just use PiouslyMenu in the future with some refactoring
            protected override void AnimateOpen() => this.FadeIn(300, Easing.OutQuint);
            protected override void AnimateClose() => this.FadeOut(300, Easing.OutQuint);

            // todo: this uses the same styling as PiouslyMenu. hopefully we can just use PiouslyMenu in the future with some refactoring
            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
                }
            }

            private Color4 accentColour;

            public Color4 AccentColour
            {
                get => accentColour;
                set
                {
                    accentColour = value;
                    foreach (var c in Children.OfType<IHasAccentColour>())
                        c.AccentColour = value;
                }
            }

            protected override Menu CreateSubMenu() => new PiouslyMenu(Direction.Vertical);

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawablePiouslyDropdownMenuItem(item) { AccentColour = accentColour };

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new PiouslyScrollContainer(direction);

            #region DrawableOsuDropdownMenuItem

            public class DrawablePiouslyDropdownMenuItem : DrawableDropdownMenuItem, IHasAccentColour
            {
                // IsHovered is used
                public override bool HandlePositionalInput => true;

                private Color4? accentColour;

                public Color4 AccentColour
                {
                    get => accentColour ?? nonAccentSelectedColour;
                    set
                    {
                        accentColour = value;
                        updateColours();
                    }
                }

                private void updateColours()
                {
                    BackgroundColourHover = accentColour ?? nonAccentHoverColour;
                    BackgroundColourSelected = accentColour ?? nonAccentSelectedColour;
                    UpdateBackgroundColour();
                    UpdateForegroundColour();
                }

                private Color4 nonAccentHoverColour;
                private Color4 nonAccentSelectedColour;

                public DrawablePiouslyDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    Foreground.Padding = new MarginPadding(2);

                    Masking = true;
                    CornerRadius = corner_radius;
                }

                [BackgroundDependencyLoader]
                private void load(PiouslyColour colours)
                {
                    BackgroundColour = Color4.Transparent;

                    nonAccentHoverColour = colours.PinkDarker;
                    nonAccentSelectedColour = Color4.Black.Opacity(0.5f);
                    updateColours();
                }

                protected override void UpdateForegroundColour()
                {
                    base.UpdateForegroundColour();

                    if (Foreground.Children.FirstOrDefault() is Content content) content.Chevron.Alpha = IsHovered ? 1 : 0;
                }

                protected override Drawable CreateContent() => new Content();

                protected new class Content : FillFlowContainer, IHasText
                {
                    public string Text
                    {
                        get => Label.Text;
                        set => Label.Text = value;
                    }

                    public readonly SpriteText Label;
                    public readonly SpriteIcon Chevron;

                    public Content()
                    {
                        RelativeSizeAxes = Axes.X;
                        AutoSizeAxes = Axes.Y;
                        Direction = FillDirection.Horizontal;

                        Children = new Drawable[]
                        {
                            Chevron = new SpriteIcon
                            {
                                AlwaysPresent = true,
                                Icon = FontAwesome.Solid.ChevronRight,
                                Colour = Color4.Black,
                                Alpha = 0.5f,
                                Size = new Vector2(8),
                                Margin = new MarginPadding { Left = 3, Right = 3 },
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                            Label = new SpriteText
                            {
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                        };
                    }
                }
            }

            #endregion
        }

        #endregion

        public class PiouslyDropdownHeader : DropdownHeader, IHasAccentColour
        {
            protected readonly SpriteText Text;

            protected override string Label
            {
                get => Text.Text;
                set => Text.Text = value;
            }

            protected readonly SpriteIcon Icon;

            private Color4 accentColour;

            public virtual Color4 AccentColour
            {
                get => accentColour;
                set
                {
                    accentColour = value;
                    BackgroundColourHover = accentColour;
                }
            }

            public PiouslyDropdownHeader()
            {
                Foreground.Padding = new MarginPadding(4);

                AutoSizeAxes = Axes.None;
                Margin = new MarginPadding { Bottom = 4 };
                CornerRadius = corner_radius;
                Height = 40;

                Foreground.Children = new Drawable[]
                {
                    Text = new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    Icon = new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.ChevronDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Margin = new MarginPadding { Right = 5 },
                        Size = new Vector2(12),
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(PiouslyColour colours)
            {
                BackgroundColour = Color4.Black.Opacity(0.5f);
                BackgroundColourHover = colours.PinkDarker;
            }
        }
    }
}