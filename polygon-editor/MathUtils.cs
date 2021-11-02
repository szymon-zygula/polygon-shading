using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon_editor {
    public static class MathUtils {
        public static double VectorLength((double, double) vec) {
            return Math.Sqrt(vec.Item1 * vec.Item1 + vec.Item2 * vec.Item2);
        }

        public static (double, double) MulVector((double, double) vec, double scal) {
            return (
                vec.Item1 * scal, vec.Item2 * scal
            );
        }

        public static void MovePoint(ref (int, int) point, (int, int) newRoot, (double, double) vec) {
            point.Item1 = newRoot.Item1 + (int)Math.Round(vec.Item1);
            point.Item2 = newRoot.Item2 + (int)Math.Round(vec.Item2);
        }

        public static double DotProduct((double, double) a, (double, double) b) {
            return a.Item1 * b.Item1 + a.Item2 * b.Item2;
        }

        public static double Determinant((double, double) a, (double, double) b) {
            return a.Item1 * b.Item2 - a.Item2 * b.Item1;
        }

        public static (double, double) Rotate((double, double) p, double phi) {
            return (
                p.Item1 * Math.Cos(phi) - p.Item2 * Math.Sin(phi),
                p.Item1 * Math.Sin(phi) + p.Item2 * Math.Cos(phi)
            );
        }

        public static (double, double) RotateByCenter((double, double) center, (double, double) p, double phi) {
            var rotated = Rotate((p.Item1 - center.Item1, p.Item2 - center.Item2), phi);
            return (rotated.Item1 + center.Item1, rotated.Item2 + center.Item2);
        }

        public static (int, int) RoundVector((double, double) vec) {
            return ((int)Math.Round(vec.Item1), (int)Math.Round(vec.Item2));
        }
    }
}
