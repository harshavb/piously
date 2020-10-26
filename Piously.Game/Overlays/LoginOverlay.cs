﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Piously.Game.Graphics;
using Piously.Game.Overlays.Settings.Sections.General;
using osuTK.Graphics;
using osu.Framework.Graphics.Shapes;
using Piously.Game.Graphics.Containers;
using Piously.Game.Graphics.Cursor;
using System;

namespace Piously.Game.Overlays
{
    public class LoginOverlay : PiouslyFocusedOverlayContainer
    {
        private LoginSettings settingsSection;

        private const float transition_time = 400;

        /// <summary>
        /// Provide a source for the toolbar height.
        /// </summary>
        public Func<float> GetToolbarHeight;

        public LoginOverlay()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(PiouslyColor colors)
        {
            Children = new Drawable[]
            {
                new PiouslyContextMenuContainer
                {
                    Width = 360,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.Black,
                            Alpha = 0.6f,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Masking = true,
                            AutoSizeDuration = transition_time,
                            AutoSizeEasing = Easing.OutQuint,
                            Children = new Drawable[]
                            {
                                settingsSection = new LoginSettings
                                {
                                    Padding = new MarginPadding(10),
                                    RequestHide = Hide,
                                },
                                new Box
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Height = 3,
                                    Colour = colors.Yellow,
                                    Alpha = 1,
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

            settingsSection.Bounding = true;
            this.FadeIn(transition_time, Easing.OutQuint);

            GetContainingInputManager().ChangeFocus(settingsSection);
        }

        protected override void PopOut()
        {
            base.PopOut();

            settingsSection.Bounding = false;
            this.FadeOut(transition_time);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Padding = new MarginPadding { Top = GetToolbarHeight?.Invoke() ?? 0 };
        }
    }
}
