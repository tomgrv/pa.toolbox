using PA.TileList.Quantified;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PA.TileList.Circular
{
    public static class CircularExtensions
    {
        public static IEnumerable<KeyValuePair<T, double>> Distance<T>(this IQuantifiedTile<T> list)
            where T : ICoordinate
        {
            return list.Select(c => new KeyValuePair<T, double>(c, Math.Sqrt(CircularExtensions.Distance2(list, c, 0, 0))));
        }

        internal static double Distance2<T>(this IQuantifiedTile<T> list, T c, float offsetX, float offsetY)
          where T : ICoordinate
        {
            double testX = (c.X - list.Reference.X + offsetX) * list.ElementStepX + list.RefOffsetX;
            double testY = (c.Y - list.Reference.Y + offsetY) * list.ElementStepY + list.RefOffsetY;
            return Math.Pow(testX, 2) + Math.Pow(testY, 2);
        }


        public static IEnumerable<KeyValuePair<T, int>> Points<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config, bool optimize = false)
           where T : ICoordinate
        {
            double minRadius2 = Math.Pow(p.GetMinRadius(), 2);
            double maxRadius2 = Math.Pow(p.GetMaxRadius(), 2);
            CircularProfile.ProfileStep first = p.GetFirst();
            CircularProfile.ProfileStep[] profile = p.Profile.ToArray();

            foreach (T c in list)
            {

                List<double> dist2 = new List<double>() {
                    CircularExtensions.Distance2(list, c,0.5f,0.5f),
                    CircularExtensions.Distance2(list, c,-0.5f,0.5f),
                    CircularExtensions.Distance2(list, c,-0.5f,-0.5f),
                    CircularExtensions.Distance2(list, c,0.5f,-0.5f)
                };

                // Certainly All Inside
                if (dist2.Max() < minRadius2)
                {
                    yield return new KeyValuePair<T, int>(c, (int)config.MaxSurface);
                    continue;
                }

                // Certainly All Outside
                if (dist2.Min() > maxRadius2)
                {
                    yield return new KeyValuePair<T, int>(c, 0);
                    continue;
                }

                int points = list.Points(c,config.Steps, config.Resolution, (testX, testY, r2) =>
                    {
                        if (r2 > maxRadius2)
                        {
                            return false;
                        }
                        else if (r2 < minRadius2)
                        {
                            return true;
                        }
                        else
                        {
                            double angle = Math.Atan2(-testY, testX);

                            CircularProfile.ProfileStep last = p.GetFirst();

                            foreach (CircularProfile.ProfileStep current in p.Profile)
                            {
                                if (current.Angle >= angle)
                                {
                                    break;
                                }
                                last = current;
                            }

                            return r2 < Math.Pow(last.Radius, 2);
                        }

                    });


                yield return new KeyValuePair<T, int>(c, points);
            }
        }

        internal static int Points<T>(this IQuantifiedTile<T> list, T c, float steps, float resolution, Func<double, double, double, bool> predicate)
         where T : ICoordinate
        {
            int points = 0;

            for (int i = 0; i < steps; i++)
            {
                double testX = ((c.X - list.Reference.X) - 0.5f + i * resolution) * list.ElementStepX + list.RefOffsetX;
                double temp = Math.Pow(testX, 2);

                for (int j = 0; j < steps; j++)
                {
                    double testY = ((c.Y - list.Reference.Y) - 0.5f + j * resolution) * list.ElementStepY + list.RefOffsetY;
                    double radius2 = temp + Math.Pow(testY, 2);

                    if (predicate(testX, testY, radius2))
                    {
                        points++;
                    }
                }
            }

            return points;
        }

        public static IEnumerable<KeyValuePair<T, float>> Percent<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config)
           where T : ICoordinate
        {
            foreach (KeyValuePair<T, int> c in list.Points(p, config, false))
            {
                yield return new KeyValuePair<T, float>(c.Key, (float)c.Value / config.MaxSurface);
            }
        }

        public static IEnumerable<T> Take<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config)
           where T : ICoordinate
        {
            foreach (KeyValuePair<T, int> c in list.Points(p, config, true))
            {
                if (config.MinSurface <= c.Value && (config.SelectionType & CircularConfiguration.SelectionFlag.Inside) > 0)
                {
                    yield return c.Key;
                }

                if (0 < c.Value && c.Value < config.MinSurface && (config.SelectionType & CircularConfiguration.SelectionFlag.Under) > 0)
                {
                    yield return c.Key;
                }

                if (c.Value == 0 && (config.SelectionType & CircularConfiguration.SelectionFlag.Outside) > 0)
                {
                    yield return c.Key;
                }
            }
        }
    }
}
