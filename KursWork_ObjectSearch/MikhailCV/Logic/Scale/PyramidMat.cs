using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Scale
{
    public class PyramidMat : Mat
    {
        private readonly double globalSigma;
        private readonly int index;
        public readonly double LocalSigma;
        private readonly int octave;

        public PyramidMat(Mat mat, int octave, int index, double localSigma, double globalSigma)
            : base(mat.Width, mat.Height, mat.GetData())
        {
            this.octave = octave;
            this.index = index;
            LocalSigma = localSigma;
            this.globalSigma = globalSigma;
        }

        public Mat GetMat()
        {
            return new Mat(Width, Height, GetData());
        }

        public override string ToString()
        {
            return $" octave={octave} layer={index} sigma={LocalSigma:F} effSigma={globalSigma:F}";
        }

    }
}
