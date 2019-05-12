using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Descriptors
{
    public class Vector : Mat
    {
        public Vector(int width, double[] data) : base(width, 1, data)
        {
        }

        public Vector(int width) : base(width, 1)
        {
        }

        public double Get(int x)
        {
            return Data[GetIndex(x, 0)];
        }

        public void Set(int index, double value)
        {
            Data[index] = value;
        }

        public double DistanceTo(Vector b)
        {
            if (b.GetData().Length != Data.Length)
                throw new ArgumentException("Vectors are not a same size");

            var sum = Data.Select((t, i) => MathHelper.Sqr(t - b.GetData()[i])).Sum();

            return Math.Sqrt(sum);
        }

        private double GetLength()
        {
            var sum = Data.Sum(item => item * item);
            return Math.Sqrt(sum);
        }

        public Vector Normalize()
        {
            var length = GetLength();
            for (var i = 0; i < Data.Length; i++) Data[i] /= length;

            return this;
        }
    }
}
