using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon_editor {
    public static class BresenhamDrawer {
        private static void LineLow(DrawingPlane plane, UInt32 color, int x0, int y0, int x1, int y1) {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;
            if (dy < 0) {
                yi = -1;
                dy = -dy;
            }

            int d = 2 * dy - dx;
            int y = y0;

            for (int x = x0; x <= x1; ++x) {

                plane.SetPixel(x, y, color);
                if (d > 0) {
                    y += yi;
                    d += (2 * (dy - dx));
                }
                else {
                    d += 2 * dy;
                }
            }
        }

        private static void LineHigh(DrawingPlane plane, UInt32 color, int x0, int y0, int x1, int y1) {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;
            if (dx < 0) {
                xi = -1;
                dx = -dx;
            }

            int d = 2 * dx - dy;
            int x = x0;

            for (int y = y0; y <= y1; ++y) {
                plane.SetPixel(x, y, color);
                if (d > 0) {
                    x += xi;
                    d += 2 * (dx - dy);
                }
                else {
                    d += 2 * dx;
                }
            }
        }

        public static void Line(DrawingPlane plane, UInt32 color, int x0, int y0, int x1, int y1) {
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0)) {
                if (x0 > x1) LineLow(plane, color, x1, y1, x0, y0);
                else LineLow(plane, color, x0, y0, x1, y1);
            }
            else {
                if (y0 > y1) LineHigh(plane, color, x1, y1, x0, y0);
                else LineHigh(plane, color, x0, y0, x1, y1);
            }
        }

        public static void Line(DrawingPlane plane, UInt32 color, double x0, double y0, double x1, double y1) {
            Line(plane, color, (int)Math.Round(x0), (int)Math.Round(y0), (int)Math.Round(x1), (int)Math.Round(y1));
        }

        private static void PutSymmetricalCirclePixel(DrawingPlane plane, UInt32 color, int x0, int y0, int x, int y) {
            plane.SetPixel(x0 + x, y0 + y, color);
            plane.SetPixel(x0 - x, y0 + y, color);
            plane.SetPixel(x0 + x, y0 - y, color);
            plane.SetPixel(x0 - x, y0 - y, color);
            plane.SetPixel(x0 + y, y0 + x, color);
            plane.SetPixel(x0 - y, y0 + x, color);
            plane.SetPixel(x0 + y, y0 - x, color);
            plane.SetPixel(x0 - y, y0 - x, color);
        }

        public static void Circle(DrawingPlane plane, UInt32 color, int r, int x0, int y0) {
            int deltaE = 3;
            int deltaSE = 5 - 2 * r;
            int d = 1 - r;
            int x = 0;
            int y = r;

            PutSymmetricalCirclePixel(plane, color, x0, y0, x, y);
            while (y > x) {
                if (d < 0) {
                    d += deltaE;
                    deltaE += 2;
                    deltaSE += 2;
                }
                else {
                    d += deltaSE;
                    deltaE += 2;
                    deltaSE += 4;
                    y -= 1;
                }
                x += 1;
                PutSymmetricalCirclePixel(plane, color, x0, y0, x, y);
            }
        }

        public static void Circle(DrawingPlane plane, UInt32 color, double r, double x0, double y0) {
            Circle(plane, color, (int)Math.Round(r), (int)Math.Round(x0), (int)Math.Round(y0));
        }
    }
}
