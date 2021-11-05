using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace polygon_editor {

    public partial class MainWindow : Window {
        private readonly CanvasState State;

        public MainWindow() {
            InitializeComponent();
            State = new CanvasState(Canvas, ShapeList) {
                LightHeight = SliderLightHeight.Value
            };

            State.UpdateCanvas();
        }

        private void ButtonDrawPolygon_Click(object sender, RoutedEventArgs e) {
            State.SetControlState(new DrawingPolygonControlState(State));
        }

        private void CanvasImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            State.ControlState.OnMouseLeftButtonUp(e);
        }

        private void CanvasImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            State.ControlState.OnMouseRightButtonUp(e);
        }

        private void CanvasImage_MouseMove(object sender, MouseEventArgs e) {
            State.ControlState.OnMouseMove(e);
        }

        private void CanvasImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            State.ControlState.OnMouseLeftButtonDown(e);
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Middle) {
                State.ControlState.OnMouseMiddleButtonDown(e);
            }
        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ShapeList.SelectedItem is Polygon) {
                Polygon polygon = ShapeList.SelectedItem as Polygon;
                State.SetControlState(new ActivePolygonControlState(State, polygon, this));
                State.UpdateCanvas();
                ShapeList.SelectedItem = null;
            }
            else {
                ShapeList.SelectedItem = null;
            }
        }

        private void ButtonLightColor_Click(object sender, RoutedEventArgs e) {
            UInt32? color = InterfaceUtils.GetColorFromDialog();
            if(color.HasValue) {
                State.LightShape.Color = color.Value;
                State.UpdateCanvas();
            }
        }

        private void SliderLightHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(State != null) {
                State.LightHeight = SliderLightHeight.Value;
                State.UpdateCanvas();
            }
        }

        private void ButtonLightPosition_Click(object sender, RoutedEventArgs e) {
            State.SetControlState(new PlacingLightControlState(State));
        }

        private void SliderMaxAnimationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (State == null) return;
            if(SliderMaxAnimationSpeed.Value < SliderMinAnimationSpeed.Value) {
                SliderMinAnimationSpeed.Value = SliderMaxAnimationSpeed.Value;
            }
            State.MaxAnimationSpeed = SliderMaxAnimationSpeed.Value;
        }

        private void SliderMinAnimationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (State == null) return;
            if(SliderMaxAnimationSpeed.Value < SliderMinAnimationSpeed.Value) {
                SliderMaxAnimationSpeed.Value = SliderMinAnimationSpeed.Value;
            }
            State.MinAnimationSpeed = SliderMinAnimationSpeed.Value;
        }

        private void ButtonAnimationToggle_Click(object sender, RoutedEventArgs e) {
            if(State.ControlState is AnimationControlState) {
                State.SetControlState(new DoingNothingControlState(State));
            }
            else {
                State.SetControlState(new AnimationControlState(State));
            }
        }
    }
}
