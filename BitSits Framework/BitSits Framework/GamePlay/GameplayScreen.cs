using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace BitSits_Framework
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields


        GameContent gameContent;

        Camera2D camera = new Camera2D();

        public int Score { get { return prevScore + level.Score; } }
        int prevScore;
        float tempScore; 

        // Meta-level game state.
        const int MaxLevelIndex = 20;    //Number of Levels
        int levelIndex;
        Level level;
        
        MessageBoxScreen m;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            this.gameContent = ScreenManager.GameContent;

            levelIndex = BitSitsGames.ScoreData.CurrentLevel;
            tempScore = 0;
            prevScore = BitSitsGames.ScoreData.PrevScore;

            LoadNextLevel();
        }


        #endregion

        #region Update and Handle Input


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (m == null)
            {
                if (level.IsLevelUp)
                {
                    m = new MessageBoxScreen(gameContent.levelUp, "Total = " + prevScore
                        + " + " + (level.Score + level.Bonus) + " = " + (Score + level.Bonus));
                    BitSitsGames.ScoreData.CurrentLevel = levelIndex;
                    BitSitsGames.ScoreData.PrevScore = Score;

                    if (levelIndex == MaxLevelIndex)
                    {
                        BitSitsGames.ScoreData.CurrentLevel = 0;
                        BitSitsGames.ScoreData.PrevScore = 0;
                        bool newHighScore = BitSitsGames.ScoreData.SetHighScore(Score + level.Bonus);

                        m = new MessageBoxScreen(newHighScore ? gameContent.gameOverNewHigh
                            : gameContent.gameOver, "Total = " + (Score + level.Bonus) + (newHighScore ? " **" : ""));
                    }

                    m.Accepted += MessageBoxAccepted;
                    ScreenManager.AddScreen(m, ControllingPlayer);
                }
                else if (level.ReloadLevel)
                {
                    m = new MessageBoxScreen("Reload Level!");
                    m.Accepted += MessageBoxAccepted;
                    ScreenManager.AddScreen(m, ControllingPlayer);
                }
            }

            if (IsActive) level.Update(gameTime);
        }


        void MessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            if (level.IsLevelUp)
            {
                if (levelIndex == MaxLevelIndex)
                {
                    //LoadingScreen.Load(ScreenManager, false, null, new QuickMenuScreen());
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
                    return;
                }

                LoadNextLevel();
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
                if (level.ReloadLevel) ReloadCurrentLevel();
        }


        private void LoadNextLevel()
        {
            // Unloads the content for the current level before loading the next one.
            if (level != null)
            {
                // Update Score
                prevScore += level.Score + level.Bonus; tempScore = 0;

                level.Dispose();
            }            

            // Load the level.
            level = new Level(ScreenManager, levelIndex); levelIndex += 1;
            m = null;
        }


        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                level.HandleInput(input, playerIndex);
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            level.Draw(gameTime, spriteBatch);

            DrawScore(gameTime, spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(1 - TransitionAlpha);
        }


        private void DrawScore(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float rate = (float)gameTime.ElapsedGameTime.TotalSeconds * 50;

            if (tempScore < level.Score) tempScore = Math.Min(level.Score, tempScore + rate);
            else if (tempScore > level.Score) tempScore = Math.Max(level.Score, tempScore - rate);

            spriteBatch.DrawString(gameContent.scoreFont, "Score", new Vector2(20, 20), Color.White, 0,
                Vector2.Zero, .55f, SpriteEffects.None, 1);            

            spriteBatch.DrawString(gameContent.scoreFont, tempScore.ToString("000"), new Vector2(20, 50), Color.White);


            spriteBatch.DrawString(gameContent.scoreFont, "Level", new Vector2(370, 20), Color.White, 0,
                Vector2.Zero, .55f, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.scoreFont, levelIndex.ToString(), new Vector2(385, 50), Color.White);


            spriteBatch.DrawString(gameContent.scoreFont, "Bonus", new Vector2(370, 375), Color.White, 0,
                Vector2.Zero, .55f, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.scoreFont, level.Bonus.ToString(), new Vector2(385, 410), Color.White);
        }
        

        #endregion
    }
}
