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
        public CelestialObject(string name, double mass, double x, double y, double velocityX, double velocityY)
        {
            this.name = name;
            this.mass = mass;
            this.x = x;
            this.y = y;
            this.velocityX = velocityX;
            this.velocityY = velocityY;
        }
    }
}
