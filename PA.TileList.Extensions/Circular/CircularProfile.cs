using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Circular
{
    public class CircularProfile
    {
        public double Radius { get; private set; }

        public struct ProfileStep
        {
            public double Angle;
            public double Radius;
        }

        private List<ProfileStep> profile;

        public IEnumerable<ProfileStep> Profile { get { return this.profile; } }

        public CircularProfile(Double radius)
        {
            this.Radius = radius;
            this.ResetProfile();
        }

        public double GetMinRadius()
        {
            return this.profile.Min<ProfileStep>(p => p.Radius);
        }

        public double GetMaxRadius()
        {
            return this.profile.Max<ProfileStep>(p => p.Radius);
        }

        public double GetRadius()
        {
            return this.profile.Sum<ProfileStep>(p => p.Radius) / this.profile.Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius">Radius of selection</param>
        public void ResetProfile()
        {
            this.profile = new List<ProfileStep>();
            this.profile.Add(new ProfileStep() { Angle = 0, Radius = this.Radius });
        }

        [Obsolete]
        public void ResetProfile(double radius)
        {
            this.profile = new List<ProfileStep>();
            this.profile.Add(new ProfileStep() { Angle = 0, Radius = radius });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Angle from which radius change (in radian)</param>
        /// <param name="radius">Radius of selection from specified angle to next step (to end of circle if last step)</param>
        public void AddProfileStep(double angle, double radius)
        {
            this.profile.Add(new ProfileStep() { Angle = angle, Radius = radius });
        }

        public void AddProfileStep(ProfileStep step)
        {
            this.profile.Add(step);
        }
    }
}
