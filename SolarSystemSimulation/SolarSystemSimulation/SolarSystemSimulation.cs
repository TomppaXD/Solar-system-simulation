using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolarSystemSimulation
{
    public class SolarSystemSimulation : Game
    {
        SpriteBatch SpriteBatch { get; set; }

        Texture2D Spritesheet { get; set; }

        GraphicsDeviceManager Manager { get; set; }

        SpriteFont Font { get; set; }
        
        int screenWidth = 1500;
        int screenHeight = 1000;

        List<CelestialObject> o = new List<CelestialObject>();

        List<(Texture2D, Vector2, Color)> points = new List<(Texture2D, Vector2, Color)>();

        double G = 6.67428 * Math.Pow(10, -11);
        double AU = 149.6 * Math.Pow(10, 6) * 1000;

        double timestep = 3600 * 24;

        double scale = 100 / (149.6 * Math.Pow(10, 6) * 1000);

        int previousScrollValue;
        public SolarSystemSimulation()
        {
            Manager = new GraphicsDeviceManager(this);
            //Manager.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            Manager.PreferredBackBufferWidth = screenWidth;
            Manager.PreferredBackBufferHeight = screenHeight;
            var mouse = Mouse.GetState();
            previousScrollValue = mouse.ScrollWheelValue;
            Manager.ApplyChanges();
            
            using (var stream = TitleContainer.OpenStream("spritesheet.png"))
            {
                Spritesheet = Texture2D.FromStream(GraphicsDevice, stream);
            }
            this.IsMouseVisible = true;
            base.Initialize();


            /*               start          width  height
             sun            (0, 0)           121    121
             mercury        (0, 125)         121    121
             venus          (0, 250)         121    121
             earth          (0, 375)         121    121
             mars           (125, 0)         121    121
             jupiter        (125, 125)       121    121
             saturn         (273, 0)         211    121
             uranus         (125, 250)       121    121
             neptune        (125, 375)       121    121
            */
            var sun = new CelestialObject("Sun", 1.98892 * Math.Pow(10, 30), 0, 0, 0, 0, 0, 0, 121, 121, 0.2f);
            o.Add(sun);

            var mercury = new CelestialObject("Mercury", 3.30 * Math.Pow(10, 23), 0.387 * AU, 0, 0, 47.4 * 1000, 0, 125, 121, 121, 0.05f);
            o.Add(mercury);

            var venus = new CelestialObject("Venus", 4.867 * Math.Pow(10, 24), 0.7 * AU, 0, 0, 35 * 1000, 0, 250, 121, 121, 0.11f);
            o.Add(venus);

            var earth = new CelestialObject("Earth", 5.972 * Math.Pow(10, 24), AU, 0, 0, 29.8 * 1000, 0, 375, 121, 121, 0.11f);
            o.Add(earth);

            var mars = new CelestialObject("Mars", 6.39 * Math.Pow(10, 23), 1.524 * AU, 0, 0, 24.1 * 1000, 125, 0, 121, 121, 0.12f);
            o.Add(mars);

            var jupiter = new CelestialObject("Jupiter", 1.898 * Math.Pow(10, 27), 5.2 * AU, 0, 0, 13.07 * 1000, 125, 125, 121, 121, 0.2f);
            o.Add(jupiter);

            var saturn = new CelestialObject("Saturn", 5.683 * Math.Pow(10, 26), 9.5 * AU, 0, 0, 9.68 * 1000, 273, 0, 211, 121, 0.15f, -0.1f);
            o.Add(saturn);

            var uranus = new CelestialObject("Uranus", 8.681 * Math.Pow(10, 25), 19.8 * AU, 0, 0, 6.80 * 1000, 125, 250, 121, 121, 0.1f);
            o.Add(uranus);

            var neptune = new CelestialObject("Neptune", 1.024 * Math.Pow(10, 26), 30 * AU, 0, 0, 5.43 * 1000, 125, 375, 121, 121, 0.1f);
            o.Add(neptune);
        }


        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();

            foreach (var point in points)
            {
                SpriteBatch.Draw(point.Item1, point.Item2, point.Item3);
            }

            foreach (var co in o)
            {
                var rect = new Texture2D(GraphicsDevice, 1, 1);

                Color c = Color.White;
                Color[] data = new Color[1 * 1];
                for (int i = 0; i < data.Length; ++i) data[i] = c;
                rect.SetData(data);

                points.Add((rect, new Vector2((float)(co.x * scale + screenWidth / 2), (float)(co.y * scale + screenHeight / 2)), c));


                DrawSprite((int)(co.x * scale), (int)(co.y * scale), co.imageX, co.imageY, co.height, co.width, co.rotation, co.size);
            }

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.ScrollWheelValue < previousScrollValue && scale > 2 / AU)
            {
                scale -= 10 / AU;
            }
            if (mouse.ScrollWheelValue > previousScrollValue)
            {
                scale += 10 / AU;
            }
            previousScrollValue = mouse.ScrollWheelValue;

            foreach (CelestialObject co in o)
            {
                (co.gravityX, co.gravityY) = CalculateTotalGravity(co);
            }
            foreach (CelestialObject co in o)
            {
                // F = ma  =>  a = F / m
                // v = v0 + at = v0 + F / m * t
                co.velocityX += co.gravityX / co.mass * timestep;
                co.velocityY += co.gravityY / co.mass * timestep;

                // x = x0 + vt
                co.x += co.velocityX * timestep;
                co.y += co.velocityY * timestep;
            }
        }
        private (double, double) CalculateTotalGravity(CelestialObject co)
        {
            double totalGravityX = 0;
            double totalGravityY = 0;

            foreach (CelestialObject celestialObject in o)
            {
                double dSquared = Math.Pow(co.x - celestialObject.x, 2) + Math.Pow(co.y - celestialObject.y, 2);
                if (dSquared == 0)
                {
                    continue;
                }
                double distanceX = celestialObject.x - co.x;
                double distanceY = celestialObject.y - co.y;

                double angle = Math.Atan2(distanceY, distanceX);

                // F = G * m1 * m2 / r^2
                double f = G * co.mass * celestialObject.mass / dSquared;
                double fx = Math.Cos(angle) * f;
                double fy = Math.Sin(angle) * f;

                totalGravityX += fx;
                totalGravityY += fy;
            }
            return (totalGravityX, totalGravityY);
        }
        void DrawSprite(int drawX, int drawY, int imageX, int imageY, int imageHeight, int imageWidth, float rotation, float scale, int? xCenter = null, int? yCenter = null)
        {
            var center = new Vector2(xCenter ?? imageWidth / 2, yCenter ?? imageHeight / 2);

            SpriteBatch.Draw(Spritesheet, new Vector2(drawX + screenWidth / 2, drawY + screenHeight / 2), new Rectangle(imageX, imageY, imageWidth, imageHeight), Color.White, rotation, center, scale, SpriteEffects.None, 1);
        }
    }
}
