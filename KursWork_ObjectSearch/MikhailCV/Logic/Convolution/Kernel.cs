
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Convolution
{
    public class Kernel
    {
        private readonly Mat xVector;
        private readonly Mat yVector;

        private Kernel(Mat xVector, Mat yVector)
        {
            this.xVector = xVector;
            this.yVector = yVector;
        }

        public Mat GetXVector()
        {
            return xVector;
        }

        public Mat GetYVector()
        {
            return yVector;
        }

        public static Kernel For(Mat xVector, Mat yVector)
        {
            return new Kernel(xVector, yVector);
        }
    }
}
