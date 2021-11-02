using System;
using System.Linq;

namespace polygon_editor {
    public class Polygon : Shape {
        public (int, int)[] Points { get; set; }
        public UInt32 Color { get; set; }

        public Polygon() {
            Points = new (int, int)[0];
        }

        public (int, int) EdgeMidpoint(int n) {
            int idx1 = n;
            int idx2 = n == Points.Length - 1 ? 0 : n + 1;
            return ((Points[idx1].Item1 + Points[idx2].Item1) / 2, (Points[idx1].Item2 + Points[idx2].Item2) / 2);
        }

        public void AddPoint(int x, int y) {
            InsertPointAt(x, y, Points.Length);
        }

        public void InsertPointAt(int x, int y, int n) {
            Points = InsertElemAt((x, y), Points, n);
        }

        private T[] InsertElemAt<T>(T elem, T[] array, int n) {
            T[] newElems = new T[array.Length + 1];
            Array.Copy(array, newElems, n);
            Array.Copy(array, n, newElems, n + 1, array.Length - n);
            newElems[n] = elem;
            return newElems;
        }

        public void RemoveNthPoint(int n) {
            Points = RemoveNthElem(n, Points);
        }

        private T[] RemoveNthElem<T>(int n, T[] array) {
            T[] newElems = new T[array.Length - 1];
            Array.Copy(array, newElems, n);
            Array.Copy(array, n + 1, newElems, n, array.Length - n - 1);
            return newElems;
        }

        public void RemoveLastPoint() {
            RemoveNthPoint(Points.Length - 1);
        }

        public int? FindVertexWithinRadius(double x0, double y0, int radius) {
            for (int i = 0; i < Points.Length; ++i) {
                double x = Points[i].Item1 - x0;
                double y = Points[i].Item2 - y0;
                if (x * x + y * y < radius * radius) {
                    return i;
                }
            }

            return null;
        }

        public int EdgeLength(int n) {
            int idx1 = n;
            int idx2 = n == Points.Length - 1 ? 0 : n + 1;
            int x = Points[idx1].Item1 - Points[idx2].Item1;
            int y = Points[idx1].Item2 - Points[idx2].Item2;
            return (int)Math.Sqrt(x * x + y * y);
        }

        public int? FindEdgeWithinSquareRadius(double x0, double y0, int radius) {
            for (int i = 0; i < Points.Length; ++i) {
                (int, int) mid = EdgeMidpoint(i);
                if (Math.Abs(mid.Item1 - x0) <= radius && Math.Abs(mid.Item2 - y0) <= radius) {
                    return i;
                }
            }

            return null;
        }

        public (int, int) GetCenter() {
            int avgX = 0;
            int avgY = 0;
            int perimeter = 0;

            for (int i = 0; i < Points.Length; ++i) {
                int len = EdgeLength(i);
                (int, int) mid = EdgeMidpoint(i);
                avgX += mid.Item1 * len;
                avgY += mid.Item2 * len;
                perimeter += len;
            }

            avgX /= perimeter;
            avgY /= perimeter;

            return (avgX, avgY);
        }

        public override string ToString() {
            return $"Polygon {Index}";
        }

        public void DrawIncompleteOn(DrawingPlane plane) {
            for (int i = 1; i < Points.Length; ++i) {
                BresenhamDrawer.Line(
                    plane, Color,
                    Points[i - 1].Item1, Points[i - 1].Item2,
                    Points[i].Item1, Points[i].Item2
                );
            }
        }

        public override void DrawOn(DrawingPlane plane) {
            DrawIncompleteOn(plane);
            BresenhamDrawer.Line(
                plane, Color,
                Points.Last().Item1, Points.Last().Item2,
                Points.First().Item1, Points.First().Item2
            );
        }

        public (double, double) EdgeVector(int edge1, int edge2) {
            return (
                Points[edge2].Item1 - Points[edge1].Item1,
                Points[edge2].Item2 - Points[edge1].Item2
            );
        }

        public void MoveVertexToEdgeLength(int vrtx, int edge, int len) {
            int rootVrtx = edge == vrtx ? (edge + 1) % Points.Length : edge;
            var edgeVec = EdgeVector(rootVrtx, vrtx);
            double edgeVecLen = MathUtils.VectorLength(edgeVec);
            var transVec = MathUtils.MulVector(edgeVec, (double)len / edgeVecLen);
            MathUtils.MovePoint(ref Points[vrtx], Points[rootVrtx], transVec);
        }
    }
}
