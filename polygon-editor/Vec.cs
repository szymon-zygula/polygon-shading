using System;

namespace polygon_editor {
    public struct Vec2 {
        static Random Rnd = new Random();
        public double X, Y;

        public Vec2(double x, double y) {
            X = x;
            Y = y;
        }

        public double Length() {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vec2 Normalize() {
            return this / Length();
        }

        public static Vec2 RandomNormal() {
            Vec2 u = new Vec2(Rnd.NextDouble() * 2.0 - 1.0, Rnd.NextDouble() * 2.0 - 1.0);
            return u.Normalize();
        }

        public static Vec2 operator +(Vec2 u) => u;
        public static Vec2 operator -(Vec2 u) => new Vec2(-u.X, -u.Y);
        public static Vec2 operator +(Vec2 u, Vec2 v) => new Vec2(u.X + v.X, u.Y + v.Y);
        public static Vec2 operator -(Vec2 u, Vec2 v) => new Vec2(u.X - v.X, u.Y - v.Y);
        public static Vec2 operator *(Vec2 u, double a) => new Vec2(a * u.X, a * u.Y);
        public static Vec2 operator *(double a, Vec2 u) => u * a;
        public static Vec2 operator /(Vec2 u, double a) => new Vec2(u.X / a, u.Y / a);
    }

    public struct Vec3 {
        static Random Rnd = new Random();
        public double X, Y, Z;

        public Vec3(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(UInt32 color) {
            X = (double)((color & 0x00FF0000) >> 16) / 255.0;
            Y = (double)((color & 0x0000FF00) >> 8) / 255.0;
            Z = (double)(color & 0x000000FF) / 255.0;
        }

        public UInt32 ToColor() {
            UInt32 a = 0xFF000000;
            UInt32 r = (UInt32)(Math.Min(Math.Round(X * 255.0), 255.0)) << 16;
            UInt32 g = (UInt32)(Math.Min(Math.Round(Y * 255.0), 255.0)) << 8;
            UInt32 b = (UInt32)(Math.Min(Math.Round(Z * 255.0), 255.0));

            return a | r | g | b;
        }

        public double Length() {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vec3 Normalize() {
            return this / Length();
        }

        public static Vec3 RandomNormal() {
            Vec3 u = new Vec3(
                Rnd.NextDouble() * 2.0 - 1.0,
                Rnd.NextDouble() * 2.0 - 1.0,
                Rnd.NextDouble() * 2.0 - 1.0
            );
            return u.Normalize();
        }

        public static double AngleCosine(Vec3 u, Vec3 v) {
            return DotProduct(u, v) / (u.Length() * v.Length());
        }

        public static double DotProduct(Vec3 u, Vec3 v) {
            return u.X * v.X + u.Y * v.Y + u.Z * v.Z;
        }

        public static Vec3 UnitDirection(Vec3 from, Vec3 to) {
            return (to - from).Normalize();
        }

        public static Vec3 operator +(Vec3 u) => u;
        public static Vec3 operator -(Vec3 u) => new Vec3(-u.X, -u.Y, -u.Z);
        public static Vec3 operator +(Vec3 u, Vec3 v) => new Vec3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        public static Vec3 operator -(Vec3 u, Vec3 v) => new Vec3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        public static Vec3 operator *(Vec3 u, double a) => new Vec3(a * u.X, a * u.Y, a * u.Z);
        public static Vec3 operator *(double a, Vec3 u) => u * a;
        public static Vec3 operator /(Vec3 u, double a) => new Vec3(u.X / a, u.Y / a, u.Z / a);
    }
}
