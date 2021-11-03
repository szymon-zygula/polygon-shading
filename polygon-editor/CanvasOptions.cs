using System;
using System.Windows.Input;

namespace polygon_editor {
    public static class CanvasOptions {
        public static readonly UInt32 BACKGROUND_COLOR = 0xFFF8F8F8;
        public static readonly UInt32 ACTIVE_EDGE_COLOR = 0xFF0000FF;
        public static readonly UInt32 ACTIVE_CENTER_COLOR = 0xFFFF00FF;
        public static readonly UInt32 ADDING_CONSTRAINT_MARKER_COLOR = 0xFFFF0000;
        public static readonly UInt32 SET_CONSTRAINT_MARKER_COLOR = 0xFF00FF00;

        public static readonly UInt32 DRAWN_POLYGON_COLOR = 0xFF00B000;
        public static readonly UInt32 DEFAULT_POLYGON_COLOR = 0xFF0000FF;
        public static readonly UInt32 DEFAULT_VERTEX_COLOR = 0xFFFF0000;

        public static readonly UInt32 DEFAULT_LIGHT_COLOR = 0xFFBB2200;
        public static readonly int LIGHT_ICON_RADIUS = 15;
        public static readonly int LIGHT_ICON_RAYS = 15;

        public static readonly int ACTIVE_VERTEX_RADIUS = 5;
        public static readonly int ACTIVE_EDGE_RADIUS = ACTIVE_VERTEX_RADIUS;

        public static readonly Cursor NORMAL_CURSOR = Cursors.Arrow;
        public static readonly Cursor DRAW_CURSOR = Cursors.Cross;
        public static readonly Cursor PLACE_LIGHT_CURSOR = Cursors.Cross;
        public static readonly Cursor REMOVE_CURSOR = Cursors.Hand;
    }
}
