using System.Windows.Input;

namespace polygon_editor {
    class MovingEdgeControlState : CanvasControlState {
        Polygon ActivePolygon;
        int EdgeIdx;

        public MovingEdgeControlState(CanvasState state, Polygon polygon, int edgeIdx) : base(state) {
            ActivePolygon = polygon;
            EdgeIdx = edgeIdx;
        }

        public override void EnterState() {
            ActivePolygon.Color = CanvasOptions.CHANGING_LINE_COLOR;
        }

        public override void OnMouseMove(MouseEventArgs e) {
            (int, int) mid = ActivePolygon.EdgeMidpoint(EdgeIdx);
            int deltaX = (int)e.GetPosition(State.Canvas).X - mid.Item1;
            int deltaY = (int)e.GetPosition(State.Canvas).Y - mid.Item2;
            int idx1 = EdgeIdx;
            int idx2 = EdgeIdx == ActivePolygon.Points.Length - 1
                ? 0
                : EdgeIdx + 1;
            ActivePolygon.Points[idx1].Item1 += deltaX;
            ActivePolygon.Points[idx1].Item2 += deltaY;
            ActivePolygon.Points[idx2].Item1 += deltaX;
            ActivePolygon.Points[idx2].Item2 += deltaY;
            State.UpdateCanvas();
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon));
        }

        public override void ExitState() {
            ActivePolygon.Color = CanvasOptions.NORMAL_LINE_COLOR;
        }
    }
}
