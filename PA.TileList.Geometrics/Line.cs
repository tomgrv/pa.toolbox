using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.TileList.Geometrics;

namespace PA.TileList.Geometrics.Line
{
    public class Line<T>
        where T : ICoordinate
    {
        public T Coordinate1 { get; private set; }
        public ICoordinate Coordinate2 { get; private set; }

        public Line(T c1, ICoordinate c2)
        {
            this.Coordinate1 = c1;
            this.Coordinate2 = c2;
        }

        public bool Intersect(Line<T> line)
        {
            int test1 = (int)this.Coordinate2.GetOrientation(this.Coordinate1, line.Coordinate1) * (int)this.Coordinate2.GetOrientation(this.Coordinate1, line.Coordinate2);
            int test2 = (int)line.Coordinate2.GetOrientation(line.Coordinate1, this.Coordinate1) * (int)line.Coordinate2.GetOrientation(line.Coordinate1, this.Coordinate2);
            return test1 <= 0 && test2 <= 0;
        }
    }
}
