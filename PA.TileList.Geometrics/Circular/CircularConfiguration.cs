using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Geometrics.Circular
{
    /// <summary>
    /// Parameters for circular operations
    /// </summary>
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
        /// Nb of steps 
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

        /// <summary>
        /// Define CircularConfiguration with automatic resolution based on tolerance
        /// </summary>
        /// <param name="tolerance">Tolerance percentage for selectionType</param>
        /// <param name="selectionType"></param>
        public CircularConfiguration(float tolerance, SelectionFlag selectionType)
        {
            if (tolerance < 0 || tolerance > 1)
                throw new ArgumentOutOfRangeException("Should be a percentage");

            this.Tolerance = tolerance;
            this.Resolution = 1;
            this.SelectionType = selectionType;

            // Automatic resolution
            double factor = tolerance;
            while (Math.Floor(factor) != factor)
            {
                this.Resolution = this.Resolution / 10f;
                factor = factor / 10f;
            }

            // Members
            this.Steps = (int)Math.Round(1 / this.Resolution + 1, 0);
            this.MaxSurface = this.Steps * this.Steps;
            this.MinSurface = this.Tolerance * this.MaxSurface;
        }

        [Obsolete("Please use CircularConfiguration(float tolerance, SelectionFlag selectionType) as constructor")]
        public CircularConfiguration(float tolerance, float resolution, SelectionFlag selectionType)
        {
            if (tolerance < 0 || tolerance > 1) 
                throw new ArgumentOutOfRangeException("Should be a percentage");

            if (resolution < 0 || resolution > 1) 
                throw new ArgumentOutOfRangeException("Should be a percentage");

            this.Tolerance = tolerance;
            this.Resolution = resolution;
            this.SelectionType = selectionType;

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
