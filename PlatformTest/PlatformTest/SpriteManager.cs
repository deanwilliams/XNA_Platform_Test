using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace PlatformTest
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteFont debugFont;

        SpriteBatch spriteBatch;
        UserControlledSprite player;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            //player = new UserControlledSprite(
            //    Game.Content.Load<Texture2D>(@"player"),
            //    Vector2.Zero, new Point(40, 48), 10, new Point(0, 0),
            //    new Point(6, 8), new Vector2(6, 6), 60);

            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>(@"Images\player"),
                new Vector2(35, 337), new Point(40, 48), 10, new Point(0, 0),
                new Point(6, 8), new Vector2(6, 6), 60);

            debugFont = Game.Content.Load<SpriteFont>(@"Fonts\arial");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            player.Update(gameTime, Game.Window.ClientBounds);

            base.Update(gameTime);
        }

        /// <summary>
        /// Base Draw Method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            player.Draw(gameTime, spriteBatch);

            //spriteBatch.DrawString(debugFont, "Velocity x = " + player.GetVelocityX(), new Vector2(10, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            //spriteBatch.DrawString(debugFont, "Direction x = " + player.GetVelocityX(), new Vector2(10, 30), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
