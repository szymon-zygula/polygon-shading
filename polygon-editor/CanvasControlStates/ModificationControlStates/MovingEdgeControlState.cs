using System.Windows.Input;

namespace polygon_editor {
    class MovingEdgeControlState : CanvasControlState {
        readonly Polygon ActivePolygon;
        readonly int EdgeIdx;
        readonly MainWindow MainWindow;

        public MovingEdgeControlState(CanvasState state, Polygon polygon, int edgeIdx, MainWindow mainWindow) : base(state) {
            ActivePolygon = polygon;
            EdgeIdx = edgeIdx;
            MainWindow = mainWindow;
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
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon, MainWindow));
        }
    }
}
