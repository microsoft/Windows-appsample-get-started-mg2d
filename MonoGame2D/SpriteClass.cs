//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame2D
{
    class SpriteClass
    {
        const float HITBOXSCALE = .5f; // experiment with this value to make the collision detection more or less forgiving

        // sprite texture
        public Texture2D texture
        {
            get;
        }

        // x coordinate of the center of the sprite
        public float x
        {
            get;
            set;
        }

        // y coordinate of the center of the sprite
        public float y
        {
            get;
            set;
        }

        // Angle of the sprite around central axis
        public float angle
        {
            get;
            set;
        }

        // Rate of change of x per second
        public float dX
        {
            get;
            set;
        }

        // Rate of change of y per second
        public float dY
        {
            get;
            set;
        }

        // Rate of change of angle per second
        public float dA
        {
            get;
            set;
        }

        // Scale of the texture, where 1 is its true size
        public float scale
        {
            get;
            set;
        }

        // Constructor
        public SpriteClass(GraphicsDevice graphicsDevice, string textureName, float scale)
        {
            this.scale = scale;

            // Load the specified texture
            var stream = TitleContainer.OpenStream(textureName);
            texture = Texture2D.FromStream(graphicsDevice, stream);
        }

        // Update the position and angle of the sprite based on each rate of change and the time elapsed
        public void Update(float elapsedTime)
        {
            this.x += this.dX * elapsedTime;
            this.y += this.dY * elapsedTime;
            this.angle += this.dA * elapsedTime;
        }

        // Draw the sprite with the given SpriteBatch
        public void Draw(SpriteBatch spriteBatch)
        {
            // Determine the position vector of the sprite
            Vector2 spritePosition = new Vector2(this.x, this.y);

            // Draw the sprite
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        // Detect collision between two rectangular sprites
        public bool RectangleCollision(SpriteClass otherSprite)
        {
            if (this.x + this.texture.Width * this.scale * HITBOXSCALE / 2 < otherSprite.x - otherSprite.texture.Width * otherSprite.scale / 2) return false;
            if (this.y + this.texture.Height * this.scale * HITBOXSCALE / 2 < otherSprite.y - otherSprite.texture.Height * otherSprite.scale / 2) return false;
            if (this.x - this.texture.Width * this.scale * HITBOXSCALE / 2 > otherSprite.x + otherSprite.texture.Width * otherSprite.scale / 2) return false;
            if (this.y - this.texture.Height * this.scale * HITBOXSCALE / 2 > otherSprite.y + otherSprite.texture.Height * otherSprite.scale / 2) return false;
            return true;
        }

    }
}
