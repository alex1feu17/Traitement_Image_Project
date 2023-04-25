using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace Traitementdimage
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
        public partial class Form1 : Form
        {
            private NumericUpDown minCanny, maxCanny, minThresh, maxThresh;
            private PictureBox pictureBox;
            private Button buttonLoadImage;
            private Button buttonGetImage;
            private Button buttonProcessImage;
            private OpenFileDialog openFileDialog1;
            private TextBox textBox1;
            private Label labelmincan, labelmaxcan, labelminthresh, labelmaxthresh;
            List<Rectangle> templateBoundingBoxes = new List<Rectangle>();
            List<Tools> Templatetools = new List<Tools>();
            List<Tools> tools = new List<Tools>();
            private string filePath = string.Empty;
            private Dictionary<string, Image<Bgr, byte>> images;
            Emgu.CV.Util.VectorOfVectorOfPoint Templatecontours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            public Form1()
            {


                images = new Dictionary<string, Image<Bgr, byte>>();
                labelmincan = new Label
                {
                    Size = new Size(130, 30),
                    Location = new Point(400, 20),
                    Text = "minimum Canny :"
                };
                labelmaxcan = new Label
                {
                    Size = new Size(130, 30),
                    Location = new Point(400, 55),
                    Text = "maximum Canny :"
                };
                labelminthresh = new Label
                {
                    Size = new Size(130, 30),
                    Location = new Point(400, 90),
                    Text = "minimum threshold :"
                };
                labelmaxthresh = new Label
                {
                    Size = new Size(130, 30),
                    Location = new Point(400, 120),
                    Text = "maximum threshold :"
                };
                minCanny = new NumericUpDown
                {
                    Size = new Size(100, 30),
                    Location = new Point(560, 20),
                    Maximum = 255,
                    Minimum = 0,
                    Value = 100,

                };
                maxCanny = new NumericUpDown
                {
                    Size = new Size(100, 30),
                    Location = new Point(560, 55),
                    Maximum = 255,
                    Minimum = 0,
                    Value = 200,



                };
                minThresh = new NumericUpDown
                {
                    Size = new Size(100, 30),
                    Location = new Point(560, 90),
                    Maximum = 255,
                    Minimum = 0,
                    Value = 19,

                };
                maxThresh = new NumericUpDown
                {
                    Size = new Size(100, 30),
                    Location = new Point(560, 120),
                    Maximum = 255,
                    Minimum = 0,
                    Value = 9,

                };


                buttonLoadImage = new Button
                {
                    Size = new Size(100, 30),
                    Location = new Point(10, 10),
                    Text = "Reload Image"
                };

                buttonGetImage = new Button
                {
                    Size = new Size(100, 30),
                    Location = new Point(130, 10),
                    Text = "Select file"
                };
                buttonProcessImage = new Button
                {
                    Size = new Size(100, 30),
                    Location = new Point(300, 10),
                    Text = "Process Image",
                };
                buttonLoadImage.Click += new EventHandler(this.Button_Click);
                buttonGetImage.Click += new EventHandler(SelectButton_Click);
                buttonProcessImage.Click += new EventHandler(ProcessButton_Click);

                pictureBox = new PictureBox
                {
                    Location = new Point(10, 150),
                    Size = new Size(1000, 1000)
                };

                this.Controls.Add(this.pictureBox);
                this.Controls.Add(this.buttonLoadImage);
                this.Controls.Add(this.buttonProcessImage);


                openFileDialog1 = new OpenFileDialog();

                textBox1 = new TextBox
                {
                    Size = new Size(300, 400),
                    Location = new Point(10, 40),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };
                ClientSize = new Size(1080, 960);
                Controls.Add(buttonGetImage);
                Controls.Add(textBox1);

                LearnTools();


            }
            private void SetText(string text)
            {
                textBox1.Text = text;
            }
            private void SelectButton_Click(object sender, EventArgs e)
            {

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sr = new StreamReader(openFileDialog1.FileName);
                        filePath = openFileDialog1.FileName;
                        textBox1.Text = filePath;
                        string imagePath = textBox1.Text;
                        var img = new Image<Bgr, byte>(imagePath);
                        //img = img.Resize(800, 500, Emgu.CV.CvEnum.Inter.Linear);
                        this.pictureBox.Image = img.ToBitmap();
                        if (images.ContainsKey("input"))
                        {
                            images.Remove("input");
                        }

                        images.Add("input", img);
                        Console.WriteLine("Image loaded successfully.");
                    }
                    catch (SecurityException ex)
                    {
                        MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                    }
                }
            }

            private void Button_Click(object sender, EventArgs e)
            {

                string imagePath = textBox1.Text;
                var img = new Image<Bgr, byte>(imagePath);
                //img = img.Resize(800, 500, Emgu.CV.CvEnum.Inter.Linear);
                //this.pictureBox.Image = img.ToBitmap();
                if (images.ContainsKey("input"))
                {
                    images.Remove("input");
                }

                images.Add("input", img);
                Console.WriteLine("Image loaded successfully.");
            }



            private void ProcessButton_Click(object sender, EventArgs e)
            {
                try
                {
                    if (images.ContainsKey("input"))
                    {
                        var img = images["input"];

                        Mat thresholdpic = new Mat();
                        Mat edgespic = new Mat();
                        Mat bluredimg = new Mat();
                        Mat countourImg = new Mat();

                        CvInvoke.MedianBlur(img, bluredimg, 5);
                        CvInvoke.CvtColor(bluredimg, countourImg, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                        CvInvoke.Threshold(countourImg, thresholdpic, 165, 255, Emgu.CV.CvEnum.ThresholdType.Binary);



                        double mincanny = 100;
                        double maxcanny = 200;
                        CvInvoke.Canny(thresholdpic, edgespic, mincanny, maxcanny);

                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

                        CvInvoke.FindContours(edgespic,
                                            contours, thresholdpic,
                                            Emgu.CV.CvEnum.RetrType.External,
                                            Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);



                        // Filter contours
                        List<Rectangle> boundingBoxes = new List<Rectangle>();
                        for (int i = 0; i < contours.Size; i++)
                        {
                            // Calculate contour properties
                            double area = CvInvoke.ContourArea(contours[i]);
                            Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                            double aspectRatio = (double)rect.Width / rect.Height;
                            double extent = area / (rect.Width * rect.Height);

                            // Filter contours based on area, aspect ratio, and extent
                            if (area > 5)
                            {

                                boundingBoxes.Add(rect);
                                var imgtools = img.Clone();
                                imgtools.ROI = rect;
                                //this.pictureBox.Image = imgtools.ToBitmap();
                                Emgu.CV.Util.VectorOfPoint countour = contours[i];
                                tools.Add(new Tools(imgtools, area, CvInvoke.ArcLength(contours[i], true), CvInvoke.Moments(contours[i]), rect.Height, rect.Width, (double)rect.Width / rect.Height, countour));

                            }
                        }
                        ShapeMatching();



                    }
                    else
                    {
                        throw new Exception("Choose an image first");
                    }


                }
                catch { }
            }
            private void ShapeMatching()
            {
                var baseimg = images["input"];
                foreach (Tools tool in tools)
                {
                    var imgSource = tool.getimg().Convert<Gray, byte>()
                    .SmoothGaussian(3)
                    .ThresholdBinaryInv(new Gray(240), new Gray(255));

                    var imgSourceContours = CalculateContour(imgSource);
                    foreach (Tools temptool in Templatetools)
                    {
                        var imgTarget = temptool.getimg().Convert<Gray, byte>()
                        .SmoothGaussian(3)
                        .ThresholdBinaryInv(new Gray(240), new Gray(255));

                        var imgTargetContours = CalculateContour(imgTarget);
                        for (int i = 0; i < imgSourceContours.Size; i++)
                        {
                            double v = CvInvoke.MatchShapes(imgSourceContours[i], imgTargetContours[0], Emgu.CV.CvEnum.ContoursMatchType.I2, 0.1);
                            if (v < 0.8)
                            {
                                var rect = CvInvoke.BoundingRectangle(tool.getContour());
                                var img = tool.getimg();
                                baseimg.Draw(rect, new Bgr(0, 255, 0), 4);
                                CvInvoke.PutText(baseimg, "Screwdriver", new Point(rect.X, rect.Y + 10), Emgu.CV.CvEnum.FontFace.HersheyPlain, 3, new MCvScalar(255, 0, 0));
                                pictureBox.Image = baseimg.ToBitmap();
                            }
                        }
                    }

                }

            }
            private Emgu.CV.Util.VectorOfVectorOfPoint CalculateContour(Image<Gray, byte> img)
            {
                Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                Mat h = new Mat();

                CvInvoke.FindContours(img, contours, h, Emgu.CV.CvEnum.RetrType.External,
                    Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

                Emgu.CV.Util.VectorOfVectorOfPoint filteredContours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                for (int i = 0; i < contours.Size; i++)
                {
                    var area = CvInvoke.ContourArea(contours[i]);
                    if (area >= 6000)
                    {
                        filteredContours.Push(contours[i]);
                    }
                }
                return filteredContours;

            }
            private void LearnTools()
            {
                string directoryPath = @"C:\Users\coco1\OneDrive\Documents\GitHub\Traitement_Image_Project\Traitementdimage\images\screwdriver\";


                // Get all the image files in the directory
                string[] imageFiles = Directory.GetFiles(directoryPath, "*.png");

                // Loop through the image files and open each one
                foreach (string imageFile in imageFiles)
                {
                    var img = new Image<Bgr, byte>(imageFile);

                    ProcessImageTempalte(img);
                }

            }
            private void ProcessImageTempalte(Image<Bgr, byte> img)
            {
                Mat thresholdpic = new Mat();
                Mat edgespic = new Mat();
                Mat bluredimg = new Mat();
                Mat countourImg = new Mat();
                Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

                CvInvoke.GaussianBlur(img, bluredimg, new System.Drawing.Size(3, 3), 5.0);
                var average = bluredimg.ToImage<Gray, byte>().GetAverage();
                var lowerthreshold = Math.Max(0, (1.0 - 0.33) * average.Intensity);
                var upperthreshold = Math.Max(255, (1.0 + 0.33) * average.Intensity);
                double mincanny = 60;
                double maxcanny = 200;
                CvInvoke.Canny(bluredimg, edgespic, lowerthreshold, upperthreshold);

                CvInvoke.FindContours(edgespic, contours, thresholdpic, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                //CvInvoke.DrawContours(img, contours, -1, new MCvScalar(255, 0, 0));
                //CvInvoke.Imshow("show", img);
                //CvInvoke.WaitKey();
                double maxarea = 0;
                double area = 0;
                Emgu.CV.Util.VectorOfPoint contour = null;
                for (int i = 0; i < contours.Size; i++)
                {
                    // Calculate contour properties
                    area = CvInvoke.ContourArea(contours[i]);

                    pictureBox.Image = img.ToBitmap();


                    if (CvInvoke.ArcLength(contours[i], true) > maxarea)
                    {
                        maxarea = CvInvoke.ArcLength(contours[i], true);
                        contour = contours[i];

                    }

                }
                Rectangle rect = CvInvoke.BoundingRectangle(contour);

                //CvInvoke.DrawContours(img, contours, -1, new MCvScalar(255, 0, 0));
                Templatetools.Add(new Tools(img, maxarea, CvInvoke.ArcLength(contour, true), CvInvoke.Moments(contour), rect.Height, rect.Width, (double)rect.Width / rect.Height, contour));
            }


        }
    }
}
