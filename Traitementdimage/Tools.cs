using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traitementdimage
{
    internal class Tools
    {
        private double surface;
        private double perimeter;
        private double centerOfGravity;
        private double height;
        private double width;
        private double ratio;
        private double area;
        private double v1;
        private Moments moments;
        private double v2;
        private Image<Bgr, byte> img;
        Emgu.CV.Util.VectorOfPoint contour;

        

        public Tools(Image<Bgr, byte> img ,double area, double v1, Moments moments, int height, int width, double v2, Emgu.CV.Util.VectorOfPoint contour)
        {
            this.img = img;
            this.area = area;
            this.perimeter = v1;
            this.moments = moments;
            this.height = height;
            this.width = width;
            this.contour = contour;
        }

        public Image<Bgr, byte> Imgtools { get; }

        public Emgu.CV.Util.VectorOfPoint getContour()
        {
           
            return contour;
        }
        public Image<Bgr, byte> getimg()
        {
            return img;
        }

    }
}
