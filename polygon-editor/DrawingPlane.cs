using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace polygon_editor {
    public class DrawingPlane {
        public int Width { get; }
        public int Height { get; }
        private readonly UInt32[] Pixels;

        public DrawingPlane(int width, int height) {
            Width = width;
            Height = height;
            Pixels = new UInt32[width * height];
        }

        public void SetPixel(int x, int y, UInt32 color) {
            if(x < 0 || x >= Width || y < 0 || y >= Height) {
                return;
            }

            Pixels[x + y * Width] = color;
        }

        public void Fill(UInt32 color) {
            for (int i = 0; i < Width; ++i) {
                for (int j = 0; j < Height; ++j) {
                    Pixels[i + j * Width] = color;
                }
            }
        }

        public void Draw(Shape shape, ScanLineFiller.LightPackage lp) {
            shape.DrawOn(this, lp);
        }

        private Polygon CreateSquare(UInt32 color, double x, double y, int r) {
            Polygon square = new Polygon();
            square.AddPoint(new Vec2(x + r, y + r));
            square.AddPoint(new Vec2(x - r, y + r));
            square.AddPoint(new Vec2(x - r, y - r));
            square.AddPoint(new Vec2(x + r, y - r));
            square.Color = color;
            return square;
        }

        public void MarkPolygonEdges(UInt32 color, int r, Polygon polygon) {
            for (int i = 0; i < polygon.Points.Length; ++i) {
                MarkPolygonEdge(color, r, polygon, i);
            }
        }

        public void MarkPolygonEdge(UInt32 color, int r, Polygon polygon, int edge) {
            Vec2 mid = polygon.EdgeMidpoint(edge);
            Polygon square = CreateSquare(color, mid.X, mid.Y, r);
            Draw(square, null);
        }

        public void MarkPolygonVertices(int r, Polygon polygon) {
            for(int i = 0; i < polygon.Points.Length; ++i) {
                BresenhamDrawer.Circle(
                    this,
                    polygon.VertexColors[i],
                    r,
                    polygon.Points[i].X,
                    polygon.Points[i].Y
                );
            }
        }

        public void MarkPolygonCenter(UInt32 color, int r, Polygon polygon) {
            Vec2 center = polygon.GetCenter();
            Polygon square = CreateSquare(color, center.X, center.Y, r);
            Draw(square, null);
        }

        public BitmapSource CreateBitmapSource() {
            return BitmapSource.Create(
                Width,
                Height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                Pixels,
                Width * 4
            );
        }
    }
}
