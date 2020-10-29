﻿using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using Piously.Game.Graphics;
using osuTK;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;

namespace Piously.Game.Overlays.Toolbar
{
    public class Toolbar : VisibilityContainer
    {
        public const float HEIGHT = 40;
        public const float TOOLTIP_HEIGHT = 30;

        public Action OnHome;

        private ToolbarUserButton userButton;

        private const double transition_time = 500;

        protected readonly IBindable<OverlayActivation> OverlayActivationMode = new Bindable<OverlayActivation>(OverlayActivation.All);

        // Toolbar components like RulesetSelector should receive keyboard input events even when the toolbar is hidden.
        public override bool PropagateNonPositionalInputSubTree => true;

        public Toolbar()
        {
            RelativeSizeAxes = Axes.X;
            Size = new Vector2(1, HEIGHT);
        }

        [BackgroundDependencyLoader(true)]
        private void load(PiouslyGame PiouslyGame)
        {
            Children = new Drawable[]
            {
                new ToolbarBackground(),
                new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new ToolbarSettingsButton(),
                        new ToolbarHomeButton
                        {
                            Action = () => OnHome?.Invoke()
                        },
                    }
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Direction = FillDirection.Horizontal,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    //TO BE IMPLEMENTED
                    Children = new Drawable[]
                    {
                        //new ToolbarNewsButton(),
                        //new ToolbarChangelogButton(),
                        //new ToolbarRankingsButton(),
                        //new ToolbarBeatmapListingButton(),
                        //new ToolbarChatButton(),
                        //new ToolbarSocialButton(),
                        //new ToolbarMusicButton(),
                        //new ToolbarButton
                        //{
                        //    Icon = FontAwesome.Solid.search
                        //},
                        userButton = new ToolbarUserButton(),
                        //new ToolbarNotificationButton(),
                    }
                }
            };

            if (PiouslyGame != null)
                OverlayActivationMode.BindTo(PiouslyGame.OverlayActivationMode);
        }

        public class ToolbarBackground : Container
        {
            private readonly Box gradientBackground;

            public ToolbarBackground()
            {
                RelativeSizeAxes = Axes.Both;
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = PiouslyColor.Gray(0.1f),
                    },
                    gradientBackground = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomLeft,
                        Alpha = 0,
                        Height = 100,
                        Colour = ColourInfo.GradientVertical(
                            PiouslyColor.Gray(0).Opacity(0.9f), PiouslyColor.Gray(0).Opacity(0)),
                    },
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                gradientBackground.FadeIn(transition_time, Easing.OutQuint);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                gradientBackground.FadeOut(transition_time, Easing.OutQuint);
            }
        }

        protected override void UpdateState(ValueChangedEvent<Visibility> state)
        {
            if (state.NewValue == Visibility.Visible && OverlayActivationMode.Value == OverlayActivation.Disabled)
            {
                State.Value = Visibility.Hidden;
                return;
            }

            base.UpdateState(state);
        }

        protected override void PopIn()
        {
            this.MoveToY(0, transition_time, Easing.OutQuint);
            this.FadeIn(transition_time / 4, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            userButton.StateContainer?.Hide();

            this.MoveToY(-DrawSize.Y, transition_time, Easing.OutQuint);
            this.FadeOut(transition_time, Easing.InQuint);
        }
    }
}