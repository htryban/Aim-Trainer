using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        Sphere sphere;
        SimpleFps fps;
        SpriteFont font;
        //Crate[] bullets;
        List<Bullet> bullets;
        Bullet bull;

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
            // TODO: Add your initialization logic here
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fpsCamera = new FPSCamera(this, new Vector3(0, 20, 10));
            //fpsCamera = new CirclingCamera(this, new Vector3(0,50,-100), 0.5f);
            Texture2D heightmap = Content.Load<Texture2D>("white-hm");
            cross = Content.Load<Texture2D>("crosshair");
            sphere = new Sphere(5, graphics.GraphicsDevice);
            terrain = new Terrain(this, heightmap, 10f, Matrix.CreateTranslation(-127f, 0, 127));
            fps = new SimpleFps();
            font = Content.Load<SpriteFont>("font");
            //bull = new Bullet(this, fpsCamera.facing);
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
            var newMouseState = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            if (newMouseState.LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
            {
                bullets.Add(new Bullet(this, fpsCamera.firingAngle, fpsCamera.position));
            }

            // TODO: Add your update logic here
            fpsCamera.Update(gameTime);
            fps.Update(gameTime);
            //bull.Update(gameTime);
            if (bullets != null) foreach (var bullet in bullets)
            {
                bullet.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); 
            
            terrain.Draw(fpsCamera);
            //sphere.Draw(fpsCamera);
            
            spriteBatch.Begin();
            fps.DrawFps(spriteBatch, font, new Vector2(10f, 10f), Color.MonoGameOrange);
            spriteBatch.DrawString(font, fpsCamera.firingAngle.ToString(), new Vector2(1500, 980), Color.White);

            if (bullets != null) foreach (var bullet in bullets)
            {
                bullet.Draw(fpsCamera);
            }
            //bull.Draw(fpsCamera);
            spriteBatch.Draw(cross, crosshair, null, Color.White, 0, new Vector2(cross.Width / 2f, cross.Height / 2f), SpriteEffects.None, 0);
            spriteBatch.End();

            
            
            base.Draw(gameTime);
        }
    }
}
