using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon_editor {
    public class LightIcon : Shape {
        public UInt32 Color;
        public int X;
        public int Y;
        public int Rays;
        public int Radius;

        public LightIcon(UInt32 color, int x, int y, int rays, int radius) {
            Color = color;
            X = x;
            Y = y;
            Rays = rays;
            Radius = radius;
        }

        public override void DrawOn(DrawingPlane plane) {
            DrawDisc(plane);
            DrawRays(plane);
        }

        private void DrawDisc(DrawingPlane plane) {
            BresenhamDrawer.Circle(
                plane,
                Color,
                Radius,
                X, Y
            );
        }

        private void DrawRays(DrawingPlane plane) {
            for(double phi = 0.0; phi < Math.PI * 2; phi += Math.PI * 2 / Rays) {
                int x1 = (int)Math.Round(2 * Radius * Math.Cos(phi)) + X;
                int y1 = (int)Math.Round(2 * Radius * Math.Sin(phi)) + Y;
                int x2 = (int)Math.Round(2 * Radius * Math.Cos(phi + Math.PI)) + X;
                int y2 = (int)Math.Round(2 * Radius * Math.Sin(phi + Math.PI)) + Y;

                BresenhamDrawer.Line(
                    plane,
                    Color,
                    x1, y1,
                    x2, y2 
                );
            }
        }
    }
}
