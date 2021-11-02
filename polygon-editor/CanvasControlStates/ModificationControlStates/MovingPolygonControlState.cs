using System.Windows.Input;

namespace polygon_editor{
    class MovingPolygonControlState : CanvasControlState {
        readonly Polygon MovedPolygon;
        public MovingPolygonControlState(CanvasState state, Polygon polygon) : base(state) {
            MovedPolygon = polygon;
        }

        public override void EnterState() {
            MovedPolygon.Color = CanvasOptions.CHANGING_LINE_COLOR;
        }

        public override void OnMouseMove(MouseEventArgs e) {
            (int, int) center = MovedPolygon.GetCenter();
            int deltaX = (int)e.GetPosition(State.Canvas).X - center.Item1;
            int deltaY = (int)e.GetPosition(State.Canvas).Y - center.Item2;
            for (int i = 0; i < MovedPolygon.Points.Length; ++i) {
                MovedPolygon.Points[i].Item1 += deltaX;
                MovedPolygon.Points[i].Item2 += deltaY;
            }
            State.UpdateCanvas();
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, MovedPolygon));
        }

        public override void ExitState() {
            MovedPolygon.Color = CanvasOptions.NORMAL_LINE_COLOR;
        }
    }
}
