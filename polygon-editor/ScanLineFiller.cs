using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace polygon_editor {
    public static class ScanLineFiller {
        private class ActiveEdge {
            public int From;
            public int To;
            public double X;
            public double Diff;
            public Vec3 Color;
            public Vec3 ColorDiff;
        }

        private static double Differential(Vec2[] points, int i, int j) {
            if(points[i].Y == points[j].Y) {
                return double.NaN;
            }

            return
                (double)(points[j].X - points[i].X) /
                (double)(points[j].Y - points[i].Y);
        }

        private static Vec3 ColorDifferential(Vec2[] points, UInt32[] colors, int i, int j) {
            return
                (new Vec3(colors[j]) - new Vec3(colors[i])) /
                (double)(points[j].Y - points[i].Y);
        }

        private static void AddToAET(List<ActiveEdge> aet, ActiveEdge ae) {
            if(!double.IsNaN(ae.Diff)) {
                aet.Add(ae);
            }
        }

        private static void AddTopPointToAET(Vec2[] points, UInt32[] colors, int point, List<ActiveEdge> aet) {
            double x = points[point].X;
            int nextPoint = (point + 1) % points.Length;
            int prevPoint = point == 0 ? points.Length - 1 : point - 1;

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, nextPoint),
                From = point,
                To = nextPoint,
                Color = new Vec3(colors[point]),
                ColorDiff = ColorDifferential(points, colors, point, nextPoint)
            });

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, prevPoint),
                From = point,
                To = prevPoint,
                Color = new Vec3(colors[point]),
                ColorDiff = ColorDifferential(points, colors, point, prevPoint)
            });
        }

        private static (List<ActiveEdge>, int) InitAET(Vec2[] points, UInt32[] colors, int[] perm) {
            List<ActiveEdge> aet = new List<ActiveEdge>();

            for(int i = 0; i < points.Length; ++i) {
                if(points[perm[i]].Y != points[perm[0]].Y) {
                    return (aet, i);
                }

                AddTopPointToAET(points, colors, perm[i], aet);
            }

            return (aet, points.Length);
        }

        private static void HandleNewEdge(Vec2[] points, UInt32[] colors, List<ActiveEdge> aet, int i, int j) {
            if(points[j].Y >= points[i].Y) {
                AddToAET(aet, new ActiveEdge() {
                    X = points[i].X,
                    Diff = Differential(points, i, j),
                    From = i,
                    To = j,
                    Color = new Vec3(colors[i]),
                    ColorDiff = ColorDifferential(points, colors, i, j)
                });
            }
            else {
                aet.RemoveAll((ActiveEdge ae) => ae.To == i && ae.From == j);
            }
        }

        private static void DrawScanline(DrawingPlane plane, List<ActiveEdge> aet, int y, LightPackage lp) {
            for(int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];
                Vec3 colorDiff = (ae2.Color - ae1.Color) / (ae2.X - ae1.X);
                Vec3 color = ae1.Color;

                for(int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    UInt32 shadedColor = CalculateColor(
                        new Vec3(x, y, 0),
                        color,
                        new Vec3(0, 0, 1),
                        lp.kd,
                        lp.ks,
                        lp.lightPos,
                        lp.IL,
                        lp.m
                    );
                    plane.SetPixel(x, y, shadedColor);
                    color += colorDiff;
                }

                ae1.X += ae1.Diff;
                ae1.Color += ae1.ColorDiff;
                ae2.X += ae2.Diff;
                ae2.Color += ae2.ColorDiff;
            }
        }

        private static (int, int) GetBound(Vec2[] points) {
            int x = (int)Math.Round(points.Min((Vec2 v) => v.X));
            int y = (int)Math.Round(points.Min((Vec2 v) => v.Y));
            return (x, y);
        }

        public class LightPackage {
            public double kd;
            public double ks;
            public Vec3 lightPos;
            public Vec3 IL;
            public double m;
        }

        private static UInt32 CalculateColor(Vec3 point, Vec3 IO, Vec3 N, double kd, double ks, Vec3 lightPos, Vec3 IL, double m) {
            Vec3 L = Vec3.UnitDirection(point, lightPos);
            Vec3 V = new Vec3(0, 0, 1);
            Vec3 R = 2 * Vec3.DotProduct(N, L) * N - L;

            double multiplier = (kd * Vec3.AngleCosine(N, L) + ks * Math.Pow(Vec3.AngleCosine(V, R), m));
            Vec3 color = new Vec3(IL.X * IO.X, IL.Y * IO.Y, IL.Z * IO.Z) * multiplier;
            return color.ToColor();
        }

        private static UInt32 GetBitmapPixel(Bitmap texture, int width, int height, int x, int y, int minX, int minY) {
            UInt32 color;
            unsafe {
                int intColor = texture.GetPixel(
                    Math.Max(0, (x - minX)) % width,
                    Math.Max(0, (y - minY)) % height
                ).ToArgb();
                color = *(UInt32*)(void*)&intColor;
            }

            return color;
        }

        private static void DrawTexturedScanline(
            DrawingPlane plane,
            List<ActiveEdge> aet,
            int y,
            Bitmap texture,
            Bitmap heightmap,
            int minX, int minY,
            RectangleF textureBounds,
            RectangleF? heightBounds,
            LightPackage lp
        ) {
            for(int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];
                int height = (int)textureBounds.Height;
                int width = (int)textureBounds.Width;
                int hheight = 0;
                int hwidth = 0;
                if(heightmap != null) {
                    hheight = (int)heightBounds.Value.Height;
                    hwidth = (int)heightBounds.Value.Width;
                }

                for(int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    Vec3 N;
                    double z = 0;
                    if(heightmap == null) {
                        N = new Vec3(0, 0, 1);
                    }
                    else {
                        z = (GetBitmapPixel(heightmap, hwidth, hheight, x, y, minX, minY) & 0x000000FF) / 255.0;
                        double xp1 = (GetBitmapPixel(heightmap, hwidth, hheight, x + 1, y, minX, minY) & 0x000000FF) / 255.0;
                        double xm1 = (GetBitmapPixel(heightmap, hwidth, hheight, x - 1, y, minX, minY) & 0x000000FF) / 255.0;
                        double yp1 = (GetBitmapPixel(heightmap, hwidth, hheight, x, y + 1, minX, minY) & 0x000000FF) / 255.0;
                        double ym1 = (GetBitmapPixel(heightmap, hwidth, hheight, x, y - 1, minX, minY) & 0x000000FF) / 255.0;
                        N = new Vec3(xp1 - xm1, yp1 - ym1, 1).Normalize();
                    }
                    UInt32 shadedColor = CalculateColor(
                        new Vec3(x, y, z * 10),
                        new Vec3(GetBitmapPixel(texture, width, height, x, y, minX, minY)),
                        N,
                        lp.kd,
                        lp.ks,
                        lp.lightPos,
                        lp.IL,
                        lp.m
                    );

                    plane.SetPixel(x, y, shadedColor);
                }

                ae1.X += ae1.Diff;
                ae2.X += ae2.Diff;
            }
        }

        private static void HandleNewPoints(
            Vec2[] points,
            UInt32[] colors,
            int[] perm,
            ref int nextToProcess,
            List<ActiveEdge> aet,
            int y
        ) {
            while((int)Math.Round(points[perm[nextToProcess]].Y) == y - 1) {
                int nextPoint = (perm[nextToProcess] + 1) % points.Length;
                int prevPoint = perm[nextToProcess] == 0 ? points.Length - 1 : perm[nextToProcess] - 1;

                HandleNewEdge(points, colors, aet, perm[nextToProcess], prevPoint);
                HandleNewEdge(points, colors, aet, perm[nextToProcess], nextPoint);

                nextToProcess += 1;
            }
        }

        public static void FillSolidColor(DrawingPlane plane, Vec2[] points, UInt32 color, LightPackage lp) {
            UInt32[] colors = Enumerable.Repeat<UInt32>(color, points.Length).ToArray();
            FillVertexInterpolation(plane, points, colors, lp);
        }

        public static void FillVertexInterpolation(DrawingPlane plane, Vec2[] points, UInt32[] colors, LightPackage lp) {
            Fill(plane, points, colors, null, null, lp);
        }

        public static void FillTexture(DrawingPlane plane, Vec2[] points, Bitmap texture, Bitmap heightmap, LightPackage lp) {
            UInt32[] colors = Enumerable.Repeat<UInt32>(0xFF000000, points.Length).ToArray();
            Fill(plane, points, colors, texture, heightmap, lp);
        }

        public static void Fill(DrawingPlane plane, Vec2[] points, UInt32[] colors, Bitmap texture, Bitmap heightmap, LightPackage lp) {
            int[] perm = Enumerable.Range(0, points.Length).ToArray();
            Array.Sort(perm, (int a, int b) => points[a].Y.CompareTo(points[b].Y));
            (List<ActiveEdge> aet, int nextToProcess) = InitAET(points, colors, perm);

            int ymin = (int)Math.Round(points[perm[0]].Y);
            int ymax = (int)Math.Round(points[perm[points.Length - 1]].Y);

            if(texture == null) {
                for(int y = ymin + 1; y <= ymax; ++y) {
                    HandleNewPoints(points, colors, perm, ref nextToProcess, aet, y);

                    aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                    DrawScanline(plane, aet, y, lp);
                }
            }
            else {
                GraphicsUnit units = GraphicsUnit.Pixel;
                RectangleF textureBounds = texture.GetBounds(ref units);
                RectangleF? heightBounds = null;
                if(heightmap != null) {
                    heightBounds= heightmap.GetBounds(ref units);
                }
                (int, int) mins = GetBound(points);
                for(int y = ymin + 1; y <= ymax; ++y) {
                    HandleNewPoints(points, colors, perm, ref nextToProcess, aet, y);

                    aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                    DrawTexturedScanline(plane, aet, y, texture, heightmap, mins.Item1, mins.Item2, textureBounds, heightBounds, lp);
                }
            }
        }
    }
}
