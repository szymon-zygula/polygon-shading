using System;
using System.Linq;

namespace polygon_editor {
    public class Polygon : Shape {
        public (int, int)[] Points { get; set; }
        public UInt32 Color { get; set; }
        public UInt32[] VertexColors { get; set; }

        public enum FillType {
            None,
            SolidColor,
            VertexIterpolation,
            Texture
        };

        public FillType Fill;

        public static FillType GetSelectedFillType(MainWindow mainWindow) {
            var selected = mainWindow.ComboBoxPolygonFill.SelectedItem;
            if (selected == mainWindow.CBISolidColor) return FillType.SolidColor;
            if (selected == mainWindow.CBIVertexInterpolation) return FillType.VertexIterpolation;
            if (selected == mainWindow.CBITexture) return FillType.Texture;
            return FillType.None;
        }

        public static object GetFillTypeSelection(MainWindow mainWindow, FillType fill) {
            switch(fill) {
                case FillType.SolidColor: return mainWindow.CBISolidColor;
                case FillType.VertexIterpolation: return mainWindow.CBIVertexInterpolation;
                case FillType.Texture: return mainWindow.CBITexture;
            }
            return null;
        }

        public Polygon() {
            Points = new (int, int)[0];
            VertexColors = new UInt32[0];
            Color = CanvasOptions.DEFAULT_POLYGON_COLOR;
            Fill = FillType.None;
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
            Points = ArrayUtils.InsertElemAt((x, y), Points, n);
            VertexColors = ArrayUtils.InsertElemAt(CanvasOptions.DEFAULT_VERTEX_COLOR, VertexColors, n);
        }

        public void RemoveNthPoint(int n) {
            Points = ArrayUtils.RemoveNthElem(n, Points);
            VertexColors = ArrayUtils.RemoveNthElem(n, VertexColors);
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
            switch(Fill) {
                case FillType.None:
                    DrawIncompleteOn(plane);
                    BresenhamDrawer.Line(
                        plane, Color,
                        Points.Last().Item1, Points.Last().Item2,
                        Points.First().Item1, Points.First().Item2
                    );
                    break;
                case FillType.SolidColor:
                    ScanLineFiller.FillSolidColor(plane, Points, Color);
                    break;
            }
        }

        public (double, double) EdgeVector(int edge1, int edge2) {
            return (
                Points[edge2].Item1 - Points[edge1].Item1,
                Points[edge2].Item2 - Points[edge1].Item2
            );
        }
    }
}
