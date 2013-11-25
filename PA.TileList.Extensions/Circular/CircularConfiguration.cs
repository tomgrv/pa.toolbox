using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Circular
{
    public class CircularConfiguration
    {
        [Flags]
        public enum SelectionFlag
        {
            Inside = 0x04,
            Outside = 0x02,
            Under = 0x01
        }

        public float Resolution { get; private set; }
        public float Tolerance { get; private set; }
        public SelectionFlag SelectionType { get; private set; }
       
        public CircularConfiguration(float tolerance, float resolution, SelectionFlag type)
        {
            if (tolerance < 0 || tolerance > 1) 
                throw new ArgumentOutOfRangeException("Should be a percentage");

            if (resolution < 0 || resolution > 1) 
                throw new ArgumentOutOfRangeException("Should be a percentage");

            this.Tolerance = tolerance;
            this.Resolution = resolution;
            this.SelectionType = type;
        }

    }
}
