using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PlatformTest
{
    class LevelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteManager spriteManager;

        private Tile[,] tiles;
        private Layer[] layers;

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { //return tiles.GetLength(0);
                return 400; // Debug
            }
        }

        private float cameraPosition;

        // What layer are the sprites drawn on?
        private const int EntityLayer = 2;

        // Entities in the level.
        public UserControlledSprite Player
        {
            get { return spriteManager.Player; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public LevelManager(Game game, SpriteManager spriteManager)
            : base(game)
        {
            // TODO: Construct any child components here
            this.spriteManager = spriteManager;
        }

        /// <summary>
        /// Load Content
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            layers = new Layer[3];
            layers[0] = new Layer(Game, @"Images\Backgrounds\Layer0", 0.2f);
            layers[1] = new Layer(Game, @"Images\Backgrounds\Layer1", 0.5f);
            layers[2] = new Layer(Game, @"Images\Backgrounds\Layer2", 0.8f);

            base.LoadContent();
        }

        /// <summary>
        /// Draw the backgrounds
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            for (int i = 0; i <= EntityLayer; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();

            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, 0.0f, 0.0f);
            
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, cameraTransform);

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Scroll the camera view
        /// </summary>
        /// <param name="viewport"></param>
        private void ScrollCamera(Viewport viewport)
        {
            const float ViewMargin = 0.35f;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (Player.Position.X < marginLeft)
                cameraMovement = Player.Position.X - marginLeft;
            else if (Player.Position.X > marginRight)
                cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.Width * Width - viewport.Width;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }

    }
}
