using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace polygon_editor {
    class PlacingLightControlState : CanvasControlState {
        public PlacingLightControlState(CanvasState state) : base(state) {

        }

        public override void EnterState() {
            State.Canvas.Cursor = CanvasOptions.PLACE_LIGHT_CURSOR;
        }

        public override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            State.SetControlState(new DoingNothingControlState(State));
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            State.LightShape.X = (int)e.GetPosition(State.Canvas).X;
            State.LightShape.Y = (int)e.GetPosition(State.Canvas).Y;

            State.SetControlState(new DoingNothingControlState(State));
        }

        public override void ExitState() {
            State.Canvas.Cursor = CanvasOptions.NORMAL_CURSOR;
        }
    }
}
