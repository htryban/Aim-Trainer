using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim_Trainer
{
    public class Sphere
    {
        VertexPositionColor[] vertices; //later, I will provide another example with VertexPositionNormalTexture
        VertexBuffer vbuffer;
        short[] indices;
        IndexBuffer ibuffer;
        float radius;
        int nvertices, nindices;
        BasicEffect effect;
        GraphicsDevice graphicd;
        public Sphere(float Radius, GraphicsDevice graphics)
        {
            radius = Radius;
            graphicd = graphics;
            effect = new BasicEffect(graphicd);
            nvertices = 90 * 90; // 90 vertices in a circle, 90 circles in a sphere
            nindices = 90 * 90 * 6;
            vbuffer = new VertexBuffer(graphics, typeof(VertexPositionNormalTexture), nvertices, BufferUsage.WriteOnly);
            ibuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, nindices, BufferUsage.WriteOnly);
            createspherevertices();
            createindices();
            vbuffer.SetData<VertexPositionColor>(vertices);
            ibuffer.SetData<short>(indices);
            effect.VertexColorEnabled = true;
        }
        void createspherevertices()
        {
            vertices = new VertexPositionColor[nvertices];
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 rad = new Vector3((float)Math.Abs(radius), 0, 0);
            for (int x = 0; x < 90; x++) //90 circles, difference between each is 4 degrees
            {
                float difx = 360.0f / 90.0f;
                for (int y = 0; y < 90; y++) //90 veritces, difference between each is 4 degrees 
                {
                    float dify = 360.0f / 90.0f;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify)); //rotate vertex around z
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx)); //rotate circle around y
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot);//transformation

                    vertices[x + y * 90] = new VertexPositionColor(point, Color.Black);
                }
            }
        }
        void createindices()
        {
            indices = new short[nindices];
            int i = 0;
            for (int x = 0; x < 90; x++)
            {
                for (int y = 0; y < 90; y++)
                {
                    int s1 = x == 89 ? 0 : x + 1;
                    int s2 = y == 89 ? 0 : y + 1;
                    short upperLeft = (short)(x * 90 + y);
                    short upperRight = (short)(s1 * 90 + y);
                    short lowerLeft = (short)(x * 90 + s2);
                    short lowerRight = (short)(s1 * 90 + s2);
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;
                    indices[i++] = lowerLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                }
            }
        }
        public void Draw(ICamera cam) // the camera class contains the View and Projection Matrices
        {
            effect.View = cam.View;
            effect.Projection = cam.Projection;
            graphicd.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame }; // Wireframe as in the picture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicd.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, nvertices, indices, 0, indices.Length / 3);
            }
        }
    }
}
