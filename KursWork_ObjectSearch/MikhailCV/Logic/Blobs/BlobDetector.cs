using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Logic.Descriptors;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;
using KursWork_ObjectSearch.MikhailCV.Logic.Points;
using KursWork_ObjectSearch.MikhailCV.Logic.Scale;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Blobs
{
    public class BlobsFinder
    {
        //   private const int PointCount = 300;
        private const double InitSigma = 0.5;
        private const double StartSigma = 1.6;
        private const double MinHarris = 0.005;
        private const int ImageBorder = 3;
        private const int OctaveSize = 4;

        public static List<Descriptor> FindBlobs(Mat image, int pointCount)
        {
            var pyramid = Pyramid.Build(image, OctaveSize, InitSigma, StartSigma);
            var points = NonMaximumSuppression.FilterA(image, FindPoints(pyramid), pointCount);

            var descriptors = new List<Descriptor>();

            for (var octave = 0; octave < Pyramid.Depth; octave++)
            {
                var firstImage = pyramid.GetLayer(octave, 0);

                var curPoints = points.Where(x => x.Octave == octave).ToList();

                // Сделать вывод точек на картинке
                //   IOHelper.WriteImageToFile(DrawHelper.DrawPoints(pyramid.GetDoG(octave, 2), curPoints), "..\\..\\..\\..\\Output\\" + octave + ".png");

                descriptors.AddRange(RotationInvariant.Calculate(firstImage, curPoints));
            }

            return descriptors;
        }

        private static List<InterestingPoint> FindPoints(Pyramid pyramid)
        {
            var blobs = new List<InterestingPoint>();

            for (var octave = 0; octave < Pyramid.Depth; octave++)
            {
                var firstImage = pyramid.GetLayer(octave, 0); // Берем первую картинку в октаве 

                for (var layer = 1; layer <= pyramid.OctaveSize; layer++) // 
                {
                    var prev = pyramid.GetDoG(octave, layer - 1);
                    var cur = pyramid.GetDoG(octave, layer); // Берем Dog на двух соседних урвнях одной октавы 
                    var next = pyramid.GetDoG(octave, layer + 1);


                    //   IOHelper.WriteImageToFile(DrawHelper.DrawPoints(pyramid.GetDoG(octave, layer), new List<InterestingPoint>()), 
                    //         "..\\..\\..\\..\\Output\\Октава " + octave + " Уровень " + layer);

                    var radius = Math.Ceiling(pyramid.GetLayer(octave, layer).LocalSigma * Math.Sqrt(2)); // Считаем радиус блобов на каждой октаве

                    for (var x = ImageBorder; x < cur.Width - ImageBorder; x++)
                        for (var y = ImageBorder; y < cur.Height - ImageBorder; y++)
                            if (IsLocalMaxOrMin(x, y, prev, cur, next)) // Отбрасываем не нужные блобы 
                            {
                                var harrisValue = Harris.FindAt(firstImage, (int)radius, x, y); // Считаем значение Харриса для данной точки

                                if (harrisValue > MinHarris)
                                    blobs.Add(new InterestingPoint(
                                        GetCoordinate(x, octave),
                                        GetCoordinate(y, octave),
                                        StartSigma * Math.Pow(2, 1 + octave + (layer - 1.0) / OctaveSize), // Радиус, который в итоге рисуем
                                        harrisValue,
                                        octave
                                    ));
                            }
                }
            }
            return blobs;
        }


        private static bool IsLocalMaxOrMin(int x, int y, Mat prev, Mat cur, Mat next)
        {
            var max = true;
            var min = true;
            var pixel = cur.GetAt(x, y);

            for (var dx = -1; dx <= 1; dx++)
                for (var dy = -1; dy <= 1; dy++)
                {
                    max &= pixel > prev.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);
                    max &= pixel > next.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);
                    min &= pixel < prev.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);
                    min &= pixel < next.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);

                    if (dx != 0 || dy != 0)
                    {
                        max &= pixel > cur.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);
                        min &= pixel < cur.GetPixel(x + dx, y + dy, BorderWrapType.Mirror);
                    }
                }

            return (max || min) && Math.Abs(pixel) > 0.03;
        }

        private static int GetCoordinate(int value, int octave)
        {
            return (int)((value + 0.5) * (1 << octave));
        }

    }
}
