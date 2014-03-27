using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class MessageBoxScreen : GameScreen
    {
        #region Fields


        Camera2D camera = new Camera2D();

        const int hPad = 32;
        const int vPad = 16;

        string text;
        SpriteFont font;

        Texture2D texture;

        Vector2 textureOrigin, textOrigin;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        //public event EventHandler<PlayerIndexEventArgs> Cancelled;


        #endregion

        #region Initialization


        public MessageBoxScreen(string message)
        {
            this.text = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        public MessageBoxScreen(Texture2D background, string message)
            : this(message)
        {
            this.texture = background;
        }


        public override void LoadContent()
        {
            font = ScreenManager.GameContent.gameFont;

            if (texture != null)
                textureOrigin = new Vector2(texture.Width, texture.Height) / 2;

            if (text != null)
                textOrigin = font.MeasureString(text) / 2; ;
        }


        #endregion

        #region Handle Input and Draw


        public override void HandleInput(InputState input)
        {
            if (ScreenState != ScreenState.Active) return;

            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex)
                || input.IsMenuCancel(ControllingPlayer, out playerIndex)
                || input.IsMouseLeftButtonClick())
            {
                // Raise the accepted event, then exit the screen.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 1 / 3);

            Vector2 pos = Camera2D.BaseScreenSize / 2;

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            spriteBatch.Draw(ScreenManager.GameContent.splash, Vector2.Zero, color);

            if (texture != null)
                spriteBatch.Draw(texture, pos - textureOrigin, color);

            if (text != null)
            {
                //if (texture == null)
                //    spriteBatch.Draw(ScreenManager.GameContent.blank, new Rectangle((int)(pos - textOrigin).X - hPad,
                //        (int)(pos - textOrigin).Y - vPad, ((int)textOrigin.X + hPad) * 2, ((int)textOrigin.Y + vPad) * 2),
                //        Color.Black * TransitionAlpha);

                spriteBatch.DrawString(font, text, new Vector2(pos.X, 325), Color.Black * TransitionAlpha,
                    0, textOrigin, 24f / ScreenManager.GameContent.gameFontSize, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }


        #endregion
    }
}
