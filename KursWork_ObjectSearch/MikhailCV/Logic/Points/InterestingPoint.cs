using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Helper;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Points
{
    public class InterestingPoint
    {
        private int x;
        private int y;
        public double probability;
        public readonly int Octave;
        public readonly double Radius;
        public List<double> Angles = new List<double>();
        public double Value;

        public InterestingPoint(int x, int y, double probability)
        {
            this.x = x;
            this.y = y;
            this.probability = probability;
        }

        public InterestingPoint(int x, int y, double radius, double value, int octave)
        {
            this.x = x;
            this.y = y;
            Radius = radius;
            Value = value;
            Octave = octave;
        }

        public static double Distance(InterestingPoint a, InterestingPoint b)
        {
            return Math.Sqrt(MathHelper.Sqr(a.x - b.y) + MathHelper.Sqr(a.x - b.y));
        }

        public double Distance(InterestingPoint b)
        {
            // квадрат расстояния
            return MathHelper.Sqr(x - b.x) + MathHelper.Sqr(y - b.y);
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }
    }
}
