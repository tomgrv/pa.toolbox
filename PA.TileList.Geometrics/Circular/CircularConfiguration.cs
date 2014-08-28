using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Geometrics.Circular
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

        /// <summary>
        /// Calc resolution
        /// </summary>
        public float Resolution { get; private set; }
        
        /// <summary>
        /// Percentage of surface considered (1f = 100% = all surface)
        /// </summary>
        public float Tolerance { get; private set; }

        /// <summary>
        /// Calc stepping (based on resolution)
        /// </summary>
        public int Steps { get; private set; }

        /// <summary>
        /// Number of Point required for under
        /// </summary>
        public float MinSurface { get; private set; }

        /// <summary>
        /// Number Of Points required for inside
        /// </summary>
        public float MaxSurface { get; private set; }

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

            this.Steps = (int)Math.Round(1 / this.Resolution + 1, 0);
            this.MaxSurface = this.Steps * this.Steps;
            this.MinSurface = this.Tolerance * this.MaxSurface;
        }

        public float GetSurfacePercent(int points)
        {
            return  points / this.MaxSurface ;
        }
    
    }
}
