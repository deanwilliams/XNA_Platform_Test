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

        private const float Acceleration = 20f;
        private const float GroundDragFactor = 5f;

        private const float MaxJumpTime = 0.35f;
        //private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 10f;
        private const float JumpLaunchVelocity = -20f;
        private const float JumpControlPower = 0.14f;
        private const float MaxFallSpeed = 5.0f;

        private float jumpTime;
        private bool isJumping;
        private bool wasJumping;

        private float debugVelocityY = 0;

        private Vector2 resultingForce;

        public override Vector2 inputDirection
        {
            get
            {
                Vector2 controllerDirection = Vector2.Zero;

                if (currentState != PlayerState.Jumping)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        currentState = PlayerState.Running;
                        facingDirecton = FacingDirection.Left;
                        controllerDirection.X -= 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        currentState = PlayerState.Running;
                        facingDirecton = FacingDirection.Right;
                        controllerDirection.X += 1;
                    }
                    else
                    {
                        currentState = PlayerState.Standing;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        currentState = PlayerState.Jumping;
                        isJumping = true;
                        controllerDirection.Y += 1;
                        currentFrame.X = 0;
                    }
                    else
                    {
                        controllerDirection.Y -= 0;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        facingDirecton = FacingDirection.Left;
                        controllerDirection.X -= 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        facingDirecton = FacingDirection.Right;
                        controllerDirection.X += 1;
                    }
                }

                return controllerDirection;
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

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="clientBounds"></param>
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            Vector2 previousPosition = position;
            CalculateCurrentFrame(gameTime);
            ApplyAcceleration(gameTime);
            if (resultingForce.X != 0)
                ApplyFriction(gameTime);
            position += resultingForce;
        }

        /// <summary>
        /// Apply E-MC2
        /// </summary>
        private void ApplyAcceleration(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 velocity = Vector2.Zero;
            
            velocity.X = Acceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            velocity.Y = DoJump(velocity.Y, gameTime);

            resultingForce += (inputDirection * velocity);
            resultingForce.X = MathHelper.Clamp(resultingForce.X, -maxSpeed.X, maxSpeed.X);
        }

        /// <summary>
        /// Do Jump
        /// </summary>
        /// <param name="velocityY"></param>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if (!wasJumping || jumpTime > 0.0f)
                {
                    //if (jumpTime == 0.0f)
                    //    jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                    debugVelocityY = velocityY;
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;
            
            return velocityY;
        }

        /// <summary>
        /// Apply Friction
        /// </summary>
        /// <param name="gameTime"></param>
        private void ApplyFriction(GameTime gameTime)
        {
            Vector2 originalForce = resultingForce;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 friction = Vector2.Zero;
            friction.X = GroundDragFactor * elapsed;

            if (resultingForce.X > 0)
                resultingForce += friction * -1;
            else if (resultingForce.X < 0)
                resultingForce += friction;
            if (resultingForce.X < 0 && originalForce.X > 0 || resultingForce.X > 0 && originalForce.X < 0)
                resultingForce.X = 0;
        }

        public float GetInputDirectionX()
        {
            return inputDirection.X;
        }

        public float GetInputDirectionY()
        {
            return inputDirection.Y;
        }

        public float GetResultingForceX()
        {
            return resultingForce.X;
        }

        public float GetResultingForceY()
        {
            return resultingForce.Y;
        }

        public float GetDebugVelocityY()
        {
            return debugVelocityY;
        }

        public float GetJumpTime()
        {
            return jumpTime;
        }

        public bool GetIsJumping()
        {
            return isJumping;
        }

        public bool GetWasJumping()
        {
            return wasJumping;
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
                    {
                        currentState = PlayerState.Standing;
                        isJumping = false;
                    }
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
