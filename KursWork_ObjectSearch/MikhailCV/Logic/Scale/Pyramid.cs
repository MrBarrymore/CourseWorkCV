using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Convolution;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Scale
{
    public class Pyramid
    {
        private const int Overlap = 3;
        public static int Depth = 5;
        private readonly Dictionary<int, List<Mat>> dog = new Dictionary<int, List<Mat>>();
        private readonly Dictionary<int, List<PyramidMat>> octaves = new Dictionary<int, List<PyramidMat>>();
        private readonly double sigma0;


        private Pyramid(double sigma0, int depth, int octaveSize)
        {
            this.sigma0 = sigma0;
            Depth = depth;
            OctaveSize = octaveSize;
        }

        public int OctaveSize { get; set; }


        public static Pyramid Build(Mat source, int layers, double sigma1, double sigma0)
        {
            if (sigma1 > sigma0)
                throw new ArgumentException("sigma1 must be <= sigma0");

            var image = GaussBlur(source, sigma1, sigma0);
            var pyramid = new Pyramid(sigma0, Depth, layers);
            var k = Math.Pow(2, 1D / layers);

            for (var i = 0; i < Depth; i++)
            {
                BuildOctave(pyramid, i, layers + Overlap, sigma0, k, image);
                var result = pyramid.octaves[i];

                if (i != Depth - 1)
                    image = Downscale(result[layers].GetMat());
            }

            return pyramid;
        }

        public PyramidMat GetLayer(int octave, int layer)
        {
            if (layer < 0 || layer >= OctaveSize + Overlap)
                throw new ArgumentException("Octave layer index out of bounds");

            return octaves[octave][layer];
        }

        private static void BuildOctave(Pyramid pyramid, int index, int layers, double sigma0, double k,
            Mat image)
        {
            var list = new List<PyramidMat>(layers);
            var dogList = new List<Mat>(layers);

            var sigmaPrev = sigma0;
            list.Add(new PyramidMat(image, index, 0, sigmaPrev, sigmaPrev * (1 << index)));

            for (var i = 1; i < layers; i++)
            {
                var sigmaCurrent = sigmaPrev * k;

                var prevImage = list[list.Count - 1].GetMat();
                var currentImage = GaussBlur(prevImage, sigma0, sigmaCurrent);

                list.Add(new PyramidMat(currentImage, index, i + 1, sigmaCurrent, sigmaCurrent * (1 << index)));
                dogList.Add(Diff(currentImage, prevImage));

                sigmaPrev = sigmaCurrent;
            }

            pyramid.octaves.Add(index, list);
            pyramid.dog.Add(index, dogList);
        }

        private static Mat GaussBlur(Mat mat, double current, double newSigma)
        {
            var sigma = Math.Sqrt(MathHelper.Sqr(newSigma) - MathHelper.Sqr(current));

            return
                sigma < 1e-6
                    ? mat.Clone()
                    : ConvolutionHelper.Convolution(mat, Gauss.GetKernel(sigma));
        }

        public double L(int x, int y, double sigma)
        {
            var pos = Math.Max(0, MathHelper.FindNearestGeomElement(sigma0 / 2, Math.Pow(2, 1D / OctaveSize), sigma));

            var targetOctave = pos / OctaveSize - 1;
            var targetLayer = pos % OctaveSize;

            var layer = targetOctave >= Depth
                ? GetLayer(Depth - 1, OctaveSize)
                : GetLayer(targetOctave, targetLayer);

            var targetX = (x / x) << targetOctave;
            var targetY = (y / y) << targetOctave;

            return layer.GetMat().GetPixel(targetX, targetY, BorderWrapType.Mirror);
        }

        public Mat GetDoG(int octave, int index)
        {
            return dog[octave][index];
        }


        private static Mat Diff(Mat a, Mat b)
        {
            var bufA = a.GetData();
            var bufB = b.GetData();
            var buf = new double[a.Width * a.Height];

            for (var i = 0; i < buf.Length; i++) buf[i] = bufA[i] - bufB[i];

            return new Mat(a.Width, a.Height, buf);
        }

        private static Mat Downscale(Mat source)
        {
            var target = new Mat(source.Width / 2, source.Height / 2);

            for (var x = 0; x < target.Width; x++)
                for (var y = 0; y < target.Height; y++)
                    target.Set(x, y, source.GetAt(x * 2, y * 2));

            return target;
        }


    }
}
