using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{
    public class Bullet
    {
        Game game;
        Model model;

        Vector3 position;

        float facing = 0;

        Vector3 aimTranslation = new Vector3(-1, -1, -1);

        Vector3 aim = Vector3.Forward;

        public Bullet(Game game, Vector3 dir, Vector3 pos)
        {
            this.game = game;
            direction = dir * aimTranslation;
            model = game.Content.Load<Model>("projektil FBX");
            position = pos;
            //transforms = new Matrix[model.Bones.Count];
            //position += new Vector3(0, 20, 0);
            
        }

        public float Speed { get; set; } = 10f;

        public Vector3 direction { get; set; }

        public void Update(GameTime gameTime)
        {
            position -= Speed * direction;
        }

        public void Draw(ICamera camera)
        {
            Matrix world = Matrix.CreateRotationY(facing) * Matrix.CreateTranslation(position);

            Matrix view = camera.View;

            Matrix projection = camera.Projection;

            model.Draw(world, view, projection);
        }


    }
}
