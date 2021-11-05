using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static Bitmap GetBitmapFromDialog() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open image";
            dlg.Filter = "Image files (*.bmp, *.png, *.tga, *.jpg)|*.bmp;*.png;*.tga;*.jpg";
            if(dlg.ShowDialog() != DialogResult.OK) {
                return null;
            }

            return new Bitmap(dlg.FileName);
        }
    }
}
