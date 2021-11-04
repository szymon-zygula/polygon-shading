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
            Vec2 mid = ActivePolygon.EdgeMidpoint(EdgeIdx);
            double deltaX = e.GetPosition(State.Canvas).X - mid.X;
            double deltaY = e.GetPosition(State.Canvas).Y - mid.Y;
            int idx1 = EdgeIdx;
            int idx2 = EdgeIdx == ActivePolygon.Points.Length - 1
                ? 0
                : EdgeIdx + 1;
            ActivePolygon.Points[idx1].X += deltaX;
            ActivePolygon.Points[idx1].Y += deltaY;
            ActivePolygon.Points[idx2].X += deltaX;
            ActivePolygon.Points[idx2].Y += deltaY;
            State.UpdateCanvas();
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon, MainWindow));
        }
    }
}
