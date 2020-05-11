using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        Rectangle crosshair;
        Texture2D cross;
        SimpleFps fps;
        SpriteFont font;
        List<Bullet> bullets;
        List<Target> targets;
        List<Wall> walls;
        Floor floor;
        Floor ceiling;
        List<SoundEffect> sounds;

        KeyboardState newKeyboard;
        MouseState newMouseState;
        KeyboardState oldKeyboard;
        MouseState oldMouseState;
        GamePadState newGame;
        GamePadState oldGame;

        Random rand = new Random();
        float fx;
        float fy;
        float fz;
        double score;
        double bulletsFired;
        int fireRate;
        float _timer;
        float checktime;
        float shootTimer;
        string firingMode;
        double accuracy;
        int gamemode;

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
            walls = new List<Wall>();

            sounds = new List<SoundEffect>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fpsCamera = new FPSCamera(this, new Vector3(45, 20, -110));//20
            cross = Content.Load<Texture2D>("crosshair");
            fps = new SimpleFps();
            font = Content.Load<SpriteFont>("font");
            fireRate = 3;
            bulletsFired = 0;
            shootTimer = 0;
            checktime = 0;
            gamemode = 5;

            //walls referenced as if loading into the game without turning or moving
            walls.Add(new Wall(this, new Vector3(0, -30, -345), MathHelper.Pi)); //back left
            walls.Add(new Wall(this, new Vector3(200, -30, -345), MathHelper.Pi)); //back right
            walls.Add(new Wall(this, new Vector3(100, -30, -345), MathHelper.Pi)); //back middle
            walls.Add(new Wall(this, new Vector3(235, -30, -208), MathHelper.PiOver2)); //right 3
            walls.Add(new Wall(this, new Vector3(235, -30, -108), MathHelper.PiOver2)); //right 2
            walls.Add(new Wall(this, new Vector3(235, -30, -8), MathHelper.PiOver2)); //right 1
            walls.Add(new Wall(this, new Vector3(-140, -30, -310), MathHelper.Pi + MathHelper.PiOver2)); //left 3
            walls.Add(new Wall(this, new Vector3(-140, -30, -210), MathHelper.Pi + MathHelper.PiOver2)); //left 2
            walls.Add(new Wall(this, new Vector3(-140, -30, -110), MathHelper.Pi + MathHelper.PiOver2)); //left 1
            walls.Add(new Wall(this, new Vector3(95, -30, 30), 0)); //back right
            walls.Add(new Wall(this, new Vector3(-105, -30, 30), 0)); //back left
            walls.Add(new Wall(this, new Vector3(-5, -30, 30), 0)); //back middle

            floor = new Floor(this, new Vector3(58,1,-84), 0, false); 
            ceiling = new Floor(this, new Vector3(58, 80, -84), 0, true);

            sounds.Add(Content.Load<SoundEffect>("singleshot"));
            sounds.Add(Content.Load<SoundEffect>("AR15single"));
            sounds.Add(Content.Load<SoundEffect>("pistol"));
            sounds.Add(Content.Load<SoundEffect>("supressed"));
            sounds.Add(Content.Load<SoundEffect>("deagle"));
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
            newGame = GamePad.GetState(PlayerIndex.One);
            checktime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //shooting
            if (fireRate == 0)
            {
                firingMode = "Single Fire";
                if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && (oldMouseState.LeftButton != ButtonState.Pressed && oldGame.Triggers.Right == 0))
                {
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[2].CreateInstance();
                    instance.Play();
                }
            }
            else if (fireRate == 1)
            {
                firingMode = "300 RPM";
                if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && checktime - shootTimer > .2)
                {
                    shootTimer = checktime;
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[1].CreateInstance();
                    instance.Play();
                }
            }
            else if (fireRate == 2)
            {
                firingMode = "600 RPM";
                if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && checktime - shootTimer > .10)
                {
                    shootTimer = checktime;
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[0].CreateInstance();
                    instance.Play();
                }
            }
            else if (fireRate == 3)
            {
                firingMode = "800 RPM";
                if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && checktime - shootTimer > .075)
                {
                    shootTimer = checktime;
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[4].CreateInstance();
                    instance.Play();
                }
            }
            else if (fireRate == 4)
            {
                firingMode = "980 RPM";
                if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && checktime - shootTimer > .06)
                {
                    shootTimer = checktime;
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[3].CreateInstance();
                    instance.Play();
                }
            }
            else
            {
                firingMode = "Buzzsaw";
                if (newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0)
                {
                    bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                    bulletsFired++;
                    var instance = sounds[1].CreateInstance();
                    instance.Play();
                }
            }


            //manually create enemies
            /*if ((newKeyboard.IsKeyDown(Keys.T) && !oldKeyboard.IsKeyDown(Keys.T)) || (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0))
            {
                targets.Add(new Target(this, randomSpot()));
            }*/

            if ((newKeyboard.IsKeyDown(Keys.Right) && !oldKeyboard.IsKeyDown(Keys.Right)) || (newGame.DPad.Right == ButtonState.Pressed && oldGame.DPad.Right == ButtonState.Released))
                if (fireRate < 5) fireRate++;
            if ((newKeyboard.IsKeyDown(Keys.Left) && !oldKeyboard.IsKeyDown(Keys.Left)) || (newGame.DPad.Left == ButtonState.Pressed && oldGame.DPad.Left == ButtonState.Released)) 
                if (fireRate > 0) fireRate--;

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
                            i--;
                            break;
                        }
                    }
                }
            }

            //difficulties/gamemodes
            if (newKeyboard.IsKeyDown(Keys.NumPad1) || newKeyboard.IsKeyDown(Keys.D1)) gamemode = 6; // easy
            if (newKeyboard.IsKeyDown(Keys.NumPad2) || newKeyboard.IsKeyDown(Keys.D2)) gamemode = 5; // normal
            if (newKeyboard.IsKeyDown(Keys.NumPad3) || newKeyboard.IsKeyDown(Keys.D3)) gamemode = 3; // hard
            if (newKeyboard.IsKeyDown(Keys.NumPad4) || newKeyboard.IsKeyDown(Keys.D4)) gamemode = 1; // fruit sniper

            //replacing targets that are shot or expire
            if (targets.Count < 4)
            {
                targets.Add(new Target(this, randomSpot(), randomFruit(gamemode), gamemode));
            }

            if (targets != null) for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Update(gameTime);
                if (targets[i].isRemoved)
                {
                    targets.RemoveAt(i);
                    i--;
                }
            }

            if (bulletsFired > 0) accuracy = score / bulletsFired;
            oldKeyboard = newKeyboard;
            oldMouseState = newMouseState;
            oldGame = newGame;
            base.Update(gameTime);
        }

        public Vector3 randomSpot()
        {
            
            
            fx = (float)rand.Next(0, 100);
            if (gamemode == 1)
            {
                fy = 60;
            }
            else fy = (float)rand.Next(6, 40);
            fz = (float)rand.Next(-250, -200);
            return new Vector3(fx, fy, fz);
        }

        public string randomFruit(int i)
        {
            int y = 1;
            if (i == 6)
            {
                y = 3;
                i = 5;
            }
            else if (i == 1) i = 5;
            int x = rand.Next(y, i);
            if (x == 1) return "apple";
            else if (x == 2) return "Orange";
            else if (x == 3) return "pumpkin";
            else return "melon";
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            
            spriteBatch.Begin();

            floor.Draw(fpsCamera);
            ceiling.Draw(fpsCamera);

            if (walls!= null) foreach (var wall in walls)
                {
                    wall.Draw(fpsCamera);
                }

            fps.DrawFps(spriteBatch, font, new Vector2(10f, 10f), Color.White);
            spriteBatch.DrawString(font, "Fire Rate: " + firingMode, new Vector2(1680, 10), Color.White);
            //spriteBatch.DrawString(font, fpsCamera.position.ToString(), new Vector2(1500, 980), Color.White);
            spriteBatch.DrawString(font, "accuracy: "+Math.Round(accuracy, 5, MidpointRounding.AwayFromZero)*100+"%", new Vector2(1500, 880), Color.White);
            spriteBatch.DrawString(font, "Score: " + (int)score, new Vector2(885, 10), Color.White);
            //if (targets.Count > 1 && targets!= null) spriteBatch.DrawString(font, targets[targets.Count-1].position.ToString(), new Vector2(1500, 880), Color.White);
            
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
