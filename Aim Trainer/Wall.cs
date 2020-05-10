using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{
    class Wall
    {
        Game game;
        public Model model;

        public Vector3 position;

        public float facing { get; set; }

        public Matrix world;

        public Wall(Game game, Vector3 pos, float facing)
        {
            this.game = game;
            this.facing = facing;
            model = game.Content.Load<Model>("Wall_1");
            position = pos;
        }

        public void Draw(ICamera camera)
        {
            world = Matrix.CreateRotationY(facing) * Matrix.CreateTranslation(position);

            Matrix view = camera.View;

            Matrix projection = camera.Projection;

            model.Draw(world, view, projection);
        }
    }
}
