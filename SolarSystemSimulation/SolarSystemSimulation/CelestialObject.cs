using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystemSimulation
{
    public class CelestialObject
    {
        public string name { get; set; }
        public double mass { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double velocityX { get; set; }
        public double velocityY { get; set; }
        public double gravityX { get; set; }
        public double gravityY { get; set; }
        public int imageX { get; set; }
        public int imageY { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public float size { get; set; }
        public float rotation { get; set; }
        public CelestialObject(string name, double mass, double x, double y, double velocityX, double velocityY, int imageX, int imageY, int width, int height, float size, float rotation = 0)
        {
            this.name = name;
            this.mass = mass;
            this.x = x;
            this.y = y;
            this.velocityX = velocityX;
            this.velocityY = velocityY;
            this.imageX = imageX;
            this.imageY= imageY;
            this.width = width;
            this.height = height;
            this.size = size;
            this.rotation = rotation;
        }
    }
}
