﻿using Microsoft.Xna.Framework;
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

        public Vector3 position;

        public float facing { get; set; }

        Vector3 aimTranslation = new Vector3(-1, -1, -1);

        public float added { get; set; } = MathHelper.Pi;

        public bool isRemoved = false;

        public float lifespan = 1.5f;

        private float _timer;

        Matrix world;

        public float Speed { get; set; } = 5f;

        public Vector3 direction { get; set; }

        public Bullet(Game game, Vector3 dir, float horizonalAngle, Vector3 pos)
        {
            this.game = game;
            direction = dir * aimTranslation;
            model = game.Content.Load<Model>("projektil FBX");
            position = pos;
            facing = (added+horizonalAngle);            
        }

        public bool collidesWith(Model target, Matrix targetWorld)
        {
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(ModelMesh targetMesh in target.Meshes)
                {
                    if (mesh.BoundingSphere.Transform(world).Intersects(targetMesh.BoundingSphere.Transform(targetWorld))){
                        return true;
                    }
                }
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            position -= Speed * direction;
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > lifespan) isRemoved = true;
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
