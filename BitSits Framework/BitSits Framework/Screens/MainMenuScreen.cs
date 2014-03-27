using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base() { }


        public override void LoadContent()
        {
            titleTexture = ScreenManager.GameContent.mainMenuTitle;

            // Create our menu entries.
            MenuEntry contiGameMenuEntry = new MenuEntry(this, "Continue", new Vector2(160, 220));
            MenuEntry newGameMenuEntry = new MenuEntry(this, "New Game", new Vector2(160, 270));
            MenuEntry highScoreMenuEntry = new MenuEntry(this, "High Scores", new Vector2(160, 320));
            MenuEntry optionsMenuEntry = new MenuEntry(this, "Options", new Vector2(160, 370));
            MenuEntry exitMenuEntry = new MenuEntry(this, "Exit", new Vector2(160, 420));

            // Hook up menu event handlers.
            contiGameMenuEntry.Selected += ContiGameMenuEntrySelected;
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            highScoreMenuEntry.Selected += HighScoresMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            if (BitSitsGames.ScoreData.CurrentLevel != 0) MenuEntries.Add(contiGameMenuEntry);

            MenuEntries.Add(newGameMenuEntry);
            MenuEntries.Add(highScoreMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            BitSitsGames.ScoreData.CurrentLevel = 0;
            BitSitsGames.ScoreData.PrevScore = 0;

            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                               new GameplayScreen());
        }


        void ContiGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                               new GameplayScreen());
        }


        void HighScoresMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new HighScoresScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
#if WINDOWS_PHONE
            ScreenManager.Game.Exit();
#endif
        }


        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
