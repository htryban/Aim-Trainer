using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{
    class Floor
    {
        Game game;
        public Model model;

        public Vector3 position;

        Matrix[] transforms;
        public float facing { get; set; }

        public Matrix world;

        public Floor(Game game, Vector3 pos, float facing, bool isCeiling)
        {
            this.game = game;
            this.facing = facing;
            if (isCeiling) model = game.Content.Load<Model>("ceiling");
            else model = game.Content.Load<Model>("tiledfloor");
            position = pos;
            transforms = new Matrix[model.Bones.Count];
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
                    effect.SpecularColor = new Vector3(1, 1, 1);
                }
                mesh.Draw();
            }
            model.Draw(world, view, projection);
        }
    }
}
