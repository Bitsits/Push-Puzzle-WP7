using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace BitSits_Framework
{
    class Level : IDisposable
    {
        #region Fields


        public int Score { get; private set; }

        public int Bonus { get { return (int)Math.Floor(timeBonus); } }
        float timeBonus;

        public bool IsLevelUp { get; private set; }
        public bool ReloadLevel { get; private set; }
        int levelIndex;

        GameContent gameContent;

        private List<Block> blocks = new List<Block>();

        MouseState mouseState, prevMouseState;
        Point mousePos;

        List<List<int>> connectedBlocks = new List<List<int>>();

        Rectangle resetRect;
        bool isResetSelect;

        float scoreTime, maxScoreTime = 5f;

        bool reduceScore;
        int tempScore, reducedScore;

        int numberOfBlockSolved;


        #endregion

        #region Initialization


        public Level(ScreenManager screenManager, int levelIndex)
        {
            this.gameContent = screenManager.GameContent;
            this.levelIndex = levelIndex;

            LoadTiles(levelIndex);

            if (blocks.Count == 4) timeBonus = 10;
            if (blocks.Count == 9) timeBonus = 20;
            if (blocks.Count == 16) timeBonus = 30;
        }


        private void LoadTiles(int levelIndex)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            lines = gameContent.content.Load<List<string>>("Levels/" + levelIndex.ToString("00"));

            width = lines[0].Length;
            // Loop over every tile position,
            for (int y = 0; y < lines.Count; ++y)
            {
                if (lines[y].Length != width)
                    throw new Exception(String.Format(
                        "The length of line {0} is different from all preceeding lines.", lines.Count));

                for (int x = 0; x < lines[0].Length; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    LoadTile(tileType, x, y);
                }
            }

            SetBlocks();
        }


        private void SetBlocks()
        {
            int N = (int)Math.Sqrt(blocks.Count);
            foreach (Block block in blocks) block.SetDirection(N);

            for (int i = 0; i < blocks.Count; i++)
            {
                connectedBlocks.Add(new List<int>());

                for (int j = 0; j < blocks.Count; j++)
                {
                    if (i != j && blocks[i].BoundingRectMouse.Intersects(blocks[j].BoundingRectMouse))
                    {
                        connectedBlocks[i].Add(j);
                    }
                }
            }

            for (int i = 0; i < connectedBlocks.Count; i++)
            {
                for (int j = 0; j < connectedBlocks[i].Count; j++)
                {
                    for (int k = 0; k < connectedBlocks[connectedBlocks[i][j]].Count; k++)
                    {
                        int number = connectedBlocks[connectedBlocks[i][j]][k];
                        if (!connectedBlocks[i].Contains(number) && number != i)
                            connectedBlocks[i].Add(number);
                    }
                }
            }
        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private void LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    blocks.Add(new Block(gameContent, GetPosition(x, y), tileType)); return;

                case 'X':
                    Block.BoardPosition = GetPosition(x, y); return;

                case 'R':
                    {
                        Vector2 resetPos = GetPosition(x, y);
                        
                        resetRect = new Rectangle((int)resetPos.X, (int)resetPos.Y,
                            gameContent.reset.Width, gameContent.reset.Height);
                        return;
                    }
            }
        }


        public Vector2 GetPosition(int x, int y)
        {
            Rectangle rect = new Rectangle(x * Block.Width, y * Block.Height, Block.Width, Block.Height);
            return new Vector2(rect.Left, rect.Top);
        }


        public void Dispose() { }


        #endregion

        #region Update and HandleInput


        public void Update(GameTime gameTime)
        {
            timeBonus = Math.Max(timeBonus - (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

            isResetSelect = false;
            if (resetRect.Contains(mousePos))
            {
                isResetSelect = true;
                if (prevMouseState.LeftButton == ButtonState.Released &&
                    mouseState.LeftButton == ButtonState.Pressed)
                {
                    gameContent.resetSound.Play();

                    numberOfBlockSolved = 0;

                    reduceScore = false;
                    foreach (Block block in blocks)
                    {
                        block.State = BlockState.Return;
                        if (block.NotInPlace) reduceScore = true;
                    }
                }
            }

            foreach (Block block in blocks)
            {
                block.Select = false; 
                block.Update(gameTime);
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                //blocks[i].Update(gameTime);

                // Draw Select box when in Ground
                if (blocks[i].BoundingRectMouse.Contains(mousePos) && blocks[i].State == BlockState.Ground)
                {
                    blocks[i].Select = true;
                    for (int j = 0; j < connectedBlocks[i].Count; j++)
                    {
                        blocks[connectedBlocks[i][j]].Select = true;
                    }
                }

                if (prevMouseState.LeftButton == ButtonState.Released &&
                    mouseState.LeftButton == ButtonState.Pressed)
                {
                    // Activate
                    if (blocks[i].State == BlockState.Ground && blocks[i].BoundingRectMouse.Contains(mousePos)
                        && !Block.IsActive)
                    {
                        blocks[i].State = BlockState.Active;

                        gameContent.hitSound[gameContent.random.Next(3)].Play();

                        for (int j = 0; j < connectedBlocks[i].Count; j++)
                        {
                            blocks[connectedBlocks[i][j]].State = BlockState.Active;
                        }
                    }
                }

                // Collision Check
                foreach (Block block2 in blocks)
                    if (blocks[i] != block2 && blocks[i].BoundingRectangle.Intersects(block2.BoundingRectangle) &&
                        blocks[i].State == BlockState.Active && block2.State == BlockState.Die)
                        blocks[i].Collision(gameTime, block2.BoundingRectangle);
            }

            //IsLevelUp = true;
            foreach (Block block in blocks)
            {
                //if (!block.IsSolved) IsLevelUp = false;

                if (block.ShowScore)
                {
                    tempScore += block.BlockNumber; Score += block.BlockNumber;
                    numberOfBlockSolved += 1;
                }
            }

            if (numberOfBlockSolved == blocks.Count) IsLevelUp = true;

            if (reduceScore)
            {
                reducedScore = tempScore + 10;
                tempScore = 0;
                Score -= reducedScore; reduceScore = false; scoreTime = 0;

                scoreTime = maxScoreTime;
            }
        }


        public void HandleInput(InputState input, int playerIndex)
        {
            prevMouseState = input.LastMouseState;
            mouseState = input.CurrentMouseState;
            mousePos = new Point(mouseState.X, mouseState.Y);
        }


        #endregion

        #region Draw


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.menuBackground, Vector2.Zero, Color.White);
            spriteBatch.Draw(gameContent.gameplayBG, Vector2.Zero, Color.White);

            if (!IsLevelUp && levelIndex == 0)
                spriteBatch.Draw(gameContent.tutorialBG, Vector2.Zero, Color.White);

            foreach (Block block in blocks) block.DrawAtDestination(gameTime, spriteBatch);

            foreach (Block block in blocks) block.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(gameContent.grid[(int)Math.Pow(blocks.Count, 0.5f) - 2], 
                Block.BoardPosition, Color.White);

            spriteBatch.Draw(gameContent.reset, resetRect, Color.White);

#if WINDOWS
            if (isResetSelect)
#endif
                spriteBatch.Draw(gameContent.resetSelect, resetRect, Color.White);

            if (scoreTime > 0)
            {
                scoreTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                spriteBatch.DrawString(gameContent.blockScoreFont, "-" + reducedScore.ToString(),
                    new Vector2(resetRect.X, resetRect.Y) - new Vector2(0, (maxScoreTime - scoreTime) * 25),
                    Color.White);
            }
        }


        #endregion
    }
}
