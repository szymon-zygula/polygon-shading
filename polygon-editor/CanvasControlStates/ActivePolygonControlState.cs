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

        private void SetControlsStatus(bool state) {
            MainWindow.ButtonColorPolygon.IsEnabled = state;
            MainWindow.ButtonColorVertex.IsEnabled = state;
            MainWindow.ComboBoxPolygonFill.IsEnabled = state;
            MainWindow.ButtonPolygonTexture.IsEnabled = state;
            MainWindow.ButtonPolygonHeightMap.IsEnabled = state;
            MainWindow.SliderDiffuseComponent.IsEnabled = state;
            MainWindow.SliderSpecularComponent.IsEnabled = state;
            MainWindow.SliderSpecularExponent.IsEnabled = state;
        }

        private void AddControlsHandlers() {
            MainWindow.ComboBoxPolygonFill.SelectionChanged += ComboBoxPolygonFill_SelectionChanged;
            MainWindow.ButtonColorPolygon.Click += ButtonColorPolygon_Click;
            MainWindow.ButtonColorVertex.Click += ButtonColorVertex_Click;
            MainWindow.ButtonPolygonTexture.Click += ButtonPolygonTexture_Click;
            MainWindow.ButtonPolygonHeightMap.Click += ButtonPolygonHeightMap_Click;
            MainWindow.SliderDiffuseComponent.ValueChanged += SliderDiffuseComponent_ValueChanged;
            MainWindow.SliderSpecularComponent.ValueChanged += SliderSpecularCompoment_ValueChanged;
            MainWindow.SliderSpecularExponent.ValueChanged += SliderSpecularExponent_ValueChanged;
        }

        private void SetControlsValues() {
            MainWindow.ComboBoxPolygonFill.SelectedItem =
                Polygon.GetFillTypeSelection(MainWindow, ActivePolygon.Fill);
            MainWindow.SliderDiffuseComponent.Value = ActivePolygon.DiffuseComponent;
            MainWindow.SliderSpecularComponent.Value = ActivePolygon.SpecularComponent;
            MainWindow.SliderSpecularExponent.Value = ActivePolygon.SpecularExponent;
        }

        public override void EnterState() {
            SetControlsStatus(true);
            SetControlsValues();
            AddControlsHandlers();
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            double mouseX = e.GetPosition(State.Canvas).X;
            double mouseY = e.GetPosition(State.Canvas).Y;
            if (TryEnterMovingPolygonState(mouseX, mouseY)) return;
            if (TryEnterMovingEdgeState(mouseX, mouseY)) return;
            if (TryEnterMovingVertexState(mouseX, mouseY)) return;
        }

        private bool TryEnterMovingPolygonState(double mouseX, double mouseY) {
            Vec2 center = ActivePolygon.GetCenter();
            bool isWithinXRadius = Math.Abs(center.X - mouseX) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            bool isWithinYRadius = Math.Abs(center.Y - mouseY) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
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
            Vec2 center = ActivePolygon.GetCenter();
            bool isWithinXRange = Math.Abs(center.X - mouseX) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            bool isWithinYRange = Math.Abs(center.Y - mouseY) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
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
            Vec2 midpoint = ActivePolygon.EdgeMidpoint(edge.Value);
            ActivePolygon.InsertPointAt(midpoint, edge.Value + 1);
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

        private void RemoveControlsHandlers() {
            MainWindow.ComboBoxPolygonFill.SelectionChanged -= ComboBoxPolygonFill_SelectionChanged;
            MainWindow.ButtonColorPolygon.Click -= ButtonColorPolygon_Click;
            MainWindow.ButtonColorVertex.Click -= ButtonColorVertex_Click;
            MainWindow.ButtonPolygonTexture.Click -= ButtonPolygonTexture_Click;
            MainWindow.ButtonPolygonHeightMap.Click -= ButtonPolygonHeightMap_Click;
            MainWindow.SliderDiffuseComponent.ValueChanged -= SliderDiffuseComponent_ValueChanged;
            MainWindow.SliderSpecularComponent.ValueChanged -= SliderSpecularCompoment_ValueChanged;
            MainWindow.SliderSpecularExponent.ValueChanged -= SliderSpecularExponent_ValueChanged;
        }

        public void ResetControlsValues() {
            MainWindow.SliderDiffuseComponent.Value = 0.5;
            MainWindow.SliderSpecularComponent.Value = 0.5;
            MainWindow.SliderSpecularExponent.Value = 50;
            MainWindow.ComboBoxPolygonFill.SelectedItem = null;
        }

        public override void ExitState() {
            SetControlsStatus(false);
            RemoveControlsHandlers();
            ResetControlsValues();
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

        private void ComboBoxPolygonFill_SelectionChanged(object sender, EventArgs e) {
            ActivePolygon.Fill = Polygon.GetSelectedFillType(MainWindow);
            State.UpdateCanvas();
        }

        private void SliderSpecularExponent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ActivePolygon.SpecularExponent = MainWindow.SliderSpecularExponent.Value;
            State.UpdateCanvas();
        }

        private void SliderSpecularCompoment_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ActivePolygon.SpecularComponent = MainWindow.SliderSpecularComponent.Value;
            State.UpdateCanvas();
        }

        private void SliderDiffuseComponent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ActivePolygon.DiffuseComponent = MainWindow.SliderDiffuseComponent.Value;
            State.UpdateCanvas();
        }

        private void ButtonPolygonHeightMap_Click(object sender, RoutedEventArgs e) {
            State.UpdateCanvas();
        }

        private void ButtonPolygonTexture_Click(object sender, RoutedEventArgs e) {
            State.UpdateCanvas();
        }

    }
}
