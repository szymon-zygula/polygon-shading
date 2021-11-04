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
            Vec2 center = MovedPolygon.GetCenter();
            double deltaX = e.GetPosition(State.Canvas).X - center.X;
            double deltaY = e.GetPosition(State.Canvas).Y - center.Y;
            for (int i = 0; i < MovedPolygon.Points.Length; ++i) {
                MovedPolygon.Points[i].X += deltaX;
                MovedPolygon.Points[i].Y += deltaY;
            }
            State.UpdateCanvas();
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, MovedPolygon, MainWindow));
        }
    }
}
