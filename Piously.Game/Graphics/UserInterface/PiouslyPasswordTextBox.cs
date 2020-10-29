﻿using osuTK;
using osuTK.Graphics;
using osuTK.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Platform;

namespace Piously.Game.Graphics.UserInterface
{
    public class PiouslyPasswordTextBox : PiouslyTextBox, ISuppressKeyEventLogging
    {
        protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
        {
            AutoSizeAxes = Axes.Both,
            Child = new PasswordMaskChar(CalculatedTextSize),
        };

        protected override bool AllowUniqueCharacterSamples => false;

        protected override bool AllowClipboardExport => false;

        private readonly CapsWarning warning;

        [Resolved]
        private GameHost host { get; set; }

        public PiouslyPasswordTextBox()
        {
            Add(warning = new CapsWarning
            {
                Size = new Vector2(20),
                Origin = Anchor.CentreRight,
                Anchor = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 10 },
                Alpha = 0,
            });
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.CapsLock)
                updateCapsWarning(host.CapsLockEnabled);
            return base.OnKeyDown(e);
        }

        protected override void OnFocus(FocusEvent e)
        {
            updateCapsWarning(host.CapsLockEnabled);
            base.OnFocus(e);
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            updateCapsWarning(false);
            base.OnFocusLost(e);
        }

        private void updateCapsWarning(bool visible) => warning.FadeTo(visible ? 1 : 0, 250, Easing.OutQuint);

        public class PasswordMaskChar : Container
        {
            private readonly CircularContainer circle;

            public PasswordMaskChar(float size)
            {
                Size = new Vector2(size / 2, size);
                Children = new[]
                {
                    circle = new CircularContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.8f, 0),
                        Children = new[]
                        {
                            new Box
                            {
                                Colour = Color4.White,
                                RelativeSizeAxes = Axes.Both,
                            }
                        },
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                circle.FadeIn(500, Easing.OutQuint);
                circle.ResizeTo(new Vector2(0.8f), 500, Easing.OutQuint);
            }
        }

        private class CapsWarning : SpriteIcon, IHasTooltip
        {
            public string TooltipText => @"caps lock is active";

            public CapsWarning()
            {
                Icon = FontAwesome.Solid.ExclamationTriangle;
            }

            [BackgroundDependencyLoader]
            private void load(PiouslyColor color)
            {
                Colour = color.YellowLight;
            }
        }
    }
}