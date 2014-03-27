using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BitSits_Framework
{
    class DebugComponent : DrawableGameComponent
    {
        public bool isEnabled = false;

        Camera2D camera = new Camera2D();

        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Point mousePos;

        KeyboardState prevKeyboardState;

        struct StringData
        {
            public Vector2 positon;
            public string s;
            public object[] args;

            public StringData(Vector2 positon, string s, object[] args)
            {
                this.positon = positon;
                this.s = s;
                this.args = args;
            }
        }
        private static List<StringData> stringData = new List<StringData>();

        public DebugComponent(Game game)
            : base(game)
        {
            content = game.Content;
            isEnabled = true;
        }

        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/DebugFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MouseState mouseState = Mouse.GetState();
            mousePos = new Point(mouseState.X, mouseState.Y);

            //if (keyboardState.IsKeyDown(Keys.F1) && prevKeyboardState.IsKeyUp(Keys.F1))
            //    DebugMode = !DebugMode;

            prevKeyboardState = keyboardState;
        }

        public static void DrawString(Vector2 position, string s)
        {
            stringData.Add(new StringData(position, s, null));
        }

        public static void DrawString(Vector2 position, string s, params object[] args)
        {
            stringData.Add(new StringData(position, s, args));
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            float fps = (1000.0f / (float)gameTime.ElapsedGameTime.TotalMilliseconds);

            DrawString(Vector2.Zero, "fps : " + fps.ToString("00") + "\n"
                + "X = " + mousePos.X + " Y = " + mousePos.Y);

            for (int i = 0; i < stringData.Count; i++)
            {
                var text = stringData[i].args == null ? stringData[i].s : string.Format(stringData[i].s, stringData[i].args);
                spriteBatch.DrawString(font, text, stringData[i].positon, Color.White);
            }

            stringData.Clear();

            spriteBatch.End();
        }
    }
}
