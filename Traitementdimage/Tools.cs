using Emgu.CV;
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

        public Tools(double area, double v1, Moments moments, int height, int width, double v2)
        {
            this.area = area;
            this.perimeter = v1;
            this.moments = moments;
            this.height = height;
            this.width = width;
            //this.area = v2;
        }

        
    }
}
