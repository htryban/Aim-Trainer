using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Aim_Trainer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        FPSCamera fpsCamera;
        Terrain terrain;
        Rectangle crosshair;
        Texture2D cross;
        SimpleFps fps;
        SpriteFont font;
        List<Bullet> bullets;
        List<Target> targets;
        KeyboardState newKeyboard;
        MouseState newMouseState;
        KeyboardState oldKeyboard;
        MouseState oldMouseState;
        Random rand = new Random();
        float fx;
        float fy;
        float fz;
        int score = 0;
        int apples = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            crosshair.X = graphics.PreferredBackBufferWidth / 2;
            crosshair.Y = graphics.PreferredBackBufferHeight / 2;
            crosshair.Height = 50;
            crosshair.Width = 50;

            bullets = new List<Bullet>();
            targets = new List<Target>();

            for(int i = 0; i < 4; i++)
            {
                targets.Add(new Target(this, randomSpot()));
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fpsCamera = new FPSCamera(this, new Vector3(45, 20, -110));
            Texture2D heightmap = Content.Load<Texture2D>("white-hm");
            cross = Content.Load<Texture2D>("crosshair");
            terrain = new Terrain(this, heightmap, 5f, Matrix.CreateTranslation(-127f, 0, 127));
            fps = new SimpleFps();
            font = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            newKeyboard = Keyboard.GetState();
            newMouseState = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //shooting
            if ((newMouseState.LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Triggers.Right > 0))// && oldMouseState.LeftButton != ButtonState.Pressed)
            {
                bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
            }
            //temp create enemies
            if ((newKeyboard.IsKeyDown(Keys.T) && !oldKeyboard.IsKeyDown(Keys.T)) || (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0))
            {
                targets.Add(new Target(this, randomSpot()));
                apples++;
            }


            fpsCamera.Update(gameTime);
            fps.Update(gameTime);

            if(bullets != null) for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);
                if (bullets[i].isRemoved)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
                else if (bullets[i].position.Y < 5f)
                    {
                        bullets.RemoveAt(i);
                        i--;
                    }
                else
                {
                    for(int j = 0; j < targets.Count; j++)
                    {
                        if(bullets[i].collidesWith(targets[j].model, targets[j].world))
                        {
                            score++;
                            bullets.RemoveAt(i);
                            targets.RemoveAt(j);
                            apples--;
                            i--;
                            break;
                        }
                    }
                }
            }

            if(targets.Count < 4)
            {
                targets.Add(new Target(this, randomSpot()));
            }

            
            //currently unnecessary, maybe for movement later.
            if (targets != null) for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Update(gameTime);
                if (targets[i].isRemoved)
                {
                    targets.RemoveAt(i);
                    i--;
                }
            }
            
            oldKeyboard = newKeyboard;
            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        public Vector3 randomSpot()
        {
            fx = (float)rand.Next(0, 100);
            fy = (float)rand.Next(6, 50);
            fz = (float)rand.Next(-250, -200);
            return new Vector3(fx, fy, fz);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            
            spriteBatch.Begin();
            fps.DrawFps(spriteBatch, font, new Vector2(10f, 10f), Color.MonoGameOrange);
            spriteBatch.DrawString(font, fpsCamera.position.ToString(), new Vector2(1500, 980), Color.White);
            //spriteBatch.DrawString(font, fx.ToString() + ", 75, " + fz.ToString() + " :  " + bullets.Count + " bullets", new Vector2(1500, 880), Color.White);
            spriteBatch.DrawString(font, "score: " + score + " apples: " + targets.Count, new Vector2(1500, 940), Color.White);
            //if (targets.Count > 1 && targets!= null) spriteBatch.DrawString(font, targets[targets.Count-1].position.ToString(), new Vector2(1500, 880), Color.White);
            terrain.Draw(fpsCamera);
            if (targets != null) foreach (var target in targets)
            {
                target.Draw(fpsCamera);
            }

            if (bullets != null) foreach (var bullet in bullets)
            {
                bullet.Draw(fpsCamera);
            }
            
            spriteBatch.Draw(cross, crosshair, null, Color.White, 0, new Vector2(cross.Width / 2f, cross.Height / 2f), SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
