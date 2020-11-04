﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Graphics;
using Piously.Game.Graphics.Primitives;
using Piously.Game.Graphics.Shapes;
using Piously.Game.Input.Bindings;
using Piously.Game.Screens.Menu;
using osuTK;
using LogLevel = osu.Framework.Logging.LogLevel;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;

namespace Piously.Game
{
    //The actual game, specifically loads the UI
    public class PiouslyGame : PiouslyGameBase
    {
        private ScreenStack piouslyMenuScreenStack;
        private TestScreen testScreen;
        private ScreenStack piouslyMenuScreenStack2;
        private TestScreen2 testScreen2;
        private ScreenStack mainMenuScreenStack;
        private MainMenu mainMenu;

        private BackgroundScreen background;
        private ScreenStack backgroundStack;

        [BackgroundDependencyLoader]
        private void load()
        {
            if (!Host.IsPrimaryInstance)
            {
                Logger.Log(@"Piously does not support multiple running instances.", LoggingTarget.Runtime, LogLevel.Error);
                Environment.Exit(0);
            }

            backgroundStack = new ScreenStack();
            piouslyMenuScreenStack = new ScreenStack();
            piouslyMenuScreenStack2 = new ScreenStack();
            mainMenuScreenStack = new ScreenStack();

            AddInternal(new PiouslyKeyBindingContainer
            {
                Children = new[]
                {
                    backgroundStack,
                    piouslyMenuScreenStack,
                    piouslyMenuScreenStack2,
                    mainMenuScreenStack,
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            background = new BackgroundScreen();
            //testScreen = new TestScreen();
            //testScreen2 = new TestScreen2();
            mainMenu = new MainMenu();

            backgroundStack.Push(background);
            //piouslyMenuScreenStack.Push(testScreen);
            //piouslyMenuScreenStack2.Push(testScreen2);
            mainMenuScreenStack.Push(mainMenu);
        }

        protected override void Update()
        {
            base.Update();
            //testScreen.rotateHexagon();
            //testScreen2.rotateText();
        }
        
        //THIS WORKS FINE
        /*protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.A)
                Logger.Log("Detected A");
            if(e.Key == Key.F)
                Logger.Log("Detected F");
            if (e.Key == Key.ControlLeft)
                Logger.Log("Detected ControlLeft");

            return base.OnKeyDown(e);
        }*/

        //THIS DOES NOT
    }
}