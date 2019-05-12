using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Points
{
    public class CornerDetectionHelper
    {
        public static Mat FindPoints(Mat mat, double threshold)
        {
            mat = NormalizationHelper.Normalization(mat);
            var result = new Mat(mat.Width, mat.Height);

            for (var x = 0; x < mat.Width; x++)
            for (var y = 0; y < mat.Height; y++)
            {
                if (mat.GetAt(x, y) <= threshold || HasBetterNeighbour(mat, x, y)) continue;
                result.Set(x, y, mat.GetAt(x, y));
            }

            return result;
        }

        private static bool HasBetterNeighbour(Mat mat, int x, int y)
        {
            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                if (mat.GetPixel(x + dx, y + dy, BorderWrapType.Mirror) - mat.GetAt(x, y) > 1e-6)
                    return true;
            }

            return false;
        }

        public static List<InterestingPoint> MatToPoints(Mat mat)
        {
            var list = new List<InterestingPoint>();
            for (var x = 0; x < mat.Width; x++)
            for (var y = 0; y < mat.Height; y++)
                if (mat.GetAt(x, y) > 1e-6)
                    list.Add(new InterestingPoint(x, y, mat.GetAt(x, y)));

            return list;
        }

    }
}
