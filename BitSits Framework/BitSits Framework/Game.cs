using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameDataLibrary;

namespace BitSits_Framework
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class BitSitsGames : Microsoft.Xna.Framework.Game
    {
        #region Fields


        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public static ScoreData ScoreData = new ScoreData();
        public static Settings Settings = new Settings();


        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public BitSitsGames()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            Camera2D.BaseScreenSize = new Vector2(480, 480);

            ScoreData = ScoreData.Load();
            Settings = Settings.Load();

#if WINDOWS
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 480;
#endif
#if WINDOWS_PHONE
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;
#endif

            Camera2D.PhoneScale = (float)graphics.PreferredBackBufferWidth / Camera2D.BaseScreenSize.X;

            IsMouseVisible = true;

            // Create the screen manager component.
            screenManager = new ScreenManager(this, graphics);

            Components.Add(screenManager);

#if DEBUG
            Components.Add(new DebugComponent(this));

            // TEST LEVELS
            LoadingScreen.Load(screenManager, false, PlayerIndex.One, new GameplayScreen());
#else
            //LoadingScreen.Load(screenManager, true, null, new QuickMenuScreen());
            LoadingScreen.Load(screenManager, true, null, new BackgroundScreen(), new MainMenuScreen());
#endif
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            ScoreData.Save();
            Settings.Save();

            base.OnExiting(sender, args);
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (BitSitsGames game = new BitSitsGames())
            {
                game.Run();
            }
        }
    }

    #endregion
}
