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

        public void Draw(Shape shape) {
            shape.DrawOn(this);
        }

        private Polygon CreateSquare(UInt32 color, int x, int y, int r) {
            Polygon square = new Polygon();
            square.AddPoint(x + r, y + r);
            square.AddPoint(x - r, y + r);
            square.AddPoint(x - r, y - r);
            square.AddPoint(x + r, y - r);
            square.Color = color;
            return square;
        }

        public void MarkPolygonEdges(UInt32 color, int r, Polygon polygon) {
            for (int i = 0; i < polygon.Points.Length; ++i) {
                MarkPolygonEdge(color, r, polygon, i);
            }
        }

        public void MarkPolygonEdge(UInt32 color, int r, Polygon polygon, int edge) {
            (int, int) mid = polygon.EdgeMidpoint(edge);
            Polygon square = CreateSquare(color, mid.Item1, mid.Item2, r);
            Draw(square);
        }

        public void MarkPolygonVertices(int r, Polygon polygon) {
            for(int i = 0; i < polygon.Points.Length; ++i) {
                BresenhamDrawer.Circle(
                    this,
                    polygon.VertexColors[i],
                    r,
                    polygon.Points[i].Item1,
                    polygon.Points[i].Item2
                );
            }
        }

        public void MarkPolygonCenter(UInt32 color, int r, Polygon polygon) {
            (int, int) center = polygon.GetCenter();
            Polygon square = CreateSquare(color, center.Item1, center.Item2, r);
            Draw(square);
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
