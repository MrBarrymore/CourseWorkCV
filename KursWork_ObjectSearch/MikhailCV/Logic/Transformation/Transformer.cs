using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;


namespace KursWork_ObjectSearch.MikhailCV.Logic
{
    public class Transformer
    {
        //преобразование из Mat to Bitmap

        public static Bitmap FromMatToBitmap(Mat data)
        {
            int Width = data.Width;
            int Height = data.Height;

            double min = double.MaxValue, max = double.MinValue;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    min = Math.Min(min, data.GetAt(x, y));
                    max = Math.Max(max, data.GetAt(x, y));
                }

            Bitmap bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    double color = ((data.GetAt(x, y) - min) * 255) / (max - min);
                    bmp.SetPixel(x, y, Color.FromArgb((int)color, (int)color, (int)color));
                }

            return bmp;
        }

        public static Bitmap Transform(Bitmap pictureA, Bitmap pictureB, List<double> matrixA, List<double> matrixB)
        {
            int minX = 0,
                maxX = pictureA.Width,
                minY = 0,
                maxY = pictureA.Height;

            var points = new List<(double, double)>
            {
                Ransac.Transform(matrixB, 0, 0),
                Ransac.Transform(matrixB, pictureB.Width, 0),
                Ransac.Transform(matrixB, 0, pictureB.Height),
                Ransac.Transform(matrixB, pictureB.Width, pictureB.Height)
            };

            minX = (int)Math.Min(minX, points.Min(x => x.Item1));
            minY = (int)Math.Min(minY, points.Min(x => x.Item2));
            maxX = (int)Math.Max(maxX, points.Max(x => x.Item1));
            maxY = (int)Math.Max(maxY, points.Max(x => x.Item2));

            int width = maxX - minX,
                height = maxY - minY;
            int dx = -minX, dy = -minY;

            var bitmap = new Bitmap(width, height);
            var g = Graphics.FromImage(bitmap);

            g.DrawImage(pictureA, dx, dy, pictureA.Width, pictureA.Height);

            for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var (nx, ny) = Ransac.Transform(matrixA, x - dx, y - dy);

                    if (nx < 0 || ny < 0 || nx >= pictureB.Width || ny >= pictureB.Height) continue;
                    bitmap.SetPixel(x, y, pictureB.GetPixel((int)nx, (int)ny));
                }

            return bitmap;
        }

    }
}
