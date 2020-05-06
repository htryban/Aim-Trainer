using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public float facing { get; set; }

        Vector3 aimTranslation = new Vector3(-1, -1, -1);

        public float added { get; set; } = 3.14f;

        public Bullet(Game game, Vector3 dir, float horizonalAngle, Vector3 pos)
        {
            this.game = game;
            direction = dir * aimTranslation;
            model = game.Content.Load<Model>("projektil FBX");
            position = pos;
            facing = (added+horizonalAngle);            
        }

        public float Speed { get; set; } = 10f;

        public Vector3 direction { get; set; }

        public void Update(GameTime gameTime)
        {
            position -= Speed * direction;
        }

        public void Draw(ICamera camera)
        {
            Matrix world = Matrix.CreateRotationY(facing) * Matrix.CreateTranslation(position);// * Matrix.CreateFromAxisAngle(new Vector3(0,0,0), .9f);
            
            Matrix view = camera.View;

            Matrix projection = camera.Projection;
            
            model.Draw(world, view, projection);
        }


    }
}
