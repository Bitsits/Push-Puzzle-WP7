using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameDataLibrary;

#if WINDOWS_PHONE
using System.Windows;
#endif

namespace BitSits_Framework
{
    /// <summary>
    /// All the Contents of the Game is loaded and stored here
    /// so that all other screen can copy from here
    /// </summary>
    public class GameContent
    {
        public ContentManager content;
        
        public Random random = new Random();

        // Textures
        public Texture2D blank, gradient;
        public Texture2D menuBackground, mainMenuTitle, gameplayBG, gameOver, gameOverNewHigh, levelUp, splash;
        public Texture2D tutorialBG, highScores, options, paused;

        public Texture2D[] blocks = new Texture2D[16], grid = new Texture2D[3];
        public Texture2D reset, resetSelect, blockSelect, blockBackground;

        // Fonts
        public SpriteFont debugFont, gameFont, scoreFont, blockScoreFont;
        public int gameFontSize;

        public SoundEffect[] hitSound = new SoundEffect[3];
        public SoundEffect resetSound;


        /// <summary>
        /// Load GameContents
        /// </summary>
        public GameContent(GameComponent screenManager)
        {
            content = screenManager.Game.Content;

            blank = content.Load<Texture2D>("Graphics/blank");
            gradient = content.Load<Texture2D>("Graphics/gradient");
            menuBackground = content.Load<Texture2D>("Graphics/menuBackground");

            mainMenuTitle = content.Load<Texture2D>("Graphics/mainMenuTitle");
            highScores = content.Load<Texture2D>("Graphics/highScores");
            options = content.Load<Texture2D>("Graphics/options");
            paused = content.Load<Texture2D>("Graphics/paused");

            splash = content.Load<Texture2D>("Graphics/splash");
            gameplayBG = content.Load<Texture2D>("Graphics/gameplayBackground");
            gameOver = content.Load<Texture2D>("Graphics/gameOver");
            gameOverNewHigh = content.Load<Texture2D>("Graphics/gameOverNewHigh");
            levelUp = content.Load<Texture2D>("Graphics/levelUp");

            tutorialBG = content.Load<Texture2D>("Graphics/tutorialBG");

            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = content.Load<Texture2D>("Graphics/block" + i.ToString("00"));

            blockSelect = content.Load<Texture2D>("Graphics/blockselect");
            blockBackground = content.Load<Texture2D>("Graphics/blockBackground");
            reset = content.Load<Texture2D>("Graphics/reset");
            resetSelect = content.Load<Texture2D>("Graphics/resetSelect");

            for (int i = 0; i < grid.Length; i++)
                grid[i] = content.Load<Texture2D>("Graphics/grid" + (i + 2) * (i + 2));

            debugFont = content.Load<SpriteFont>("Fonts/debugFont");

            gameFontSize = 48;
            gameFont = content.Load<SpriteFont>("Fonts/theBubbleLetters" + gameFontSize.ToString());

            scoreFont = content.Load<SpriteFont>("Fonts/scoreFont");
            blockScoreFont = content.Load<SpriteFont>("Fonts/blockScoreFont");

            for (int i = 0; i < hitSound.Length; i++)
                hitSound[i] = content.Load<SoundEffect>("Audio/hit" + i);

            resetSound = content.Load<SoundEffect>("Audio/reset");

            MediaPlayer.IsRepeating = true;

#if DEBUG
            MediaPlayer.Volume = .4f; SoundEffect.MasterVolume = .4f;
#else
            if (BitSitsGames.Settings.MusicEnabled) PlayMusic();

            if (BitSitsGames.Settings.SoundEnabled) SoundEffect.MasterVolume = 1;
            else SoundEffect.MasterVolume = 0;
#endif

            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();
        }


        public void PlayMusic()
        {
            if (MediaPlayer.GameHasControl)
            {
                if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();

                    return;
                }
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
#if WINDOWS_PHONE
                MessageBoxResult Choice;

                Choice = MessageBox.Show("Media is currently playing, do you want to stop it?",
                    "Stop Player", MessageBoxButton.OKCancel);

                if (Choice == MessageBoxResult.OK) MediaPlayer.Pause();
                else
                {
                    BitSitsGames.Settings.MusicEnabled = false;
                    return;
                }
#endif
            }

            MediaPlayer.Play(content.Load<Song>("Audio/Back to old school"));
            MediaPlayer.IsRepeating = true;
        }


        /// <summary>
        /// Unload GameContents
        /// </summary>
        public void UnloadContent() { content.Unload(); }
    }
}
