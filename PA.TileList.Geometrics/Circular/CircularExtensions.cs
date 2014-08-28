using PA.TileList.Quantified;
using PA.TileList.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PA.TileList.Geometrics.Circular
{
    public static class CircularExtensions
    {
        public static IEnumerable<KeyValuePair<T, double>> Distance<T>(this IQuantifiedTile<T> tile, Func<T, bool> predicate = null)
            where T : ICoordinate
        {
            return tile.WhereOrDefault(predicate).Select(c => new KeyValuePair<T, double>(c, Math.Sqrt(tile.Distance2(c))));
        }

        internal static double Distance2<T>(this IQuantifiedTile<T> tile, T c)
          where T : ICoordinate
        {
            double testX = (c.X - tile.Reference.X) * tile.ElementStepX + tile.RefOffsetX;
            double testY = (c.Y - tile.Reference.Y) * tile.ElementStepY + tile.RefOffsetY;
            return Math.Pow(testX, 2) + Math.Pow(testY, 2);
        }


        public static IEnumerable<KeyValuePair<T, int>> Points<T>(this IQuantifiedTile<T> tile, CircularProfile p, CircularConfiguration config, Func<T, bool> predicate = null)
           where T : ICoordinate
        {
            double minRadius2 = Math.Pow(p.GetMinRadius(), 2);
            double maxRadius2 = Math.Pow(p.GetMaxRadius(), 2);
            CircularProfile.ProfileStep first = p.GetFirst();
            CircularProfile.ProfileStep[] profile = p.Profile.ToArray();
            bool mix = true;

            foreach (T c in tile.WhereOrDefault(predicate))
            {
                // Quick check with 4 corners
                int quick = tile.Points(c, 2, 1, (testX, testY, r2) =>
                {
                    if (r2 > maxRadius2)
                    {
                        return mix = false;
                    }
                    else if (r2 < minRadius2 * 0.8f)
                    {
                        return mix = true;
                    }
                    else
                    {
                        return mix ^= true;
                    }
                });

                // Certainly All Outside
                if (quick == 0)
                {
                    yield return new KeyValuePair<T, int>(c, 0);
                    continue;
                }

                // Certainly All Inside
                if (quick == 4)
                {
                    yield return new KeyValuePair<T, int>(c, (int)config.MaxSurface);
                    continue;
                }

                // Full check on all surface
                int full = tile.Points(c, config.Steps, config.Resolution, (testX, testY, r2) =>
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
                            double angle = Math.Atan2(testY, testX);
                            CircularProfile.ProfileStep last = profile.Where(ps => ps.Angle < angle).LastOrDefault() ?? first;
                            return r2 < Math.Pow(last.Radius, 2);
                        }

                    });

                yield return new KeyValuePair<T, int>(c, full);
            }
        }


        internal static int Points<T>(this IQuantifiedTile<T> tile, T c, int steps, float resolution, Func<double, double, double, bool> predicate)
         where T : ICoordinate
        {
            int points = 0;

            double[] testY = new double[steps];
            double[] testY2 = new double[steps];

            for (int i = 0; i < steps; i++)
            {
                double testX = ((c.X - tile.Reference.X) - 0.5f + i * resolution) * tile.ElementStepX + tile.RefOffsetX;
                double temp = Math.Pow(testX, 2);

                if (i == 0)
                {
                    for (int j = 0; j < steps; j++)
                    {
                        testY[j] = ((c.Y - tile.Reference.Y) - 0.5f + j * resolution) * tile.ElementStepY + tile.RefOffsetY;
                        testY2[j] = Math.Pow(testY[j], 2);
                        double radius2 = temp + testY2[j];
                        points += predicate(testX, testY[j], radius2) ? 1 : 0;
                    }
                }
                else
                {
                    for (int j = 0; j < steps; j++)
                    {
                        double radius2 = temp + testY2[j];
                        points += predicate(testX, testY[j], radius2) ? 1 : 0;
                    }
                }
            }

            return points;
        }

        public static IEnumerable<KeyValuePair<T, float>> Percent<T>(this IQuantifiedTile<T> tile, CircularProfile p, CircularConfiguration config, Func<T, bool> predicate = null)
           where T : ICoordinate
        {
            foreach (KeyValuePair<T, int> c in tile.Points(p, config, predicate))
            {
                yield return new KeyValuePair<T, float>(c.Key, (float)c.Value / config.MaxSurface);
            }
        }

        public static IEnumerable<T> Take<T>(this IQuantifiedTile<T> tile, CircularProfile p, CircularConfiguration config, Func<T, bool> predicate = null)
           where T : ICoordinate
        {
            bool inside = (config.SelectionType & CircularConfiguration.SelectionFlag.Inside) > 0;
            bool under = (config.SelectionType & CircularConfiguration.SelectionFlag.Under) > 0;
            bool outside = (config.SelectionType & CircularConfiguration.SelectionFlag.Outside) > 0;

            foreach (KeyValuePair<T, int> c in tile.Points(p, config, predicate))
            {
                if (inside && config.MinSurface <= c.Value)
                {
                    yield return c.Key;
                }

                if (under && 0 < c.Value && c.Value < config.MinSurface)
                {
                    yield return c.Key;
                }

                if (outside && c.Value == 0)
                {
                    yield return c.Key;
                }
            }
        }

        public static IQuantifiedTile<T> Take<T>(this IQuantifiedTile<T> tile, CircularProfile p, CircularConfiguration config, ref bool referenceChange, Func<T, bool> predicate = null)
          where T : ICoordinate
        {
            IQuantifiedTile<T> qtile = new QuantifiedTile<T>(tile);

            IEnumerable<T> list = tile.Take(p, config, predicate);

            if (list.Count() == 0)
            {
                referenceChange = false;
                (qtile as IList<T>).Clear();
            }
            else
            {
                referenceChange = referenceChange && !list.Contains(tile.Reference);

                if (referenceChange)
                {
                    qtile.SetReference(list.First());
                }

                foreach (T e in qtile.Except(list).ToArray())
                {
                    qtile.Remove(e);
                }
            }

            return qtile;
        }
    }
}
