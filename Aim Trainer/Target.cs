using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{
    class Target
    {
        Game game;
        public Model model;

        public Vector3 position;

        public Vector3 direction { get; set; }

        public float facing { get; set; }

        Matrix[] transforms;

        public bool isRemoved = false;

        public float lifespan = 5f;

        private float _timer;

        public Matrix world;

        public Target(Game game, Vector3 pos)
        {
            this.game = game;
            model = game.Content.Load<Model>("apple");
            position = pos;
            transforms = new Matrix[model.Bones.Count];
        }

        public void Update(GameTime gameTime)
        {
            //position.Y -= .1f;
            if (position.Y <= -5)
            {
                isRemoved = true;
            }
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > lifespan) isRemoved = true;
        }

        public void Draw(ICamera camera)
        {
            world = Matrix.CreateRotationY(facing) * Matrix.CreateTranslation(position);// * Matrix.CreateFromAxisAngle(new Vector3(0,0,0), .9f);

            Matrix view = camera.View;

            Matrix projection = camera.Projection;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.SpecularColor = new Vector3(1,1,1);
                }
                mesh.Draw();
            }
            model.Draw(world, view, projection);
        }
    }
}
