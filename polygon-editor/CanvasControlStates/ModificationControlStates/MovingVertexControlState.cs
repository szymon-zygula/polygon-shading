using System.Windows.Input;

namespace polygon_editor {
    class MovingVertexControlState : CanvasControlState {
        Polygon ActivePolygon;
        int VertexIdx;

        public MovingVertexControlState(CanvasState state, Polygon polygon, int vertexIdx) : base(state) {
            ActivePolygon = polygon;
            VertexIdx = vertexIdx;
        }

        public override void EnterState() {
            ActivePolygon.Color = CanvasOptions.CHANGING_LINE_COLOR;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon));
        }

        public override void OnMouseMove(MouseEventArgs e) {
            ActivePolygon.Points[VertexIdx] = (
                (int)e.GetPosition(State.Canvas).X,
                (int)e.GetPosition(State.Canvas).Y);
            State.UpdateCanvas();
        }

        public override void ExitState() {
            ActivePolygon.Color = CanvasOptions.NORMAL_LINE_COLOR;
        }
    }
}
