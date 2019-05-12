using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Blobs;
using KursWork_ObjectSearch.MikhailCV.Logic.Descriptors;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;


namespace KursWork_ObjectSearch.MikhailCV.Logic
{
    using Match = ValueTuple<Descriptor, Descriptor>;
    public class Hough
    {
        private int CoordinateStep = 200;
        private const int AngleCells = 12;
        private const double MinScale = 1 / 4D;
        private const double KScale = 2;
        private const int CellsScale = 4;
        private const int VotersThreshold = 4;
        private readonly Mat image;
        private readonly Mat sample;

        private readonly List<Point[]> objects = new List<Point[]>();
        public List<Match> ReverseMatches;
        private List<Match>[,,,] voters;
        private double[,,,] votes;

        public System.Drawing.Image VotesImage;

        public Hough(Mat smpl, Mat img)
        {
            sample = smpl;
            image = img;
        }

        public List<Point[]> Find()
        {
            CoordinateStep = Math.Min(image.Width, image.Height) / 5;
            // CoordinateStep = (int)(1.1 * Math.Min(image.Width, image.Height) / (Math.Max(image.Width, image.Height) / Math.Max(sample.Width, sample.Height)));

            var cellsX = (int)Math.Ceiling(image.Width * 1D / CoordinateStep);
            var cellsY = (int)Math.Ceiling(image.Height * 1D / CoordinateStep);

            VotesImage = IOHelper.MatToImage(image);
            for (var i = 1; i < cellsX; i++)
            {
                var x = CoordinateStep * i;
                DrawHelper.DrawLine(VotesImage, x, 0, x, image.Height - 1);
            }

            for (var i = 1; i < cellsY; i++)
            {
                var y = CoordinateStep * i;
                DrawHelper.DrawLine(VotesImage, 0, y, image.Width - 1, y);
            }

            var descriptorsSample = BlobsFinder.FindBlobs(sample, 500);
            var descriptorsFullImage = BlobsFinder.FindBlobs(image, 500);
            var matches = DescriptorMatcher.Nndr(descriptorsFullImage, descriptorsSample);
            ReverseMatches = matches.Select(x => new Match(x.Item2, x.Item1)).ToList();

            var sampleCenterX = sample.Width / 2.0;
            var sampleCenterY = sample.Height / 2.0;

            votes = new double[cellsX, cellsY, AngleCells, CellsScale];
            voters = new List<Match>[cellsX, cellsY, AngleCells, CellsScale];

            foreach (var match in matches)
            {
                var samplePoint = match.Item2.Point;
                var imagePoint = match.Item1.Point;

                var scale = imagePoint.Radius / samplePoint.Radius;
                var angle = match.Item2.Angle - match.Item1.Angle;

                var vectorX = scale * (sampleCenterX - samplePoint.getX());
                var vectorY = scale * (sampleCenterY - samplePoint.getY());

                var centerX = imagePoint.getX() + vectorX * Math.Cos(angle) - vectorY * Math.Sin(angle);
                var centerY = imagePoint.getY() + vectorX * Math.Sin(angle) + vectorY * Math.Cos(angle);

                Vote(match, centerX, centerY, scale, angle);
                DrawHelper.DrawLine(VotesImage,
                    imagePoint.getX(), imagePoint.getY(),
                    (int)Math.Round(centerX), (int)Math.Round(centerY));
            }

            for (var x = 0; x < cellsX; x++)
                for (var y = 0; y < cellsY; y++)
                    for (var a = 0; a < AngleCells; a++)
                        for (var s = 0; s < CellsScale; s++)
                            if (IsVotesLocalMaximum(x, y, a, s) && voters[x, y, a, s].Count > VotersThreshold)
                            {
                                //  DrawHelper.DrawPolygon(VotesImage, GetPreliminary(x, y, a, s), true);
                                objects.Add(GetLocation(Ransac.CalculateTransform(voters[x, y, a, s])));
                            }

            return objects;
        }

        private void Vote(Match match, double x, double y, double scale, double angle)
        {
            if (x < 0 || x >= image.Width || y < 0 || y >= image.Height) return;

            while (angle < 0) angle += Math.PI * 2;
            while (angle >= Math.PI * 2) angle -= Math.PI * 2;

            // Coordinates
            var xPos = (int)Math.Floor(x / CoordinateStep);
            var yPos = (int)Math.Floor(y / CoordinateStep);

            // Rotation
            var angleStep = Math.PI * 2 / AngleCells;
            var aPos = (int)Math.Floor(angle / angleStep);

            // Scale
            var sPos = MathHelper.FindNearestGeomElement(MinScale, KScale, scale);
            if (scale < MinScale * Math.Pow(KScale, sPos)) sPos--;

            if (sPos < 0) sPos = 0;
            if (sPos >= CellsScale) sPos = CellsScale - 1;

            // Distribution
            var xDelta = x % CoordinateStep / CoordinateStep;
            var yDelta = y % CoordinateStep / CoordinateStep;
            var aDelta = angle % angleStep / angleStep;

            var scaleLeft = MinScale * Math.Pow(KScale, sPos);
            var scaleRight = MinScale * Math.Pow(KScale, sPos + 1);
            var sDelta = (scale - scaleLeft) / (scaleRight - scaleLeft);

            //noinspection SuspiciousNameCombination
            var d = new List<(int, double)>
            {
                new ValueTuple<int, double>(xPos, xDelta),
                new ValueTuple<int, double>(yPos, yDelta),
                new ValueTuple<int, double>(aPos, aDelta),
                new ValueTuple<int, double>(sPos, sDelta)
            };

            Vote(d, 0, new int[d.Count], new double[d.Count], match);
        }


