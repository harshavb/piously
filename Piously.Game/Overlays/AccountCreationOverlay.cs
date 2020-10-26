﻿using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using Piously.Game.Graphics;
using Piously.Game.Graphics.Containers;
using Piously.Game.Online.API;
using Piously.Game.Overlays.AccountCreation;
using osuTK;
using osuTK.Graphics;

namespace Piously.Game.Overlays
{
    public class AccountCreationOverlay : PiouslyFocusedOverlayContainer
    {
        private const float transition_time = 400;

        private ScreenWelcome welcomeScreen;

        public AccountCreationOverlay()
        {
            Size = new Vector2(620, 450);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        private readonly IBindable<APIState> apiState = new Bindable<APIState>();

        [BackgroundDependencyLoader]
        private void load(PiouslyColor colors, IAPIProvider api)
        {
            apiState.BindTo(api.State);
            apiState.BindValueChanged(apiStateChanged, true);

            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 5,
                        Colour = Color4.Black.Opacity(0.2f),
                    },
                    Masking = true,
                    CornerRadius = 10,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.Black,
                            Alpha = 0.6f,
                        },
                        new DelayedLoadWrapper(new AccountCreationBackground(), 0),
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.6f,
                            AutoSizeDuration = transition_time,
                            AutoSizeEasing = Easing.OutQuint,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black,
                                    Alpha = 0.9f,
                                },
                                new ScreenStack(welcomeScreen = new ScreenWelcome())
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                            }
                        }
                    }
                }
            };
        }

        protected override void PopIn()
        {
            base.PopIn();
            this.FadeIn(transition_time, Easing.OutQuint);

            if (welcomeScreen.GetChildScreen() != null)
                welcomeScreen.MakeCurrent();
        }

        protected override void PopOut()
        {
            base.PopOut();
            this.FadeOut(100);
        }

        private void apiStateChanged(ValueChangedEvent<APIState> state) => Schedule(() =>
        {
            switch (state.NewValue)
            {
                case APIState.Offline:
                case APIState.Failing:
                    break;

                case APIState.Connecting:
                    break;

                case APIState.Online:
                    Hide();
                    break;
            }
        });
    }
}
