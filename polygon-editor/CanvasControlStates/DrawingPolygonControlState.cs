using System.Windows.Input;

namespace polygon_editor {
    class DrawingPolygonControlState : CanvasControlState {
        readonly Polygon DrawnPolygon;

        public DrawingPolygonControlState(CanvasState state) : base(state) {
            DrawnPolygon = new Polygon {
                Color = CanvasOptions.DRAWN_POLYGON_COLOR
            };
        }

        public override void EnterState() {
            State.Canvas.Cursor = CanvasOptions.DRAW_CURSOR;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            double mouseX = e.GetPosition(State.Canvas).X;
            double mouseY = e.GetPosition(State.Canvas).Y;

            if (DrawnPolygon.Points.Length == 0) {
                DrawnPolygon.AddPoint(new Vec2(mouseX, mouseY));
            }

            DrawnPolygon.AddPoint(new Vec2(mouseX, mouseY));
            State.UpdateCanvas();
        }

        public override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new DoingNothingControlState(State));
        }

        public override void OnMouseMove(MouseEventArgs e) {
            if (DrawnPolygon.Points.Length == 0) return;
            DrawnPolygon.Points[DrawnPolygon.Points.Length - 1] = new Vec2(
                e.GetPosition(State.Canvas).X, e.GetPosition(State.Canvas).Y
            );
            State.UpdateCanvas();
        }

        public override void DrawStateFeatures() {
            DrawnPolygon.DrawIncompleteOn(State.Plane);
        }

        public override void ExitState() {
            State.Canvas.Cursor = CanvasOptions.NORMAL_CURSOR;

            if (DrawnPolygon.Points.Length >= 4) {
                DrawnPolygon.Color = CanvasOptions.DEFAULT_POLYGON_COLOR;
                DrawnPolygon.RemoveLastPoint();
                DrawnPolygon.Fill = Polygon.FillType.SolidColor;
                State.AddPolygon(DrawnPolygon);
            }

            State.UpdateCanvas();
        }
    }
}
