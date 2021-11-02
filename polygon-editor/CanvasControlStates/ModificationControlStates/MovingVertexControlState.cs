using System.Windows.Input;

namespace polygon_editor {
    class MovingVertexControlState : CanvasControlState {
        readonly Polygon ActivePolygon;
        readonly int VertexIdx;
        readonly MainWindow MainWindow;

        public MovingVertexControlState(CanvasState state, Polygon polygon, int vertexIdx, MainWindow mainWindow) : base(state) {
            ActivePolygon = polygon;
            VertexIdx = vertexIdx;
            MainWindow = mainWindow;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon, MainWindow));
        }

        public override void OnMouseMove(MouseEventArgs e) {
            ActivePolygon.Points[VertexIdx] = (
                (int)e.GetPosition(State.Canvas).X,
                (int)e.GetPosition(State.Canvas).Y);
            State.UpdateCanvas();
        }
    }
}
