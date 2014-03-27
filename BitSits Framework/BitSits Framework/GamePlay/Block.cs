using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    enum BlockState
    { Ground, Active, Die, Return }

    class Block
    {
        public static Vector2 BoardPosition;

        public static bool IsActive { get; private set; }

        public bool Select { get; set; }

        public bool IsSolved { get { return position == destPosition; } }

        public bool NotInPlace { get { return position != oriPosition; } }

        public const int Width = 30;
        public const int Height = 30;

        float velocity = 500;

        public bool ShowScore { get; private set; }
        private float time, scoreShowTime = 5f;

        public BlockState State { get; set; }

        GameContent gameContent;
        private Vector2 position, oriPosition, destPosition, maxSlidePos, direction;

        public int BlockNumber { get; private set; }

        public Block(GameContent gameContent, Vector2 position, char number)
        {
            this.position = oriPosition = position;
            BlockNumber = number >= 'a' ? number - 'a' + 10 : number - '0';
            State = BlockState.Ground;
            IsActive = false;

            this.gameContent = gameContent;
        }

        public void SetDirection(int N)
        {
            Point blockIndex = new Point((BlockNumber) % N, (BlockNumber) / N);
            destPosition = BoardPosition + new Vector2(blockIndex.X * Width, blockIndex.Y * Height);

            direction = new Vector2(destPosition.X != position.X ? destPosition.X < position.X ? -1 : 1 : 0,
                destPosition.Y != position.Y ? destPosition.Y < position.Y ? -1 : 1 : 0);

            if (direction.X != 0) { blockIndex = new Point(direction.X < 0 ? 0 : (N - 1), blockIndex.Y); }
            else { blockIndex = new Point(blockIndex.X, direction.Y < 0 ? 0 : (N - 1)); }

            maxSlidePos = BoardPosition + new Vector2(blockIndex.X * Width, blockIndex.Y * Height);
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, Width, Height);
            }
        }

        public Rectangle BoundingRectMouse
        {
            get
            {
                Rectangle a = BoundingRectangle; a.Inflate(Width / 2, Height / 2); 
                return a;
            }
        }

        public void Update(GameTime gameTime)
        {
            ShowScore = false;
            if (State == BlockState.Active)
            {
                Move(gameTime, direction, maxSlidePos);
                if (position == maxSlidePos)
                {
                    State = BlockState.Die;
                    if (maxSlidePos == destPosition) ShowScore = true;
                }
            }
            else if (State == BlockState.Return)
            {
                Move(gameTime, direction * -1, oriPosition);
                if (position == oriPosition) State = BlockState.Ground;
            }

            if (ShowScore) time = scoreShowTime;
        }

        private void Move(GameTime gameTime, Vector2 direction, Vector2 destination)
        {
            IsActive = true;
            position += direction * (int)(velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            float small, big;

            if (direction.X != 0)
            {
                small = position.X; big = destination.X;
                if (direction.X < 0) { big = position.X; small = destination.X; }
            }
            else
            {
                small = position.Y; big = destination.Y;
                if (direction.Y < 0) { big = position.Y; small = destination.Y; }
            }

            if (small >= big) { position = destination; IsActive = false; }
        }

        public void Collision(GameTime gameTime, Rectangle bounds)
        {
            if (State != BlockState.Active) return;

            State = BlockState.Die; IsActive = false;

            if (direction.X != 0) { position.X = bounds.Left - direction.X * Width; }
            else if (direction.Y != 0) { position.Y = bounds.Top - direction.Y * Height; }

            if (IsSolved) ShowScore = true;
            if (ShowScore) time = scoreShowTime;
        }

        public void DrawAtDestination(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.blocks[BlockNumber], destPosition, Color.White * .3f);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.blockBackground, position, Color.White);
            spriteBatch.Draw(gameContent.blocks[BlockNumber], position, Color.CornflowerBlue);

#if WINDOWS
            if (Select) 
#endif
                spriteBatch.Draw(gameContent.blockSelect, position, Color.White);

            if (time > 0)
            {
                time -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                spriteBatch.DrawString(gameContent.blockScoreFont, "+" + BlockNumber.ToString(),
                    destPosition - new Vector2(0, (scoreShowTime - time) * 25), Color.White);
            }
        }
    }
}
