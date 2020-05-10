using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{ 
    /// <summary>
    /// A camera controlled by WASD + Mouse
    /// </summary>
    public class FPSCamera : ICamera
    {
        // The angle of rotation about the Y-axis
        public float horizontalAngle { get; set; }

        // The angle of rotation about the X-axis
        float verticalAngle;

        // The camera's position in the world 
        public Vector3 position { get; set; }

        // The Game this camera belongs to 
        Game game;

        //the old mousestate
        MouseState oldMouseState;
        private Vector3 direction;

        /// <summary>
        /// The view matrix for this camera
        /// </summary>
        public Matrix View { get; protected set; }

        /// <summary>
        /// The projection matrix for this camera
        /// </summary>
        public Matrix Projection { get; protected set; }

        /// <summary>
        /// The sensitivity of the mouse when aiming
        /// </summary>
        public float Sensitivity { get; set; } = 0.1f;

        /// <summary>
        /// The speed of the player while moving 
        /// </summary>
        public float Speed { get; set; } = 0.7f;

        /// <summary>
        /// vector3 where the player is facing 
        /// </summary>
        public Vector3 facing { get; set; }

        public Vector3 firingAngle { get; set; }


        /// <summary>
        /// Constructs a new FPS Camera
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The player's initial position</param>
        public FPSCamera(Game game, Vector3 position)
        {
            this.game = game;
            this.position = position;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 1000);
            facing = new Vector3(0, 0, 1);
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var newMouseState = Mouse.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //get dir player is facing
            facing = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(horizontalAngle));

            //movement
            if (keyboard.IsKeyDown(Keys.W)) position += facing * Speed;
            if (keyboard.IsKeyDown(Keys.S)) position -= facing * Speed;
            if (keyboard.IsKeyDown(Keys.A)) position += Vector3.Cross(Vector3.Up, facing) * Speed;
            if (keyboard.IsKeyDown(Keys.D)) position -= Vector3.Cross(Vector3.Up, facing) * Speed;
            //position += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * facing * Speed;
            //position -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * Vector3.Cross(Vector3.Up, facing) * Speed;

            //adj mouse angles
            horizontalAngle += Sensitivity * (oldMouseState.X - newMouseState.X) * elapsed;
            verticalAngle += Sensitivity * (.75f) * (oldMouseState.Y - newMouseState.Y) * elapsed;
            horizontalAngle -= Sensitivity * (.35f) * GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;
            verticalAngle += Sensitivity * (.2f) * GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y;
            direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(verticalAngle) * Matrix.CreateRotationY(horizontalAngle));

            //create view matrix
            View = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
            firingAngle = new Vector3(facing.X, verticalAngle, facing.Z);
            //reset mouse state
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();
        }

    }

}
