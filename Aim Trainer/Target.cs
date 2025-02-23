﻿using Microsoft.Xna.Framework;
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

        public float _timer;

        public Matrix world;

        bool falling;

        int gm;

        public Target(Game game, Vector3 pos, string fruit, int gamemode)
        {
            this.game = game;
            model = game.Content.Load<Model>(fruit);
            position = pos;
            transforms = new Matrix[model.Bones.Count];
            gm = gamemode;
            if (gm == 1) falling = true;
        }

        public void Update(GameTime gameTime)
        {
            if(falling) position.Y -= .3f;
            if (position.Y <= -5)
            {
                isRemoved = true;
            }
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if ((_timer > lifespan) && (gm != 0)) isRemoved = true;
        }

        public void Draw(ICamera camera)
        {
            world = Matrix.CreateRotationY(facing) * Matrix.CreateTranslation(position);

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
