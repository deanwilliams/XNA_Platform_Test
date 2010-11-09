using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformTest
{
    class UserControlledSprite: Sprite
    {
        public enum PlayerState
        {
            Standing = 4,
            Running = 8,
            Jumping = 9
        }

        public enum FacingDirection
        {
            Right,
            Left
        }

        private PlayerState currentState = PlayerState.Standing;
        private FacingDirection facingDirecton = FacingDirection.Right;

        private const float Acceleration = 500f;
        //private const float GroundDragFactor = 1f;
        private Vector2 DragFactor = new Vector2(0.48f, 1f);

        private Vector2 resultingForce;

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                if (currentState != PlayerState.Jumping)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        currentState = PlayerState.Running;
                        facingDirecton = FacingDirection.Left;
                        inputDirection.X -= 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        currentState = PlayerState.Running;
                        facingDirecton = FacingDirection.Right;
                        inputDirection.X += 1;
                    }
                    else
                    {
                        currentState = PlayerState.Standing;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        currentState = PlayerState.Jumping;
                        currentFrame.X = 0;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        facingDirecton = FacingDirection.Left;
                        inputDirection.X -= 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        facingDirecton = FacingDirection.Right;
                        inputDirection.X += 1;
                    }
                }

                return inputDirection;
            }
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 maxSpeed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, maxSpeed)
        {
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 maxSpeed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, maxSpeed, millisecondsPerFrame)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            CalculateCurrentFrame(gameTime);
            ApplyPhysics(gameTime);
            position += resultingForce;
        }

        /// <summary>
        /// Apply E-MC2
        /// </summary>
        private void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 velocity = Vector2.Zero;

            velocity.X = direction.X * Acceleration * elapsed;
            //velocity.X *= GroundDragFactor;
            if (velocity.X < 0)
                velocity.X = velocity.X * -1;
            resultingForce += direction * velocity;
            resultingForce *= DragFactor;
            resultingForce.X = MathHelper.Clamp(resultingForce.X, -maxSpeed.X, maxSpeed.X);
            resultingForce = new Vector2((float)Math.Round(resultingForce.X), (float)Math.Round(resultingForce.Y));
        }

        public float GetInputDirectionX()
        {
            return direction.X;
        }

        public float GetResultingForceX()
        {
            return resultingForce.X;
        }

        /// <summary>
        /// Calculate the current frame
        /// </summary>
        /// <param name="gameTime"></param>
        private void CalculateCurrentFrame(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;

                switch (currentState)
                {
                    case PlayerState.Standing:
                        currentFrame.Y = 0;
                        sheetSize.X = PlayerState.Standing.GetHashCode();
                        break;
                    case PlayerState.Running:
                        currentFrame.Y = 1;
                        sheetSize.X = PlayerState.Running.GetHashCode();
                        break;
                    case PlayerState.Jumping:
                        currentFrame.Y = 2;
                        sheetSize.X = PlayerState.Jumping.GetHashCode();
                        break;
                }

                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    if (currentState == PlayerState.Jumping)
                        currentState = PlayerState.Standing;
                }
            }
        }

        /// <summary>
        /// Override the draw method so we can flip the sprite based on the facing direction
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (facingDirecton == FacingDirection.Left)
                effect = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1f, effect, 0);
        }
    }
}
