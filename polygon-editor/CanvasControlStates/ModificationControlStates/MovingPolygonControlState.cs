using System.Windows.Input;

namespace polygon_editor{
    class MovingPolygonControlState : CanvasControlState {
        readonly Polygon MovedPolygon;
        readonly MainWindow MainWindow;

        public MovingPolygonControlState(CanvasState state, Polygon polygon, MainWindow mainWindow) : base(state) {
            MovedPolygon = polygon;
            MainWindow = mainWindow;
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
            State.SetControlState(new ActivePolygonControlState(State, MovedPolygon, MainWindow));
        }
    }
}
