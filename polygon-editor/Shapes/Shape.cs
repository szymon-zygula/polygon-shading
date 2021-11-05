using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon_editor {
    public abstract class Shape {
        public int Index { get; set; }

        public abstract void DrawOn(DrawingPlane plane, ScanLineFiller.LightPackage lp);
    }
}
