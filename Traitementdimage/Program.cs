using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
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
            private PictureBox pictureBox;
            private Button buttonLoadImage;
            private Button buttonGetImage;
            private OpenFileDialog openFileDialog1;
            private TextBox textBox1;
            private string filePath = string.Empty;

            public Form1()
            {           
                
                buttonLoadImage = new Button {
                    Size =  new Size(100, 30),
                    Location = new Point(10, 10),
                    Text = "Load Image",
                };
               
                buttonGetImage = new Button
                {
                    Size = new Size(100, 30),
                    Location = new Point(130, 10),
                    Text = "Select file"
                };
                buttonLoadImage.Click += new EventHandler(this.Button_Click);
                buttonGetImage.Click += new EventHandler(SelectButton_Click);

                pictureBox = new PictureBox {
                    Location = new Point(10, 150),
                    Size = new Size(1000, 1000)
                };

                this.Controls.Add(this.pictureBox);
                this.Controls.Add(this.buttonLoadImage);

                openFileDialog1 = new OpenFileDialog();
        
                textBox1 = new TextBox
                {
                    Size = new Size(300, 400),
                    Location = new Point(10, 40),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };
                ClientSize = new Size(330, 360);
                Controls.Add(buttonGetImage);
                Controls.Add(textBox1);
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

                Bitmap image = new Bitmap(imagePath);
                this.pictureBox.Image = image;
                Console.WriteLine("Image loaded successfully.");
            }
        }
    }
}
