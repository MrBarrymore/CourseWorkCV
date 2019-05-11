using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using KursWork_ObjectSearch.EmguCV;

namespace KursWork_ObjectSearch
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InputFull_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            InputSample_pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            OutputPictureBoxMatches.SizeMode = PictureBoxSizeMode.Zoom;
            capturedImageBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public const string PathToReadImage = "..\\..\\..\\input\\";
        public const string PathToWriteImage = "..\\..\\..\\output\\";


        private void FindPointButton_Click(object sender, EventArgs e)
        {
            Bitmap pictureSample = new Bitmap(PathToReadImage + "hough-3-sample.jpg");
            Bitmap pictureFull = new Bitmap(PathToReadImage + "hough-3-full.jpg");

            InputSample_pictureBox.Image = pictureSample;
            InputFull_PictureBox.Image = pictureFull;

            Mat modelImage = CvInvoke.Imread(PathToReadImage + "hough-3-sample.jpg", ImreadModes.Grayscale);
            Mat observedImage = CvInvoke.Imread(PathToReadImage + "hough-3-full.jpg", ImreadModes.Grayscale);


            long matchTime;

            Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);

             

             //ImageViewer.Show(result, String.Format("Matched in {0} milliseconds", matchTime));

             capturedImageBox.Image = result;
        }
    }
}
