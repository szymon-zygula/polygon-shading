﻿using System;

namespace polygon_editor {
    public struct Vec2 {
        public double X, Y;

        public Vec2(double x, double y) {
            X = x;
            Y = y;
        }
    }

    public struct Vec3 {
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
            UInt32 r = (UInt32)(Math.Round(X * 255.0)) << 16;
            UInt32 g = (UInt32)(Math.Round(Y * 255.0)) << 8;
            UInt32 b = (UInt32)(Math.Round(Z * 255.0));

            return a | r | g | b;
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
