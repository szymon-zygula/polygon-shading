using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace polygon_editor {
    public class ColoringVertexControlState : CanvasControlState {
        readonly Polygon ActivePolygon;
        readonly MainWindow MainWindow;

        public ColoringVertexControlState(CanvasState state, Polygon activePolygon, MainWindow mainWindow) : base(state) {
            ActivePolygon = activePolygon;
            MainWindow = mainWindow;
        }

        public override void EnterState() {
            State.Canvas.Cursor = CanvasOptions.DRAW_CURSOR;
        }

        public override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon, MainWindow));
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            double x = e.GetPosition(State.Canvas).X;
            double y = e.GetPosition(State.Canvas).Y;
            int? vertex = ActivePolygon.FindVertexWithinRadius(x, y, CanvasOptions.ACTIVE_VERTEX_RADIUS);
            if (!vertex.HasValue) return;

            UInt32? color = InterfaceUtils.GetColorFromDialog();
            if(color.HasValue) {
                ActivePolygon.VertexColors[vertex.Value] = color.Value;
            }

            State.SetControlState(new ActivePolygonControlState(State, ActivePolygon, MainWindow));
        }

        public override void DrawStateFeatures() {
            State.Plane.MarkPolygonVertices(CanvasOptions.ACTIVE_EDGE_RADIUS, ActivePolygon);
        }

        public override void ExitState() {
            State.Canvas.Cursor = CanvasOptions.NORMAL_CURSOR;
        }
    }
}
