﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using Piously.Game.Graphics.Containers;

namespace Piously.Game.Users.Drawables
{
    [LongRunningLoad]
    public class DrawableAvatar : Container
    {
        /// <summary>
        /// Whether to open the user's profile when clicked.
        /// </summary>
        public readonly BindableBool OpenOnClick = new BindableBool(true);

        private readonly User user;

        [Resolved(CanBeNull = true)]
        private PiouslyGame game { get; set; }

        /// <summary>
        /// An avatar for specified user.
        /// </summary>
        /// <param name="user">The user. A null value will get a placeholder avatar.</param>
        public DrawableAvatar(User user = null)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            if (textures == null)
                throw new ArgumentNullException(nameof(textures));

            Texture texture = null;
            if (user != null && user.Id > 1) texture = textures.Get($@"https://piously.xyz/{user.Id}"); //Domain to be determined
            texture ??= textures.Get(@"Online/avatar-guest");

            ClickableArea clickableArea;
            Add(clickableArea = new ClickableArea
            {
                RelativeSizeAxes = Axes.Both,
                Child = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Texture = texture,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                Action = openProfile
            });

            clickableArea.Enabled.BindTo(OpenOnClick);
        }

        private void openProfile()
        {
            if (!OpenOnClick.Value)
                return;

            if (user?.Id > 1)
                game?.ShowUser(user.Id);
        }

        private class ClickableArea : PiouslyClickableContainer
        {
            public override string TooltipText => Enabled.Value ? @"view profile" : null;

            protected override bool OnClick(ClickEvent e)
            {
                if (!Enabled.Value)
                    return false;

                return base.OnClick(e);
            }
        }
    }
}