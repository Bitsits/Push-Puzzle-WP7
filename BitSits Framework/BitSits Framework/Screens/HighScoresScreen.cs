using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class HighScoresScreen : MenuScreen
    {
        GameContent gameContent;
        SpriteBatch spriteBatch;
        List<int> highScores;


        public override void LoadContent()
        {
            gameContent = ScreenManager.GameContent;

            //titleString = "High\nScores";
            titleTexture = gameContent.highScores;
            //titlePosition = new Vector2(20, 20);

            highScores = BitSitsGames.ScoreData.HighScores;
            spriteBatch = ScreenManager.SpriteBatch;

            MenuEntry exitMenuEntry = new MenuEntry(this, "Back", new Vector2(60, 360));

            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(exitMenuEntry);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            for (int i = 0; i < highScores.Count; i++)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(gameContent.gameFont, highScores[i].ToString(),
                    new Vector2(300, 200 + i * 70), Color.Black * TransitionAlpha, 0, 
                    Vector2.Zero, 40f / gameContent.gameFontSize, SpriteEffects.None, 1);
                spriteBatch.End();
            }
        }
    }
}
