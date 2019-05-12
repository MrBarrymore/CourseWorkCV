using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using KursWork_ObjectSearch.EmguCV;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic;

namespace KursWork_ObjectSearch
{
    public partial class MainForm : Form
    {
        public const string PathToReadImage = "..\\..\\..\\input\\";
        public const string PathToWriteImage = "..\\..\\..\\output\\";

        private Bitmap pictureSample;
        private Bitmap pictureSample2;
        private Bitmap pictureFull;

        private int TestNum = 3;

        public MainForm()
        {
            InitializeComponent();
            InputFull_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            InputSample_pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            OutputPictureBoxMatches.SizeMode = PictureBoxSizeMode.Zoom;
            capturedImageBox.SizeMode = PictureBoxSizeMode.Zoom;

            pictureSample = new Bitmap(PathToReadImage + "hough-"+ TestNum + "-sample.jpg");
            pictureSample2 = new Bitmap(PathToReadImage + "hough-" + TestNum + "-sample.jpg");
            pictureFull = new Bitmap(PathToReadImage + "hough-" + TestNum + "-full.jpg");

            InputSample_pictureBox.Image = pictureSample;
            InputFull_PictureBox.Image = pictureFull;
        }

        private void FindPointButton_Click(object sender, EventArgs e)
        {
            Mat modelImage = CvInvoke.Imread(PathToReadImage + "hough-" + TestNum + "-sample.jpg", ImreadModes.Grayscale);
            Mat modelImage2 = CvInvoke.Imread(PathToReadImage + "hough-" + TestNum + "-sample.jpg", ImreadModes.Grayscale);
            Mat observedImage = CvInvoke.Imread(PathToReadImage + "hough-" + TestNum + "-full.jpg", ImreadModes.Grayscale);

            long matchTime;
            Mat result = DrawMatches.Draw(modelImage, modelImage2, observedImage, out matchTime);        
            capturedImageBox.Image = result;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap result = new Bitmap(PathToReadImage + "hough-" + TestNum + "-full.jpg");
            string imageName = "hough-" + TestNum + "-full";

            var imageSample = IOHelper.ImageToMat(pictureSample);
            var imageFull = IOHelper.ImageToMat(pictureFull);

            Hough hough = new Hough(imageSample, imageFull);
            List<Point[]> objects = hough.Find();

            Bitmap matchImage = DrawHelper.DrawTwoImages(pictureSample, pictureFull, hough.ReverseMatches);

            foreach (var obj in objects)
            {
                DrawHelper.DrawPolygon(matchImage, obj, imageSample.Width, 20);
            }

            matchImage.Save(PathToWriteImage + $"{imageName}_1matches.jpeg", ImageFormat.Jpeg);
            hough.VotesImage.Save(PathToWriteImage + $"{imageName}_2votes.jpeg", ImageFormat.Jpeg);
            result.Save(PathToWriteImage + $"{imageName}_3result.jpeg", ImageFormat.Jpeg);

            OutputPictureBoxMatches.Image = matchImage;
      //      OutputPictureBoxVotes.Image = hough.VotesImage;
         //   OutputPictureBoxResult.Image = result;
        }
    }
}
