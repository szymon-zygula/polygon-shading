using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace polygon_editor {
    public static class InterfaceUtils {
        public static UInt32? GetColorFromDialog() {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() != DialogResult.OK) {
                return null;
            }

            UInt32 convertedColor;
            unsafe {
                int color = colorDialog.Color.ToArgb();
                convertedColor = *((UInt32*)(void*)&color);
            }

            return convertedColor;
        }
    }
}
