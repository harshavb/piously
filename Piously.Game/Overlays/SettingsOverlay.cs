﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Piously.Game.Overlays.Settings;
using Piously.Game.Overlays.Settings.Sections;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;

namespace Piously.Game.Overlays
{
    public class SettingsOverlay : SettingsPanel
    {
        public string Title => "settings";
        public string Description => "change the way Piously behaves";

        protected override IEnumerable<SettingsSection> CreateSections() => new SettingsSection[]
        {
            new GraphicsSection(),
        };

        private readonly List<SettingsSubPanel> subPanels = new List<SettingsSubPanel>();

        protected override Drawable CreateHeader() => new SettingsHeader(Title, Description);
        protected override Drawable CreateFooter() => new SettingsFooter();

        public SettingsOverlay()
            : base()
        {
        }

        public override bool AcceptsFocus => subPanels.All(s => s.State.Value != Visibility.Visible);

        private T createSubPanel<T>(T subPanel)
            where T : SettingsSubPanel
        {
            subPanel.Depth = 1;
            subPanel.Anchor = Anchor.TopRight;
            subPanel.State.ValueChanged += subPanelStateChanged;

            subPanels.Add(subPanel);

            return subPanel;
        }

        private void subPanelStateChanged(ValueChangedEvent<Visibility> state)
        {
            switch (state.NewValue)
            {
                case Visibility.Visible:
                    Background.FadeTo(0.9f, 300, Easing.OutQuint);

                    SectionsContainer.FadeOut(300, Easing.OutQuint);
                    ContentContainer.MoveToX(-WIDTH, 500, Easing.OutQuint);
                    break;

                case Visibility.Hidden:
                    Background.FadeTo(0.6f, 500, Easing.OutQuint);

                    SectionsContainer.FadeIn(500, Easing.OutQuint);
                    ContentContainer.MoveToX(0, 500, Easing.OutQuint);
                    break;
            }
        }

        protected override float ExpandedPosition => subPanels.Any(s => s.State.Value == Visibility.Visible) ? -WIDTH : base.ExpandedPosition;

        [BackgroundDependencyLoader]
        private void load()
        {
            foreach (var s in subPanels)
                ContentContainer.Add(s);
        }
    }
}
