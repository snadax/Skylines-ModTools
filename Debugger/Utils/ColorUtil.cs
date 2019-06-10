using System;
using UnityEngine;

namespace ModTools
{
    public static class ColorUtil
    {
        public struct HSV
        {
            public double h;
            public double s;
            public double v;

            public override string ToString() => $"H: {h.ToString("0.00")}, S: {s.ToString("0.00")}, V:{v.ToString("0.00")}";

            public static HSV RGB2HSV(Color color) => RGB2HSV((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));

            public static HSV RGB2HSV(double r, double b, double g)
            {
                double delta, min;
                double h = 0, s, v;

                min = Math.Min(Math.Min(r, g), b);
                v = Math.Max(Math.Max(r, g), b);
                delta = v - min;

                if (v == 0.0)
                {
                    s = 0;
                }
                else
                {
                    s = delta / v;
                }

                if (s == 0)
                {
                    h = 0.0f;
                }
                else
                {
                    if (r == v)
                    {
                        h = (g - b) / delta;
                    }
                    else if (g == v)
                    {
                        h = 2 + (b - r) / delta;
                    }
                    else if (b == v)
                    {
                        h = 4 + (r - g) / delta;
                    }

                    h *= 60;
                    if (h < 0.0)
                    {
                        h += 360;
                    }
                }

                var hsvColor = new HSV
                {
                    h = h,
                    s = s,
                    v = v / 255
                };

                return hsvColor;
            }

            public static Color HSV2RGB(HSV color) => HSV2RGB(color.h, color.s, color.v);

            public static Color HSV2RGB(double h, double s, double v)
            {
                double r;
                double g;
                double b;

                if (s == 0)
                {
                    r = v;
                    g = v;
                    b = v;
                }
                else
                {
                    int i;
                    double f, p, q, t;

                    if (h == 360)
                    {
                        h = 0;
                    }
                    else
                    {
                        h /= 60;
                    }

                    i = (int)h;
                    f = h - i;

                    p = v * (1.0 - s);
                    q = v * (1.0 - s * f);
                    t = v * (1.0 - s * (1.0f - f));

                    switch (i)
                    {
                        case 0:
                            r = v;
                            g = t;
                            b = p;
                            break;

                        case 1:
                            r = q;
                            g = v;
                            b = p;
                            break;

                        case 2:
                            r = p;
                            g = v;
                            b = t;
                            break;

                        case 3:
                            r = p;
                            g = q;
                            b = v;
                            break;

                        case 4:
                            r = t;
                            g = p;
                            b = v;
                            break;

                        default:
                            r = v;
                            g = p;
                            b = q;
                            break;
                    }
                }

                return new Color((float)r, (float)g, (float)b, 1);
            }
        }
    }
}
