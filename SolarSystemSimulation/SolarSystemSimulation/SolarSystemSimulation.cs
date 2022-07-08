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
        
        int screenWidth = 1000;
        int screenHeight = 1000;

        List<CelestialObject> o = new List<CelestialObject>();

        double G = 6.67428 * Math.Pow(10, -11);
        double AU = 149.6 * Math.Pow(10, 6) * 1000;

        double timestep = 3600 * 24;
        public SolarSystemSimulation()
        {
            Manager = new GraphicsDeviceManager(this);
            //Manager.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            Manager.PreferredBackBufferWidth = screenWidth;
            Manager.PreferredBackBufferHeight = screenHeight;
            
            Manager.ApplyChanges();
            
            using (var stream = TitleContainer.OpenStream("spritesheet.png"))
            {
                Spritesheet = Texture2D.FromStream(GraphicsDevice, stream);
            }
            this.IsMouseVisible = true;
            base.Initialize();

            var sun = new CelestialObject("Sun", 1.98892 * Math.Pow(10, 30), 0, 0, 0, 0);
            var mercury = new CelestialObject("Mercury", 3.30 * Math.Pow(10, 23), 0.387 * AU, 0, 0, 47.4 * 1000);
            var venus = new CelestialObject("Venus", 4.867 * Math.Pow(10, 24), 0.7 * AU, 0, 0, 35 * 1000);
            var earth = new CelestialObject("Earth", 5.972 * Math.Pow(10, 24), AU, 0, 0, 29.8 * 1000);
            var mars = new CelestialObject("Mars", 6.39 * Math.Pow(10, 23), 1.524 * AU, 0, 0, 24.1 * 1000);
            var jupiter = new CelestialObject("Jupiter", 1.898 * Math.Pow(10, 27), 5.2 * AU, 0, 0, 13.07 * 1000);
            var saturn = new CelestialObject("Saturn", 5.683 * Math.Pow(10, 26), 9.5 * AU, 0, 0, 9.68 * 1000);
            var uranus = new CelestialObject("Uranus", 8.681 * Math.Pow(10, 25), 19.8 * AU, 0, 0, 6.80 * 1000);
            var neptune = new CelestialObject("Neptune", 1.024 * Math.Pow(10, 26), 30 * AU, 0, 0, 5.43 * 1000);
            
            o.Add(sun);
            o.Add(mercury);
            
            o.Add(venus);
            o.Add(earth);
            o.Add(mars);
            o.Add(jupiter);
            o.Add(saturn);
            o.Add(uranus);
            o.Add(neptune);
        }


        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            /*               start             end
             sun            (0, 0)          (121, 121)
             mercury        (0, 125)        (121, 121)
             venus          (0, 250)        (121, 371)
             earth          (0, 375)        
             mars           (125, 0)
             jupiter        (125, 125)
             saturn         (273, 0)        (484, 121)
             uranus         (125, 250)
             neptune        (125, 375)
            */


            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();

            foreach (var co in o)
            {
                switch (co.name)
                {
                    case "Sun":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 0, 0, 121, 121, 0, 0.2f);
                        break;
                    case "Mercury":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 0, 125, 121, 121, 0, 0.05f);
                        break;
                    case "Venus":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 0, 250, 121, 121, 0, 0.11f);
                        break;
                    case "Earth":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 0, 375, 121, 121, 0, 0.11f);
                        break;
                    case "Mars":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 125, 0, 121, 121, 0, 0.12f);
                        break;
                    case "Jupiter":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 125, 125, 121, 121, 0, 0.2f);
                        break;
                    case "Saturn":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 273, 0, 121, 211, -0.1f, 0.15f);
                        break;
                    case "Uranus":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 125, 250, 121, 121, 0, 0.1f);
                        break;
                    case "Neptune":
                        DrawSprite((int)(co.x / AU * 16), (int)(co.y / AU * 16), 125, 375, 121, 121, 0, 0.1f);
                        break;
                }
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (CelestialObject co in o)
            {
                (co.gravityX, co.gravityY) = CalculateTotalGravity(co);
            }
            foreach (CelestialObject co in o)
            {
                co.velocityX += co.gravityX / co.mass * timestep;
                co.velocityY += co.gravityY / co.mass * timestep;

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
                double fy = 0;
                double fx = 0;
                double d = Math.Sqrt(Math.Pow(co.x - celestialObject.x, 2) + Math.Pow(co.y - celestialObject.y, 2));
                if (d == 0)
                {
                    continue;
                }
                double dx = celestialObject.x - co.x;
                double dy = celestialObject.y - co.y;

                double f = G * co.mass * celestialObject.mass / Math.Pow(d, 2);
                double theta = Math.Atan2(dy, dx);
                
             
                fx = Math.Cos(theta) * f;
                fy = Math.Sin(theta) * f;
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
