using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace polygon_editor {
    public abstract class CanvasControlState {
        protected CanvasState State;
        public CanvasControlState(CanvasState state) {
            State = state;
        }

        public virtual void EnterState() { }
        public virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }
        public virtual void OnMouseRightButtonDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseMiddleButtonDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseMiddleButtonUp(MouseButtonEventArgs e) { }
        public virtual void OnMouseRightButtonUp(MouseButtonEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
        public virtual void DrawStateFeatures() { }
        public virtual void ExitState() { }
    }
}