        private void Vote(List<ValueTuple<int, double>> list, int step, int[] indices, double[] result, Match match)
        {
            if (step == list.Count)
            {
                var value = result.Aggregate<double, double>(1, (current, d) => current * d);

                Vote(indices[0], indices[1], indices[2], indices[3], value, match);
            }
            else
            {
                var current = list[step];
                var value = 1 - Math.Abs(current.Item2 - 0.5);

                indices[step] = current.Item1;
                result[step] = value;
                Vote(list, step + 1, indices, result, match);

                indices[step] = current.Item1 + (current.Item2 < 0.5 ? -1 : 1);
                result[step] = 1 - value;
                Vote(list, step + 1, indices, result, match);
            }
        }

        private void Vote(int xPos, int yPos, int aPos, int sPos, double value, Match match)
        {
            if (xPos < 0 || xPos >= votes.GetLength(0)) return;
            if (yPos < 0 || yPos >= votes.GetLength(1)) return;
            if (sPos < 0 || sPos >= CellsScale) return;

            if (aPos < 0) aPos += AngleCells;
            if (aPos >= AngleCells) aPos -= AngleCells;

            votes[xPos, yPos, aPos, sPos] += value;

            if (voters[xPos, yPos, aPos, sPos] == null)
                voters[xPos, yPos, aPos, sPos] = new List<Match>();
            voters[xPos, yPos, aPos, sPos].Add(match);
        }


        private bool IsVotesLocalMaximum(int x, int y, int a, int s)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                var nX = x + dx;
                if (nX < 0 || nX >= votes.GetLength(0)) continue;

                for (var dy = -1; dy <= 1; dy++)
                {
                    var nY = y + dy;
                    if (nY < 0 || nY >= votes.GetLength(1)) continue;

                    for (var da = -1; da <= 1; da++)
                    {
                        var nA = a + da;

                        if (nA < 0) nA += AngleCells;
                        if (nA >= AngleCells) nA -= AngleCells;

                        for (var ds = -1; ds <= 1; ds++)
                        {
                            var nS = s + ds;
                            if (nS < 0 || nS >= CellsScale) continue;

                            if (dx == 0 && dy == 0 && da == 0 && ds == 0) continue;

                            if (votes[nX, nY, nA, nS] >= votes[x, y, a, s]) return false;
                        }
                    }
                }
            }

            return true;
        }

        private Point[] GetPreliminary(int pX, int pY, int pA, int pS)
        {
            var x = (int)Math.Round((pX + 0.5) * CoordinateStep);
            var y = (int)Math.Round((pY + 0.5) * CoordinateStep);

            var angle = (pA + 0.5) * (Math.PI * 2 / AngleCells);
            var scale = MinScale * Math.Pow(KScale, pS + 0.5);

            var sampleCenterX = sample.Width / 2D;
            var sampleCenterY = sample.Height / 2D;

            var points = new List<Point>();
            foreach (var (cornerX, cornerY) in GetSampleCorners())
            {
                var vX = (cornerX - sampleCenterX) * scale;
                var vY = (cornerY - sampleCenterY) * scale;

                var centerX = x + vX * Math.Cos(angle) - vY * Math.Sin(angle);
                var centerY = y + vX * Math.Sin(angle) + vY * Math.Cos(angle);

                points.Add(new Point((int)Math.Round(centerX), (int)Math.Round(centerY)));
            }

            return points.ToArray();
        }

        private Point[] GetLocation(ValueTuple<List<double>, List<double>> mx)
        {
            var points = new List<Point>();
            var matrix = mx.Item2;
            foreach (var (cornerX, cornerY) in GetSampleCorners())
            {
                var (pointX, pointY) = Ransac.Transform(matrix, cornerX, cornerY);
                points.Add(new Point((int)Math.Round(pointX), (int)Math.Round(pointY)));
            }

            return points.ToArray();
        }

        private List<ValueTuple<int, int>> GetSampleCorners()
        {
            return new List<ValueTuple<int, int>>
            {
                new ValueTuple<int, int>(0, 0),
                new ValueTuple<int, int>(sample.Width, 0),
                new ValueTuple<int, int>(sample.Width, sample.Height),
                new ValueTuple<int, int>(0, sample.Height),
            };
        }
    }
}
