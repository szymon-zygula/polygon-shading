using System;
using System.Linq;

namespace polygon_editor {
    public class Polygon : Shape {
        public Vec2[] Points { get; set; }
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
            Points = new Vec2[0];
            VertexColors = new UInt32[0];
            Color = CanvasOptions.DEFAULT_POLYGON_COLOR;
            Fill = FillType.None;
        }

        public Vec2 EdgeMidpoint(int n) {
            int idx1 = n;
            int idx2 = n == Points.Length - 1 ? 0 : n + 1;
            return new Vec2((Points[idx1].X + Points[idx2].X) / 2, (Points[idx1].Y + Points[idx2].Y) / 2);
        }

        public void AddPoint(Vec2 point) {
            InsertPointAt(point, Points.Length);
        }

        public void InsertPointAt(Vec2 point, int n) {
            Points = ArrayUtils.InsertElemAt(point, Points, n);
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
                double x = Points[i].X - x0;
                double y = Points[i].Y - y0;
                if (x * x + y * y < radius * radius) {
                    return i;
                }
            }

            return null;
        }

        public double EdgeLength(int n) {
            int idx1 = n;
            int idx2 = n == Points.Length - 1 ? 0 : n + 1;
            double x = Points[idx1].X - Points[idx2].X;
            double y = Points[idx1].Y - Points[idx2].Y;
            return Math.Sqrt(x * x + y * y);
        }

        public int? FindEdgeWithinSquareRadius(double x0, double y0, int radius) {
            for (int i = 0; i < Points.Length; ++i) {
                Vec2 mid = EdgeMidpoint(i);
                if (Math.Abs(mid.X - x0) <= radius && Math.Abs(mid.Y - y0) <= radius) {
                    return i;
                }
            }

            return null;
        }

        public Vec2 GetCenter() {
            Vec2 avg = new Vec2();
            double perimeter = 0;

            for (int i = 0; i < Points.Length; ++i) {
                double len = EdgeLength(i);
                Vec2 mid = EdgeMidpoint(i);
                avg += mid * len;
                perimeter += len;
            }

            avg /= perimeter;

            return avg;
        }

        public override string ToString() {
            return $"Polygon {Index}";
        }

        public void DrawIncompleteOn(DrawingPlane plane) {
            for (int i = 1; i < Points.Length; ++i) {
                BresenhamDrawer.Line(
                    plane, Color,
                    Points[i - 1].X, Points[i - 1].Y,
                    Points[i].X, Points[i].Y
                );
            }
        }

        public override void DrawOn(DrawingPlane plane) {
            switch(Fill) {
                case FillType.None:
                    DrawIncompleteOn(plane);
                    BresenhamDrawer.Line(
                        plane, Color,
                        Points.Last().X, Points.Last().Y,
                        Points.First().X, Points.First().Y
                    );
                    break;
                case FillType.VertexIterpolation:
                    ScanLineFiller.FillVertexInterpolation(plane, Points, VertexColors);
                    break;
                case FillType.SolidColor:
                    ScanLineFiller.FillSolidColor(plane, Points, Color);
                    break;
            }
        }

        public void Translate(Vec2 trans) {
            for(int i = 0; i < Points.Length; ++i) {
                Points[i] += trans;
            }
        }
    }
}
