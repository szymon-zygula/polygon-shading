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
        }

        private static double Differential((int, int)[] points, int i, int j) {
            if(points[i].Item2 == points[j].Item2) {
                return double.NaN;
            }

            return
                (double)(points[j].Item1 - points[i].Item1) /
                (double)(points[j].Item2 - points[i].Item2);
        }

        private static void AddToAET(List<ActiveEdge> aet, ActiveEdge ae) {
            if(!double.IsNaN(ae.Diff)) {
                aet.Add(ae);
            }
        }

        private static (List<ActiveEdge>, int) InitAET((int, int)[] points, int[] perm) {
            List<ActiveEdge> aet = new List<ActiveEdge>();

            for(int i = 0; i < points.Length; ++i) {
                if(points[perm[i]].Item2 != points[perm[0]].Item2) {
                    return (aet, i);
                }

                int x = points[perm[i]].Item1;
                int nextPoint = (perm[i] + 1) % points.Length;
                int prevPoint = perm[i] == 0 ? points.Length - 1 : perm[i] - 1;

                AddToAET(aet, new ActiveEdge() {
                    X = x,
                    Diff = Differential(points, perm[i], nextPoint),
                    From = perm[i],
                    To = nextPoint
                });

                AddToAET(aet, new ActiveEdge() {
                    X = x,
                    Diff = Differential(points, perm[i], prevPoint),
                    From = perm[i],
                    To = prevPoint
                });
            }

            return (aet, points.Length);
        }

        private static void HandleNewEdge((int, int)[] points, List<ActiveEdge> aet, int i, int j) {
            if(points[j].Item2 >= points[i].Item2) {
                AddToAET(aet, new ActiveEdge() {
                    X = points[i].Item1,
                    Diff = Differential(points, i, j),
                    From = i,
                    To = j
                });
            }
            else {
                aet.RemoveAll((ActiveEdge ae) => ae.To == i && ae.From == j);
            }
        }

        private static void DrawScanline(DrawingPlane plane, List<ActiveEdge> aet, UInt32 color, int y) {
            for(int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];

                for(int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    plane.SetPixel(x, y, color);
                }

                ae1.X += ae1.Diff;
                ae2.X += ae2.Diff;
            }
        }

        public static void FillSolidColor(DrawingPlane plane, (int, int)[] points, UInt32 color) {
            int[] perm = Enumerable.Range(0, points.Length).ToArray();
            Array.Sort(perm, (int a, int b) => points[a].Item2.CompareTo(points[b].Item2));
            (List<ActiveEdge> aet, int nextToProcess) = InitAET(points, perm);

            int ymin = points[perm[0]].Item2;
            int ymax = points[perm[points.Length - 1]].Item2;

            for(int y = ymin; y <= ymax; ++y) {
                while(points[perm[nextToProcess]].Item2 == y - 1) {
                    int nextPoint = (perm[nextToProcess] + 1) % points.Length;
                    int prevPoint = perm[nextToProcess] == 0 ? points.Length - 1 : perm[nextToProcess] - 1;

                    HandleNewEdge(points, aet, perm[nextToProcess], prevPoint);
                    HandleNewEdge(points, aet, perm[nextToProcess], nextPoint);

                    nextToProcess += 1;
                }

                aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                DrawScanline(plane, aet, color, y);
            }
        }
    }
}
