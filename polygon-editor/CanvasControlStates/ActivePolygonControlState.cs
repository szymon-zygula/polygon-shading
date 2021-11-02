using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace polygon_editor{
    class ActivePolygonControlState : CanvasControlState {
        readonly Polygon ActivePolygon;
        readonly MainWindow MainWindow;

        public ActivePolygonControlState(CanvasState state, Polygon polygon, MainWindow mainWindow) : base(state) {
            ActivePolygon = polygon;
            MainWindow = mainWindow;
        }

        public override void EnterState() {
            MainWindow.ButtonColorPolygon.IsEnabled = true;
            MainWindow.ButtonColorPolygon.Click += ButtonColorPolygon_Click;
            MainWindow.ButtonColorVertex.IsEnabled = true;
            MainWindow.ButtonColorVertex.Click += ButtonColorVertex_Click;
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            double mouseX = e.GetPosition(State.Canvas).X;
            double mouseY = e.GetPosition(State.Canvas).Y;
            if (TryEnterMovingPolygonState(mouseX, mouseY)) return;
            if (TryEnterMovingEdgeState(mouseX, mouseY)) return;
            if (TryEnterMovingVertexState(mouseX, mouseY)) return;
        }

        private bool TryEnterMovingPolygonState(double mouseX, double mouseY) {
            (int, int) center = ActivePolygon.GetCenter();
            bool isWithinXRadius = Math.Abs(center.Item1 - mouseX) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            bool isWithinYRadius = Math.Abs(center.Item2 - mouseY) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            if (!isWithinXRadius || !isWithinYRadius) return false;
            State.SetControlState(new MovingPolygonControlState(State, ActivePolygon, MainWindow));
            return true;
        }

        private bool TryEnterMovingEdgeState(double mouseX, double mouseY) {
            int? activeEdge = ActivePolygon.FindEdgeWithinSquareRadius(
                mouseX, mouseY, CanvasOptions.ACTIVE_EDGE_RADIUS);
            if (activeEdge == null) return false;
            State.SetControlState(new MovingEdgeControlState(State, ActivePolygon, activeEdge.Value, MainWindow));
            return true;
        }

        private bool TryEnterMovingVertexState(double mouseX, double mouseY) {
            int? activeVertex = ActivePolygon.FindVertexWithinRadius(
                mouseX, mouseY, CanvasOptions.ACTIVE_VERTEX_RADIUS);
            if (activeVertex == null) return false;
            State.SetControlState(new MovingVertexControlState(State, ActivePolygon, activeVertex.Value, MainWindow));
            return true;
        }

        public override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            double mouseX = e.GetPosition(State.Canvas).X;
            double mouseY = e.GetPosition(State.Canvas).Y;
            if (TryRemoveActivePolygonVertex(mouseX, mouseY)) return;
            if (TryRemoveActivePolygon(mouseX, mouseY)) return;
        }

        private bool TryRemoveActivePolygon(double mouseX, double mouseY) {
            (int, int) center = ActivePolygon.GetCenter();
            bool isWithinXRange = Math.Abs(center.Item1 - mouseX) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            bool isWithinYRange = Math.Abs(center.Item2 - mouseY) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            if (!isWithinXRange || !isWithinYRange) return false;
            RemoveActivePolygon();
            return true;
        }

        private void RemoveActivePolygon() {
            State.Polygons.Remove(ActivePolygon);
            State.ShapeList.Items.Remove(ActivePolygon);
            State.SetControlState(new DoingNothingControlState(State));
        }

        private bool TryRemoveActivePolygonVertex(double mouseX, double mouseY) {
            int? activeVertex = ActivePolygon.FindVertexWithinRadius(
                mouseX, mouseY, CanvasOptions.ACTIVE_VERTEX_RADIUS);
            if (activeVertex == null) return false;
            ActivePolygon.RemoveNthPoint(activeVertex.Value);
            if (ActivePolygon.Points.Length < 3) RemoveActivePolygon();
            State.UpdateCanvas();
            return true;
        }

        public override void OnMouseMiddleButtonDown(MouseButtonEventArgs e) {
            double mouseX = e.GetPosition(State.Canvas).X;
            double mouseY = e.GetPosition(State.Canvas).Y;
            int? edge = ActivePolygon.FindEdgeWithinSquareRadius(mouseX, mouseY, CanvasOptions.ACTIVE_EDGE_RADIUS);
            if (edge == null) return;
            (int, int) midpoint = ActivePolygon.EdgeMidpoint(edge.Value);
            ActivePolygon.InsertPointAt(midpoint.Item1, midpoint.Item2, edge.Value + 1);
            State.UpdateCanvas();
        }

        public override void DrawStateFeatures() {
            State.Plane.MarkPolygonVertices(
                CanvasOptions.ACTIVE_VERTEX_RADIUS,
                ActivePolygon
            );

            State.Plane.MarkPolygonEdges(
                CanvasOptions.ACTIVE_EDGE_COLOR,
                CanvasOptions.ACTIVE_VERTEX_RADIUS,
                ActivePolygon
            );

            State.Plane.MarkPolygonCenter(
                CanvasOptions.ACTIVE_CENTER_COLOR,
                CanvasOptions.ACTIVE_VERTEX_RADIUS,
                ActivePolygon
            );
        }

        public override void ExitState() {
            MainWindow.ButtonColorPolygon.IsEnabled = false;
            MainWindow.ButtonColorPolygon.Click -= ButtonColorPolygon_Click;
            MainWindow.ButtonColorVertex.IsEnabled = false;
            MainWindow.ButtonColorVertex.Click -= ButtonColorVertex_Click;
        }

        private void ButtonColorVertex_Click(object sender, RoutedEventArgs e) {
            State.SetControlState(new ColoringVertexControlState(State, ActivePolygon, MainWindow));
        }

        private void ButtonColorPolygon_Click(object sender, RoutedEventArgs e) {
            UInt32? newColor = InterfaceUtils.GetColorFromDialog();
            if(newColor.HasValue) {
                ActivePolygon.Color = newColor.Value;
                State.UpdateCanvas();
            }
        }
    }
}
