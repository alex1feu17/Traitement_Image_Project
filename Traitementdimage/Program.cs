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
            private NumericUpDown minCanny,maxCanny,minThresh,maxThresh;
            private PictureBox pictureBox;
            private Button buttonLoadImage;
            private Button buttonGetImage;
            private Button buttonProcessImage;
            private OpenFileDialog openFileDialog1;
            private TextBox textBox1;
            private Label labelmincan,labelmaxcan,labelminthresh,labelmaxthresh;
            List<Rectangle> templateBoundingBoxes = new List<Rectangle>();
            List<Tools> tools = new List<Tools>();
            private string filePath = string.Empty;
            private Dictionary<string, Image<Bgr,byte>> images;
            public Form1()
            {
               

                images = new Dictionary<string, Image<Bgr, byte>>();
                labelmincan = new Label {
                    Size = new Size(130, 30),
                    Location = new Point(400, 20),
                    Text ="minimum Canny :"};
                labelmaxcan = new Label {
                    Size = new Size(130, 30),
                    Location = new Point(400, 55),
                    Text = "maximum Canny :" };
                labelminthresh = new Label {
                    Size = new Size(130, 30),
                    Location = new Point(400, 90),
                    Text = "minimum threshold :" };
                labelmaxthresh = new Label {
                    Size = new Size(130, 30),
                    Location = new Point(400, 120),
                    Text = "maximum threshold :" };
                minCanny = new NumericUpDown {
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
                

                buttonLoadImage = new Button {
                    Size =  new Size(100, 30),
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

                pictureBox = new PictureBox {
                    Location = new Point(10, 150),
                    Size = new Size(1000, 1000)
                };
              
                this.Controls.Add(this.pictureBox);
                this.Controls.Add(this.buttonLoadImage);
                this.Controls.Add(this.buttonProcessImage);
                this.Controls.Add(labelmincan);
                this.Controls.Add(labelmaxcan);
                this.Controls.Add(labelminthresh);
                this.Controls.Add(labelmaxthresh);

                openFileDialog1 = new OpenFileDialog();
        
                textBox1 = new TextBox
                {
                    Size = new Size(300, 400),
                    Location = new Point(10, 40),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };
                ClientSize = new Size(930, 960);
                Controls.Add(buttonGetImage);
                Controls.Add(textBox1);
                SelectTempalte();
                ProcessTemplate();


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
                        img = img.Resize(800, 500, Emgu.CV.CvEnum.Inter.Linear);
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
            private void SelectTempalte()
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
                        img = img.Resize(800, 500, Emgu.CV.CvEnum.Inter.Linear);
                       
                        if (images.ContainsKey("template"))
                        {
                            images.Remove("template");
                        }

                        images.Add("template", img);
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
                img = img.Resize(800, 500, Emgu.CV.CvEnum.Inter.Linear);
                this.pictureBox.Image = img.ToBitmap();
                if (images.ContainsKey("input"))
                {
                    images.Remove("input");
                }

                images.Add("input", img);
                Console.WriteLine("Image loaded successfully.");
            }

          
          
            private void ProcessButton_Click(object sender, EventArgs e)
            {
                try {
                    if (images.ContainsKey("input"))
                    {
                        var img = images["input"];

                        Mat thresholdpic = new Mat();
                        Mat edgespic =new Mat();
                        Mat bluredimg = new Mat();
                        Mat countourImg =new Mat();

                        CvInvoke.MedianBlur(img,bluredimg, 5);
                        CvInvoke.CvtColor(bluredimg, countourImg,Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                        this.pictureBox.Image = countourImg.ToBitmap();
                        CvInvoke.Threshold(countourImg, thresholdpic, 170, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                        

                        int minthresh = 9;
                        double maxcthresh = 19;
                        //CvInvoke.AdaptiveThreshold(countourImg, thresholdpic,255,Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary,minthresh,maxcthresh);

                        double mincanny = 100;
                        double maxcanny = 200;
                        CvInvoke.Canny(thresholdpic, edgespic, mincanny, maxcanny);
                        
                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                        
                          CvInvoke.FindContours(edgespic,
                                              contours,thresholdpic,
                                              Emgu.CV.CvEnum.RetrType.External,
                                              Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                        //CvInvoke.DrawContours(img, contours, -1, new MCvScalar(255, 0, 0));
                        

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
                            if (extent < 0.80 && CvInvoke.ArcLength(contours[i], true) > 300&& aspectRatio < 0.3)
                            {
                                tools.Add(new Tools(area,CvInvoke.ArcLength(contours[i],true),CvInvoke.Moments(contours[i]),rect.Height,rect.Width, (double)rect.Width / rect.Height));
                                boundingBoxes.Add(rect);
                            }
                        }
                        var imgScene= img.Clone();
                        Mat imgOut = new Mat();
                        // Draw bounding boxes
                        foreach (Rectangle rect in boundingBoxes)
                        {
                            
                            imgScene.ROI = rect;
                            CvInvoke.Rectangle(img, rect, new MCvScalar(0, 0, 255), 2);
                            var templaeteImg = images["template"];
                            

                            
                            


                        }
                        this.pictureBox.Image = img.ToBitmap();



                    }
                    else {
                        throw new Exception("Choose an image first");
                    }
                       
                
                } catch { }
            }
            private void ProcessTemplate()
            {
                try
                {
                    if (images.ContainsKey("template"))
                    {
                        var img = images["template"];

                        Mat thresholdpic = new Mat();
                        Mat edgespic = new Mat();
                        Mat bluredimg = new Mat();
                        Mat countourImg = new Mat();

                        CvInvoke.MedianBlur(img, bluredimg, 5);
                        CvInvoke.CvtColor(bluredimg, countourImg, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                        this.pictureBox.Image = countourImg.ToBitmap();
                        CvInvoke.Threshold(countourImg, thresholdpic, 170, 255, Emgu.CV.CvEnum.ThresholdType.Binary);


                        int minthresh = (int)minThresh.Value;
                        double maxcthresh = (double)maxThresh.Value;
                        //CvInvoke.AdaptiveThreshold(countourImg, thresholdpic,255,Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary,minthresh,maxcthresh);

                        double mincanny = (double)minCanny.Value;
                        double maxcanny = (double)maxCanny.Value;
                        CvInvoke.Canny(thresholdpic, edgespic, mincanny, maxcanny);

                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

                        CvInvoke.FindContours(edgespic,
                                            contours, thresholdpic,
                                            Emgu.CV.CvEnum.RetrType.External,
                                            Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                        CvInvoke.DrawContours(img, contours, -1, new MCvScalar(255, 0, 0));


                        // Filter contours
                        templateBoundingBoxes = new List<Rectangle>();
                        for (int i = 0; i < contours.Size; i++)
                        {
                            // Calculate contour properties
                            double area = CvInvoke.ContourArea(contours[i]);
                            Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                            double aspectRatio = (double)rect.Width / rect.Height;
                            double extent = area / (rect.Width * rect.Height);

                            // Filter contours based on area, aspect ratio, and extent
                            if (area > 50)
                            {
                                templateBoundingBoxes.Add(rect);
                            }
                        }

                        // Draw bounding boxes
                        foreach (Rectangle rect in templateBoundingBoxes)
                        {
                            CvInvoke.Rectangle(img, rect, new MCvScalar(0, 0, 255), 2);


                        }

                        this.pictureBox.Image = img.ToBitmap();


                    }
                    else
                    {
                        throw new Exception("Choose an image first");
                    }


                }
                catch { }
            }
        }
    }
}
