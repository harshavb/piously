﻿using System;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Piously.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;
using osu.Framework.Extensions.Color4Extensions;
using Piously.Game.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace Piously.Game.Screens.Menu
{
    /// <summary>
    /// Button designed specifically for the osu!next main menu.
    /// In order to correctly flow, we have to use a negative margin on the parent container (due to the parallelogram shape).
    /// </summary>
    public class Button : ScalingContainer, IStateful<ButtonState>
    {
        public event Action<ButtonState> StateChanged;

        public readonly Key TriggerKey;

        private readonly Container iconText;
        private readonly Container box;
        private readonly Box boxHoverLayer;
        private readonly SpriteIcon icon;
        private readonly string sampleName;

        /// <summary>
        /// The menu state for which we are visible for.
        /// </summary>
        public ButtonSystemState VisibleState = ButtonSystemState.TopLevel;

        private readonly Action clickAction;
        private SampleChannel sampleClick;
        private SampleChannel sampleHover;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => box.ReceivePositionalInputAt(screenSpacePos);

        public Button(string text, string sampleName, IconUsage symbol, Color4 colour, Action clickAction = null, float extraWidth = 0, Key triggerKey = Key.Unknown)
        {
            this.sampleName = sampleName;
            this.clickAction = clickAction;
            TriggerKey = triggerKey;

            AutoSizeAxes = Axes.Both;
            Alpha = 0;

            Vector2 boxSize = new Vector2(ButtonSystem.BUTTON_WIDTH + Math.Abs(extraWidth), ButtonArea.BUTTON_AREA_HEIGHT);

            Children = new Drawable[]
            {
                box = new Container
                {
                    // box needs to be always present to ensure the button is always sized correctly for flow
                    AlwaysPresent = true,
                    Masking = true,
                    MaskingSmoothness = 2,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Color4.Black.Opacity(0.2f),
                        Roundness = 5,
                        Radius = 8,
                    },
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0, 1),
                    Size = boxSize,
                    Shear = new Vector2(ButtonSystem.WEDGE_WIDTH / boxSize.Y, 0),
                    Children = new[]
                    {
                        new Box
                        {
                            EdgeSmoothness = new Vector2(1.5f, 0),
                            RelativeSizeAxes = Axes.Both,
                            Colour = colour,
                        },
                        boxHoverLayer = new Box
                        {
                            EdgeSmoothness = new Vector2(1.5f, 0),
                            RelativeSizeAxes = Axes.Both,
                            Blending = BlendingParameters.Additive,
                            Colour = Color4.White,
                            Alpha = 0,
                        },
                    }
                },
                iconText = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Position = new Vector2(extraWidth / 2, 0),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        icon = new SpriteIcon
                        {
                            Shadow = true,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(30),
                            Position = new Vector2(0, 0),
                            Icon = symbol
                        },
                        new PiouslySpriteText
                        {
                            Shadow = true,
                            AllowMultiline = false,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Position = new Vector2(0, 35),
                            Text = text
                        }
                    }
                }
            };
        }

        private bool rightward;

        protected override bool OnHover(HoverEvent e)
        {
            if (State != ButtonState.Expanded) return true;

            sampleHover?.Play();

            box.ScaleTo(new Vector2(1.5f, 1), 500, Easing.OutElastic);

            icon.ClearTransforms();
            icon.RotateTo(rightward ? -10 : 10, 500, Easing.InOutSine);
            icon.ScaleTo(new Vector2(1, 0.9f), 200, Easing.Out);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            icon.ClearTransforms();
            icon.RotateTo(0, 500, Easing.Out);
            icon.MoveTo(Vector2.Zero, 500, Easing.Out);
            icon.ScaleTo(Vector2.One, 200, Easing.Out);

            if (State == ButtonState.Expanded)
                box.ScaleTo(new Vector2(1, 1), 500, Easing.OutElastic);
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleHover = audio.Samples.Get(@"Menu/button-hover");
            if (!string.IsNullOrEmpty(sampleName))
                sampleClick = audio.Samples.Get($@"Menu/{sampleName}");
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            boxHoverLayer.FadeTo(0.1f, 1000, Easing.OutQuint);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            boxHoverLayer.FadeTo(0, 1000, Easing.OutQuint);
            base.OnMouseUp(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            trigger();
            return true;
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Repeat || e.ControlPressed || e.ShiftPressed || e.AltPressed)
                return false;

            if (TriggerKey == e.Key && TriggerKey != Key.Unknown)
            {
                trigger();
                return true;
            }

            return false;
        }

        private void trigger()
        {
            sampleClick?.Play();

            clickAction?.Invoke();

            boxHoverLayer.ClearTransforms();
            boxHoverLayer.Alpha = 0.9f;
            boxHoverLayer.FadeOut(800, Easing.OutExpo);
        }

        public override bool HandleNonPositionalInput => state == ButtonState.Expanded;
        public override bool HandlePositionalInput => state != ButtonState.Exploded && box.Scale.X >= 0.8f;

        protected override void Update()
        {
            iconText.Alpha = Math.Clamp((box.Scale.X - 0.5f) / 0.3f, 0, 1);
            base.Update();
        }

        public int ContractStyle;

        private ButtonState state;

        public ButtonState State
        {
            get => state;

            set
            {
                if (state == value)
                    return;

                state = value;

                switch (state)
                {
                    case ButtonState.Contracted:
                        switch (ContractStyle)
                        {
                            default:
                                box.ScaleTo(new Vector2(0, 1), 500, Easing.OutExpo);
                                this.FadeOut(500);
                                break;

                            case 1:
                                box.ScaleTo(new Vector2(0, 1), 400, Easing.InSine);
                                this.FadeOut(800);
                                break;
                        }

                        break;

                    case ButtonState.Expanded:
                        const int expand_duration = 500;
                        box.ScaleTo(new Vector2(1, 1), expand_duration, Easing.OutExpo);
                        this.FadeIn(expand_duration / 6f);
                        break;

                    case ButtonState.Exploded:
                        const int explode_duration = 200;
                        box.ScaleTo(new Vector2(2, 1), explode_duration, Easing.OutExpo);
                        this.FadeOut(explode_duration / 4f * 3);
                        break;
                }

                StateChanged?.Invoke(State);
            }
        }

        public ButtonSystemState ButtonSystemState
        {
            set
            {
                ContractStyle = 0;

                switch (value)
                {
                    case ButtonSystemState.Initial:
                        State = ButtonState.Contracted;
                        break;

                    case ButtonSystemState.EnteringMode:
                        ContractStyle = 1;
                        State = ButtonState.Contracted;
                        break;

                    default:
                        if (value == VisibleState)
                            State = ButtonState.Expanded;
                        else if (value < VisibleState)
                            State = ButtonState.Contracted;
                        else
                            State = ButtonState.Exploded;
                        break;
                }
            }
        }
    }

    public enum ButtonState
    {
        Contracted,
        Expanded,
        Exploded
    }
}