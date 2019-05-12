using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursWork_ObjectSearch.MikhailCV.Helper
{
    class MathHelper
    {
        public static double Sqr(double value)
        {
            return value * value;
        }

        public static int FindNearestGeomElement(double a, double q, double value)
        {
            var x = (Math.Log(value) - Math.Log(a)) / Math.Log(q);
            return (int)Math.Round(x);
        }

        public static double SqrtOfSqrSum(double a, double b)
        {
            return Math.Sqrt(Sqr(a) + Sqr(b));
        }

    }
}
