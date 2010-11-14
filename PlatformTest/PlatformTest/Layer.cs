﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace PlatformTest
{
    /// <summary>
    /// For layering the background
    /// </summary>
    class Layer
    {
        public Texture2D[] Textures { get; private set; }
        public float ScrollRate { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content"></param>
        /// <param name="basePath"></param>
        /// <param name="scrollRate"></param>
        public Layer(Game game, string basePath, float scrollRate)
        {
            // Assumes each layer only has 3 segments.
            Textures = new Texture2D[3];
            for (int i = 0; i < 3; ++i)
                Textures[i] = game.Content.Load<Texture2D>(basePath + "_" + i);

            ScrollRate = scrollRate;
        }

        /// <summary>
        /// Draw Method
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        public void Draw(SpriteBatch spriteBatch, float cameraPosition, int drawLayer)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x, 0.0f), Color.White);
            spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, 0.0f), Color.White);
        }

    }
}
