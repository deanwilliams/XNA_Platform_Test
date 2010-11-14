using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformTest
{
    public abstract class Sprite
    {
        protected Texture2D textureImage;
        protected Point frameSize;
        int collisionOffset;
        protected Point currentFrame;
        protected Point sheetSize;
        protected int timeSinceLastFrame = 0;
        protected int millisecondsPerFrame;
        protected Vector2 maxSpeed;
        const int defaultMillisecondsPerFrame = 16;

        public abstract Vector2 Position
        {
            get;
        }
        Vector2 position;

        public abstract Vector2 inputDirection
        {
            get;
        }

        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + collisionOffset,
                    (int)Position.Y + collisionOffset,
                    frameSize.X - (collisionOffset * 2),
                    frameSize.Y - (collisionOffset * 2));
            }
        }

        public int getTimeSinceLastFrame()
        {
            return timeSinceLastFrame;
        }

        /// <summary>
        /// Convenience Constructor
        /// </summary>
        /// <param name="textureImage"></param>
        /// <param name="position"></param>
        /// <param name="frameSize"></param>
        /// <param name="collisionOffset"></param>
        /// <param name="currentFrame"></param>
        /// <param name="sheetSize"></param>
        /// <param name="maxSpeed"></param>
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 maxSpeed)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, maxSpeed, defaultMillisecondsPerFrame)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textureImage"></param>
        /// <param name="position"></param>
        /// <param name="frameSize"></param>
        /// <param name="collisionOffset"></param>
        /// <param name="currentFrame"></param>
        /// <param name="sheetSize"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="millisecondsPerFrame"></param>
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 maxSpeed, int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.maxSpeed = maxSpeed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        /// <summary>
        /// Base Update Method
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="clientBounds"></param>
        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        /// <summary>
        /// Base Draw Method
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, float drawLayer)
        {
            spriteBatch.Draw(textureImage,
                Position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1f, SpriteEffects.None, drawLayer);
        }
    }
}
