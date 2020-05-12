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
        SpriteFont menufont;
        List<Bullet> bullets;
        List<Target> targets;
        List<Wall> walls;
        Floor floor;
        Floor ceiling;
        List<SoundEffect> sounds;
        Texture2D end;
        Rectangle endRect;

        KeyboardState newKeyboard;
        MouseState newMouseState;
        KeyboardState oldKeyboard;
        MouseState oldMouseState;
        int oldScroll;
        GamePadState newGame;
        GamePadState oldGame;

        Random rand = new Random();
        float fx;
        float fy;
        float fz;
        double score;
        double bulletsFired;
        int fireRate;
        float startTime;
        float checktime;
        float shootTimer;
        float gameTimer;
        string firingMode;
        double accuracy;
        int gamemode;
        bool mainmenu;
        bool countdown;
        bool endscreen;
        int setGamemode;
        int maxTargets;
        double leftHit;
        double leftTargets;
        double rightHit;
        double rightTargets;
        double leftAcc;
        double rightAcc;
        double leftTime;
        double rightTime;
        double avgLeft;
        double avgRight;
        double avgTotal;
        double tempGT;
        
        //text locations in 3d to be transformed to 2d for displaying with spritebatch
        Vector3 easyLocation3D;
        Vector3 normalLocation3D;
        Vector3 hardLocation3D;
        Vector3 sniperLocation3D;
        Vector3 breakdownLocation3D;
        Vector3 countdownLocation3D;
        Vector3 easyLocation2D;
        Vector3 normalLocation2D;
        Vector3 hardLocation2D;
        Vector3 sniperLocation2D;
        Vector3 breakdownLocation2D;
        Vector3 countdownLocation2D;

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
            setGamemode = 5;
            gameTimer = 30;
            endRect.X = (graphics.PreferredBackBufferWidth / 2) - 300;
            endRect.Y = (graphics.PreferredBackBufferHeight / 2) - 450;
            endRect.Width = 600;
            endRect.Height = 900;
            oldScroll= oldMouseState.ScrollWheelValue;

            bullets = new List<Bullet>();
            targets = new List<Target>();
            walls = new List<Wall>();
            sounds = new List<SoundEffect>();
            mainmenu = true;
            countdown = false;
            endscreen = false;

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
            cross = Content.Load<Texture2D>("crosshair");
            fps = new SimpleFps();
            font = Content.Load<SpriteFont>("font");
            menufont = Content.Load<SpriteFont>("menufont");
            fireRate = 0;
            bulletsFired = 0;
            shootTimer = 0;
            checktime = 0;
            gamemode = 5;
            maxTargets = 4;

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

            end = Content.Load<Texture2D>("blue");

            floor = new Floor(this, new Vector3(58,1,-84), 0, false); 
            ceiling = new Floor(this, new Vector3(58, 80, -84), 0, true);

            sounds.Add(Content.Load<SoundEffect>("singleshot"));
            sounds.Add(Content.Load<SoundEffect>("AR15single"));
            sounds.Add(Content.Load<SoundEffect>("pistol"));
            sounds.Add(Content.Load<SoundEffect>("supressed"));
            sounds.Add(Content.Load<SoundEffect>("deagle"));
            sounds.Add(Content.Load<SoundEffect>("splat"));

            easyLocation3D = new Vector3(-80, 25, -200);
            normalLocation3D = new Vector3(-38, 25, -200);
            hardLocation3D = new Vector3(10, 25, -200);
            sniperLocation3D = new Vector3(54, 25, -200);
            breakdownLocation3D = new Vector3(-18, 60, -200);
            countdownLocation3D = new Vector3(-5, 30, -200);
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

            if (mainmenu)
            {

                if(setGamemode == 0) //easy selected
                {
                    gamemode = 6;
                    fireRate = 3;
                    for (int i = 2; i > -1; i--) targets.RemoveAt(i);
                    mainmenu = false;
                    score = 0;
                    bulletsFired = 0;
                    maxTargets = 4;
                    leftHit = 0;
                    rightHit = 0;
                    leftTargets = 0;
                    rightTargets = 0;
                    leftTime = 0;
                    rightTime = 0;
                    gameTimer = 30;
                    tempGT = 0;
                    countdown = true;
                    startTime = checktime;
                }
                else if (setGamemode == 1)//normal selected
                {
                    gamemode = 5;
                    fireRate = 3;
                    for (int i = 2; i > -1; i--) targets.RemoveAt(i);
                    mainmenu = false;
                    score = 0;
                    bulletsFired = 0;
                    maxTargets = 4;
                    leftHit = 0;
                    rightHit = 0;
                    leftTargets = 0;
                    rightTargets = 0;
                    leftTime = 0;
                    rightTime = 0;
                    gameTimer = 30;
                    tempGT = 0;
                    countdown = true;
                    startTime = checktime;
                }
                else if (setGamemode == 2)//hard selected
                {
                    gamemode = 3;
                    fireRate = 3;
                    for (int i = 2; i > -1; i--) targets.RemoveAt(i);
                    mainmenu = false;
                    score = 0;
                    bulletsFired = 0;
                    maxTargets = 4;
                    leftHit = 0;
                    rightHit = 0;
                    leftTargets = 0;
                    rightTargets = 0;
                    leftTime = 0;
                    rightTime = 0;
                    gameTimer = 30;
                    tempGT = 0;
                    countdown = true;
                    startTime = checktime;
                }
                else if (setGamemode == 3)//fruit sniper selected
                {
                    gamemode = 1;
                    fireRate = 4;
                    for (int i = 2; i > -1; i--) targets.RemoveAt(i);
                    mainmenu = false;
                    score = 0;
                    bulletsFired = 0;
                    maxTargets = 6;
                    leftHit = 0;
                    rightHit = 0;
                    leftTargets = 0;
                    rightTargets = 0;
                    leftTime = 0;
                    rightTime = 0;
                    gameTimer = 30;
                    tempGT = 0;
                    countdown = true;
                    startTime = checktime;
                }
            }
            if(!mainmenu && !countdown && !endscreen)
            {
                gameTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (mainmenu && targets.Count < 4)
            {
                //starting menu options
                targets.Add(new Target(this, new Vector3(13, 21, -200), "pumpkin", 0));//easy
                targets.Add(new Target(this, new Vector3(33, 17, -200), "melon", 0));//normal
                targets.Add(new Target(this, new Vector3(53, 20, -200), "menuApple", 0));//hard
                targets.Add(new Target(this, new Vector3(73, 20, -200), "menuOrange", 0));//fruit sniper
            }

            if (gameTimer <= 0)
            {
                tempGT = 0;
                gameTimer = 30;
                endscreen = true;
            }

            //shooting
            if (!endscreen)
            {
                if (fireRate == 0)
                {
                    firingMode = "Single Fire";
                    if ((newMouseState.LeftButton == ButtonState.Pressed || newGame.Triggers.Right > 0) && (oldMouseState.LeftButton != ButtonState.Pressed && oldGame.Triggers.Right == 0))
                    {
                        bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.horizontalAngle, fpsCamera.position));
                        bulletsFired++;
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
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
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
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
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
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
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
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
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
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
                        if (!mainmenu)
                        {
                            if (fpsCamera.facing.X >= 0) rightTargets++;
                            else leftTargets++;
                        }
                        var instance = sounds[1].CreateInstance();
                        instance.Play();
                    }
                }
            }

            if (endscreen)
            {
                for (int i = targets.Count - 1; i > -1; i--) targets.RemoveAt(i);
                gamemode = 5;
                setGamemode = 5;
                maxTargets = 4;
                fireRate = 0;
                if (gameTimer != 30)tempGT = gameTimer;
                gameTimer = 30;
                countdown = false;
                if ((newMouseState.RightButton == ButtonState.Pressed || newGame.Triggers.Left> 0) && (oldMouseState.RightButton != ButtonState.Pressed && oldGame.Triggers.Left == 0))
                {
                    endscreen = false;
                    mainmenu = true;
                }
            }

            //manually create enemies
            /*if ((newKeyboard.IsKeyDown(Keys.T) && !oldKeyboard.IsKeyDown(Keys.T)) || (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0))
            {
                targets.Add(new Target(this, randomSpot()));
            }*/


            //changing weapons / firerate
            if (!mainmenu && !endscreen && !countdown)
            {
                if ((newKeyboard.IsKeyDown(Keys.Right) && !oldKeyboard.IsKeyDown(Keys.Right)) || (newGame.DPad.Right == ButtonState.Pressed && oldGame.DPad.Right == ButtonState.Released)
                    || newMouseState.ScrollWheelValue > oldScroll)
                    if (fireRate < 5) fireRate++;
                if ((newKeyboard.IsKeyDown(Keys.Left) && !oldKeyboard.IsKeyDown(Keys.Left)) || (newGame.DPad.Left == ButtonState.Pressed && oldGame.DPad.Left == ButtonState.Released)
                    || newMouseState.ScrollWheelValue < oldScroll)
                    if (fireRate > 0) fireRate--;

                if ((newKeyboard.IsKeyDown(Keys.Up) && !oldKeyboard.IsKeyDown(Keys.Up)) || (newGame.DPad.Up == ButtonState.Pressed && oldGame.DPad.Up == ButtonState.Released))
                    fpsCamera.Sensitivity += .01f;
                if ((newKeyboard.IsKeyDown(Keys.Down) && !oldKeyboard.IsKeyDown(Keys.Down)) || (newGame.DPad.Down == ButtonState.Pressed && oldGame.DPad.Down == ButtonState.Released))
                    fpsCamera.Sensitivity -= .01f;
            }

            //manually quitting a game before time is up
            if((newKeyboard.IsKeyDown(Keys.Q) || newGame.Buttons.Start == ButtonState.Pressed) && !mainmenu && !endscreen)
            {
                for (int i = targets.Count - 1; i > -1; i--) targets.RemoveAt(i);
                gamemode = 5;
                setGamemode = 5;
                maxTargets = 4;
                fireRate = 0;
                tempGT = gameTimer;
                countdown = false;
                endscreen = true;
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
                                if (!mainmenu)
                                {
                                    if (targets[j].position.X <= 50)
                                    {
                                        leftHit++;
                                        leftTime += targets[j]._timer;
                                    }
                                    else
                                    {
                                        rightHit++;
                                        rightTime += targets[j]._timer;
                                    }
                                }
                            bullets.RemoveAt(i);
                            targets.RemoveAt(j);
                            setGamemode = j; // used in main menu to detect which gamemode option has been shot
                            i--;
                            var instance = sounds[5].CreateInstance();
                            instance.Play();
                            if (gamemode == 1) gameTimer += .2f;
                            break;
                        }
                    }
                }
            }

            //switching difficulties/gamemodes for testing
            /*
            if (newKeyboard.IsKeyDown(Keys.NumPad1) || newKeyboard.IsKeyDown(Keys.D1)) gamemode = 6; // easy
            if (newKeyboard.IsKeyDown(Keys.NumPad2) || newKeyboard.IsKeyDown(Keys.D2)) gamemode = 5; // normal
            if (newKeyboard.IsKeyDown(Keys.NumPad3) || newKeyboard.IsKeyDown(Keys.D3)) gamemode = 3; // hard
            if (newKeyboard.IsKeyDown(Keys.NumPad4) || newKeyboard.IsKeyDown(Keys.D4)) gamemode = 1; // fruit sniper
            */

            //replacing targets that are shot or expire
            if (targets.Count < maxTargets && !mainmenu && !countdown && !endscreen)
            {
                targets.Add(new Target(this, randomSpot(), randomFruit(gamemode), gamemode));
            }

            if (targets != null) for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Update(gameTime);
                if (targets[i].isRemoved)
                {
                        //if (targets[i].position.X <= 50) leftTime += 5;
                        //else rightTime += 5;
                    targets.RemoveAt(i);
                    i--;
                }
            }

            //computing averages
            if (bulletsFired > 0)
            {
                accuracy = score / bulletsFired;
                if (leftTargets > 0) leftAcc = leftHit / leftTargets;
                else leftAcc = 0;
                if (rightTargets > 0) rightAcc = rightHit / rightTargets;
                else rightAcc = 0;
                if (leftTime > 0) avgLeft = leftTime / leftHit;
                else avgLeft = 0;
                if (rightTime > 0) avgRight = rightTime / rightHit;
                else avgRight = 0;
                if (avgRight + avgLeft > 0) avgTotal = avgRight + avgLeft / score;
                else avgTotal = 0;
            }

            oldKeyboard = newKeyboard;
            oldMouseState = newMouseState;
            oldGame = newGame;
            oldScroll = newMouseState.ScrollWheelValue;
            base.Update(gameTime);
        }

        public Vector3 randomSpot()
        {            
            fx = (float)rand.Next(0, 100);
            if (gamemode == 1)
            {
                fy = 60;
                fz = -250;
            }
            else
            {
                fy = (float)rand.Next(6, 40);
                fz = (float)rand.Next(-250, -200);
            }
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
            var viewport = GraphicsDevice.Viewport;
            var projection = fpsCamera.Projection;
            var view = fpsCamera.View;
            var world = fpsCamera.world;
            
            if (mainmenu)
            {
                easyLocation2D = viewport.Project(easyLocation3D, projection, view, world);
                normalLocation2D = viewport.Project(normalLocation3D, projection, view, world);
                hardLocation2D = viewport.Project(hardLocation3D, projection, view, world);
                sniperLocation2D = viewport.Project(sniperLocation3D, projection, view, world);
                breakdownLocation2D = viewport.Project(breakdownLocation3D, projection, view, world);
            }

            spriteBatch.Begin();

            floor.Draw(fpsCamera);
            ceiling.Draw(fpsCamera);

            if (walls!= null) foreach (var wall in walls)
                {
                    wall.Draw(fpsCamera);
                }

            //in an active game
            if (!mainmenu && !countdown && !endscreen)
            {
                fps.DrawFps(spriteBatch, font, new Vector2(10f, 1040f), Color.Black);
                spriteBatch.DrawString(font, "Fire Rate: " + firingMode, new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(font, "Time Left: "+ Math.Round(gameTimer, 1, MidpointRounding.AwayFromZero), new Vector2(1750, 10), Color.White); 
                spriteBatch.DrawString(font, "Score: " + (int)score, new Vector2(885, 10), Color.White);
            }

            //countdown after difficulty select before game begins 
            if (countdown)
            {
                countdownLocation2D = viewport.Project(countdownLocation3D, projection, view, world);
                if (checktime - startTime < 1) spriteBatch.DrawString(menufont, "3", new Vector2(countdownLocation2D.X, countdownLocation2D.Y), Color.White, 0, default, 3, SpriteEffects.None, default);
                else if (checktime - startTime < 2) spriteBatch.DrawString(menufont, "2", new Vector2(countdownLocation2D.X, countdownLocation2D.Y), Color.White, 0, default, 3, SpriteEffects.None, default);
                else if (checktime - startTime < 3) spriteBatch.DrawString(menufont, "1", new Vector2(countdownLocation2D.X, countdownLocation2D.Y), Color.White, 0, default, 3, SpriteEffects.None, default);
                else { countdown = false; bulletsFired = 0; }
                spriteBatch.DrawString(font, "Use the Scroll wheel or L/R on the D-Pad to change Weapons\n\n" +
                                             "   Press Q or the Start button to return to the Main Menu,\n\n" +
                                             "  Up/Down arrow keys or on the D-Pad to adjust sensitivity", new Vector2(countdownLocation2D.X - 240, countdownLocation2D.Y + 200), Color.White);
            }

            //main menu screen
            if (mainmenu && !countdown && fpsCamera.facing.Z < 0)
            {
                spriteBatch.DrawString(menufont, "EASY", new Vector2(easyLocation2D.X, easyLocation2D.Y), Color.White);
                spriteBatch.DrawString(menufont, "NORMAL", new Vector2(normalLocation2D.X, normalLocation2D.Y), Color.White);
                spriteBatch.DrawString(menufont, "HARD", new Vector2(hardLocation2D.X, hardLocation2D.Y), Color.White);
                spriteBatch.DrawString(menufont, " FRUIT\nSNIPER", new Vector2(sniperLocation2D.X, sniperLocation2D.Y), Color.White);
                spriteBatch.DrawString(menufont, "   SELECT\n         A\nGAMEMODE", new Vector2(breakdownLocation2D.X, breakdownLocation2D.Y), Color.White);
            }

            //on the scorecard results screen
            if (endscreen)
            {
                spriteBatch.Draw(end, endRect, null, Color.White * .5f);
                spriteBatch.DrawString(menufont, "SCORECARD\n-----------------------------------------------", new Vector2(endRect.X + 20, endRect.Y + 20), Color.White);
                spriteBatch.DrawString(font, "    You destroyed a total of "+ score + " targets\n\n    You fired a total of " + bulletsFired + " bullets\n\n" +
                    "    Total Game Time was: " + Math.Round(30-tempGT, 1, MidpointRounding.AwayFromZero) + " seconds\n\n\n" +
                    "ACCURACY BREAKDOWN\n\n    Your overall accuracy was " + Math.Round(accuracy, 4, MidpointRounding.AwayFromZero) * 100 + "%\n\n" +
                    "    Left side accuracy: " + leftHit + " / " + leftTargets + " => " + Math.Round(leftAcc, 4, MidpointRounding.AwayFromZero) * 100 + "%\n\n" +
                    "    Right side accuracy: " + rightHit + " / " + rightTargets + " => " + Math.Round(rightAcc, 4, MidpointRounding.AwayFromZero) * 100 + "%\n\n\n" +
                    "REACTION TIME AVERAGES\n\n" +
                    "    Average left side reaction time: " + Math.Round(avgLeft, 3, MidpointRounding.AwayFromZero) + " seconds\n\n" +
                    "    Average right side reaction time: " + Math.Round(avgRight, 3, MidpointRounding.AwayFromZero) + " seconds\n\n" +
                    "    Average overall reaction time: " + Math.Round(avgTotal, 3, MidpointRounding.AwayFromZero) + " seconds\n\n\n" +
                    "Right Click (Left Trigger) to return to the Main Menu", new Vector2(endRect.X + 20, endRect.Y + 95), Color.White);
                spriteBatch.DrawString(menufont, "-----------------------------------------------", new Vector2(endRect.X + 20, endRect.Y + 840), Color.White);
            }
            
            if (targets != null) foreach (var target in targets)
            {
                target.Draw(fpsCamera);
            }

            if (bullets != null) foreach (var bullet in bullets)
            {
                bullet.Draw(fpsCamera);
            }
            
            if(!endscreen) spriteBatch.Draw(cross, crosshair, null, Color.White, 0, new Vector2(cross.Width / 2f, cross.Height / 2f), SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
