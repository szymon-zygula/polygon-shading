using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        private static double Differential((int, int)[] points, int i, int j) {
            if(points[i].Item2 == points[j].Item2) {
                return double.NaN;
            }

            return
                (double)(points[j].Item1 - points[i].Item1) /
                (double)(points[j].Item2 - points[i].Item2);
        }

        private static Vec3 ColorDifferential((int, int)[] points, UInt32[] colors, int i, int j) {
            return
                (new Vec3(colors[j]) - new Vec3(colors[i])) /
                (double)(points[j].Item2 - points[i].Item2);
        }

        private static void AddToAET(List<ActiveEdge> aet, ActiveEdge ae) {
            if(!double.IsNaN(ae.Diff)) {
                aet.Add(ae);
            }
        }

        private static void AddTopPointToAET((int, int)[] points, UInt32[] colors, int point, List<ActiveEdge> aet) {
            int x = points[point].Item1;
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

        private static (List<ActiveEdge>, int) InitAET((int, int)[] points, UInt32[] colors, int[] perm) {
            List<ActiveEdge> aet = new List<ActiveEdge>();

            for(int i = 0; i < points.Length; ++i) {
                if(points[perm[i]].Item2 != points[perm[0]].Item2) {
                    return (aet, i);
                }

                AddTopPointToAET(points, colors, perm[i], aet);
            }

            return (aet, points.Length);
        }

        private static void HandleNewEdge((int, int)[] points, UInt32[] colors, List<ActiveEdge> aet, int i, int j) {
            if(points[j].Item2 >= points[i].Item2) {
                AddToAET(aet, new ActiveEdge() {
                    X = points[i].Item1,
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

        private static void DrawScanline(DrawingPlane plane, List<ActiveEdge> aet, int y) {
            for(int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];
                Vec3 colorDiff = (ae2.Color - ae1.Color) / (ae2.X - ae1.X);
                Vec3 color = ae1.Color;

                for(int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    plane.SetPixel(x, y, color.ToColor());
                    color += colorDiff;
                }

                ae1.X += ae1.Diff;
                ae1.Color += ae1.ColorDiff;
                ae2.X += ae2.Diff;
                ae2.Color += ae2.ColorDiff;
            }
        }

        private static void HandleNewPoints((int, int)[] points, UInt32[] colors, int[] perm, ref int nextToProcess, List<ActiveEdge> aet, int y) {
            while(points[perm[nextToProcess]].Item2 == y - 1) {
                int nextPoint = (perm[nextToProcess] + 1) % points.Length;
                int prevPoint = perm[nextToProcess] == 0 ? points.Length - 1 : perm[nextToProcess] - 1;

                HandleNewEdge(points, colors, aet, perm[nextToProcess], prevPoint);
                HandleNewEdge(points, colors, aet, perm[nextToProcess], nextPoint);

                nextToProcess += 1;
            }
        }

        public static void FillSolidColor(DrawingPlane plane, (int, int)[] points, UInt32 color) {
            UInt32[] colors = Enumerable.Repeat<UInt32>(color, points.Length).ToArray();
            FillVertexInterpolation(plane, points, colors);
        }

        public static void FillVertexInterpolation(DrawingPlane plane, (int, int)[] points, UInt32[] colors) {
            int[] perm = Enumerable.Range(0, points.Length).ToArray();
            Array.Sort(perm, (int a, int b) => points[a].Item2.CompareTo(points[b].Item2));
            (List<ActiveEdge> aet, int nextToProcess) = InitAET(points, colors, perm);

            int ymin = points[perm[0]].Item2;
            int ymax = points[perm[points.Length - 1]].Item2;

            for(int y = ymin + 1; y <= ymax; ++y) {
                HandleNewPoints(points, colors, perm, ref nextToProcess, aet, y);

                aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                DrawScanline(plane, aet, y);
            }
        }
    }
}
