using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace polygon_editor {
    public class CanvasState {
        public DrawingPlane Plane { get; }

        public Image Canvas { get; }
        public ListView ShapeList { get; }

        public int TotalPolygonCount { get; set; }

        public List<Polygon> Polygons { get; }

        public CanvasControlState ControlState { get; private set; }

        public LightIcon LightShape;
        public double LightHeight;

        public double MaxAnimationSpeed;
        public double MinAnimationSpeed;

        public CanvasState(Image canvas, ListView shapeList) {
            Canvas = canvas;
            ShapeList = shapeList;
            TotalPolygonCount = 0;
            Polygons = new List<Polygon>();
            Plane = new DrawingPlane((int)Canvas.Width, (int)Canvas.Height);
            ControlState = new DoingNothingControlState(this);
            LightShape = new LightIcon(
                CanvasOptions.DEFAULT_LIGHT_COLOR,
                400, 400,
                CanvasOptions.LIGHT_ICON_RAYS,
                CanvasOptions.LIGHT_ICON_RADIUS
            );

            MaxAnimationSpeed = 0.5;
            MinAnimationSpeed = 0.5;

            Plane.Fill(CanvasOptions.BACKGROUND_COLOR);
            Canvas.Source = Plane.CreateBitmapSource();
            LightHeight = 100;
        }

        public (int?, Polygon) FindAnyEdgeWithinMouse(double mouseX, double mouseY) {
            int? activeEdge = null;
            Polygon activePolygon = null;
            foreach(Polygon polygon in Polygons) {
                activePolygon = polygon;
                activeEdge = polygon.FindEdgeWithinSquareRadius(
                    mouseX, mouseY,
                    CanvasOptions.ACTIVE_EDGE_RADIUS
                );
                if (activeEdge != null) break;
            }

            return (activeEdge, activePolygon);
        }

        public bool IsWithinCircleCenterMouse(Circle circle, double mouseX, double mouseY) {
            bool isWithinXRange = Math.Abs(circle.X - mouseX) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            bool isWithinYRange = Math.Abs(circle.Y - mouseY) <= CanvasOptions.ACTIVE_VERTEX_RADIUS;
            return isWithinXRange && isWithinYRange;
        }

        public void AddPolygon(Polygon polygon) {
            Polygons.Add(polygon);
            TotalPolygonCount += 1;
            polygon.Index = TotalPolygonCount;
            ShapeList.Items.Add(polygon);
        }

        public void UpdateCanvas() {
            Plane.Fill(CanvasOptions.BACKGROUND_COLOR);

            foreach (Polygon polygon in Polygons) {
                ScanLineFiller.LightPackage lp = new ScanLineFiller.LightPackage() {
                    IL = new Vec3(LightShape.Color),
                    ks = polygon.SpecularComponent,
                    kd = polygon.DiffuseComponent,
                    lightPos = new Vec3(LightShape.X, LightShape.Y, LightHeight),
                    m = polygon.SpecularExponent
                };
                Plane.Draw(polygon, lp);
            }

            Plane.Draw(LightShape, null);

            ControlState.DrawStateFeatures();

            Canvas.Source = Plane.CreateBitmapSource();
        }
        public void SetControlState(CanvasControlState controlState) {
            ControlState.ExitState();
            ControlState = controlState;
            ControlState.EnterState();
            UpdateCanvas();
        }
    }
}
