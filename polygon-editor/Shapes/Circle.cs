using System;

namespace polygon_editor {
    public class Circle : Shape {
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public UInt32 Color { get; set; }

        public Circle() {

        }

        public override string ToString() {
            return $"Circle {Index}";
        }

        public override void DrawOn(DrawingPlane plane) {
            BresenhamDrawer.Circle(plane, Color, R, X, Y);
        }
    }
}
