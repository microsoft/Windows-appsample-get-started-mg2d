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
using Microsoft.Xna.Framework.Input;
using System;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

namespace MonoGame2D
{
    public class Game1 : Game
    {
        // The ratio of the screen that is sky versus ground
        const float SKYRATIO = 2f / 3f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int score = 0;

        float screenWidth;
        float screenHeight;
        float broccoliSpeedMultiplier;
        float dinoSpeedX;
        float dinoJumpY;
        float gravitySpeed;

        bool spaceDown;
        bool gameStarted;
        bool gameOver;

        Texture2D grass;
        Texture2D startGameSplash;
        Texture2D gameOverTexture;

        SpriteClass dino;
        SpriteClass broccoli;
        
        Random random;

        SpriteFont scoreFont;
        SpriteFont stateFont;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";  // Set the directory where game assets can be found by the ContentManager
        }


        // Give variables their initial states
        // Called once when the app is started
        protected override void Initialize()
        {
            base.Initialize();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen; // Attempt to launch in fullscreen mode

            // Get screen height and width, scaling them up if running on a high-DPI monitor.
            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            broccoliSpeedMultiplier = 0.5f;

            spaceDown = false;
            gameStarted = false;
            gameOver = false;

            random = new Random();

            dinoSpeedX = ScaleToHighDPI(1000f);
            dinoJumpY = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
            score = 0;

            this.IsMouseVisible = false; // Hide the mouse within the app window

        }


        // Load content (eg.sprite textures) before the app runs
        // Called once when the app is started
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            grass = Content.Load<Texture2D>("grass");
            startGameSplash = Content.Load<Texture2D>("start-splash");
            gameOverTexture = Content.Load<Texture2D>("game-over");

            // Construct SpriteClass objects
            dino = new SpriteClass(GraphicsDevice, "Content/ninja-cat-dino.png", ScaleToHighDPI(1f));
            broccoli = new SpriteClass(GraphicsDevice, "Content/broccoli.png", ScaleToHighDPI(0.2f));

            // Load fonts
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");
        }


        // Unloads any non ContentManager content
        protected override void UnloadContent()
        {
        }


        // Updates the logic of the game state each frame, checking for collision, gathering input, etc.
        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration

            KeyboardHandler(); // Handle keyboard input

            // Stop all movement when the game ends
            if (gameOver)
            {
                dino.dX = 0;
                dino.dY = 0;
                broccoli.dX = 0;
                broccoli.dY = 0;
                broccoli.dA = 0;
            }

            // Update animated SpriteClass objects based on their current rates of change
            dino.Update(elapsedTime);
            broccoli.Update(elapsedTime);

            // Accelerate the dino downward each frame to simulate gravity.
            dino.dY += gravitySpeed;

            // Set game floor
            if (dino.y > screenHeight * SKYRATIO)
            {
                dino.dY = 0;
                dino.y = screenHeight * SKYRATIO;
            }

            // Set right edge
            if (dino.x > screenWidth - dino.texture.Width / 2)
            {
                dino.x = screenWidth - dino.texture.Width / 2;
                dino.dX = 0;
            }

            // Set left edge
            if (dino.x < 0 + dino.texture.Width / 2)
            {
                dino.x = 0 + dino.texture.Width / 2;
                dino.dX = 0;
            }

            // If the broccoli goes offscreen, spawn a new one and iterate the score
            if (broccoli.y > screenHeight + 100 || broccoli.y < -100 || broccoli.x > screenWidth + 100 || broccoli.x < -100)
            {
                SpawnBroccoli();
                score++;
            }

            if (dino.RectangleCollision(broccoli)) gameOver = true; // End game if the dino collides with the broccoli

            base.Update(gameTime);
        }


        // Draw the updated game state each frame
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the screen

            spriteBatch.Begin(); // Begin drawing

            // Draw grass
            spriteBatch.Draw(grass, new Rectangle(0, (int)(screenHeight * SKYRATIO), (int)screenWidth, (int)screenHeight), Color.White);

            if (gameOver)
            {
                // Draw game over texture
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, screenHeight / 4 - gameOverTexture.Width / 2), Color.White);

                String pressEnter = "Press Enter to restart!";

                // Measure the size of text in the given font
                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, screenHeight - 200), Color.White);

                // If the game is over, draw the score in red
                spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, 50), Color.Red);
            }

            // If the game is not over, draw it in black
            else spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, 50), Color.Black);

            // Draw broccoli and dino with the SpriteClass method
            broccoli.Draw(spriteBatch);
            dino.Draw(spriteBatch);

            if (!gameStarted)
            {
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "VEGGIE JUMP";
                String pressSpace = "Press Space to start";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.ForestGreen);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
            }

            spriteBatch.End(); // Stop drawing

            base.Draw(gameTime);
        }


        // Scale a number of pixels so that it displays properly on a High-DPI screen, such as a Surface Pro or Studio
        public float ScaleToHighDPI(float f)
        {
            DisplayInformation d = DisplayInformation.GetForCurrentView();
            f *= (float)d.RawPixelsPerViewPixel;
            return f;
        }


        // Spawn the broccoli object in a random location offscreen
        public void SpawnBroccoli()
        {
            // Spawn broccoli either left (1), above (2), right (3), or below (4) the screen
            int direction = random.Next(1, 5);
            switch (direction)
            {
                case 1:
                    broccoli.x = -100;
                    broccoli.y = random.Next(0, (int)screenHeight);
                    break;
                case 2:
                    broccoli.y = -100;
                    broccoli.x = random.Next(0, (int)screenWidth);
                    break;
                case 3:
                    broccoli.x = screenWidth + 100;
                    broccoli.y = random.Next(0, (int)screenHeight);
                    break;
                case 4:
                    broccoli.y = screenHeight + 100;
                    broccoli.x = random.Next(0, (int)screenWidth);
                    break;
            }

            if (score % 5 == 0) broccoliSpeedMultiplier += 0.2f; // Increase game difficulty (ie broccoli speed) for every five points scored

            // Orient the broccoli sprite towards the dino sprite and set angular velocity
            broccoli.dX = (dino.x - broccoli.x) * broccoliSpeedMultiplier;
            broccoli.dY = (dino.y - broccoli.y) * broccoliSpeedMultiplier;
            broccoli.dA = 7f;
        }


        // Start a new game, either when the app starts up or after game over
        public void StartGame()
        {
            // Reset dino position
            dino.x = screenWidth / 2;
            dino.y = screenHeight * SKYRATIO;

            // Reset broccoli speed and respawn it
            broccoliSpeedMultiplier = 0.5f;
            SpawnBroccoli();

            score = 0; // Reset score
        }


        // Handle user input from the keyboard
        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();
            
            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            // Exit the keyboard handler method early, preventing the dino from jumping on the same keypress.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
                }
                return;
            }

            // Restart the game if Enter is pressed
            if (gameOver)
            {
                if (state.IsKeyDown(Keys.Enter))
                {
                    StartGame();
                    gameOver = false;
                }
            }

            // Jump if Space (or another jump key) is pressed
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
            {
                // Jump if Space is pressed but not held and the dino is on the floor
                if (!spaceDown && dino.y >= screenHeight * SKYRATIO - 1) dino.dY = dinoJumpY;
            
                spaceDown = true;
            }
            else spaceDown = false;

            // Handle left and right
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A)) dino.dX = dinoSpeedX * -1;
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D)) dino.dX = dinoSpeedX;
            else dino.dX = 0;
        }
    }
}
