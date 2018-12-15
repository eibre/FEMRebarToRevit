using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLRebarReader
{
    class FemDesignXYZ
    {
        public double x;
        public double y;
        public double z;

        public FemDesignXYZ(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public FemDesignXYZ(string x, string y, string z)
        {
            this.x = Double.Parse(x, System.Globalization.CultureInfo.InvariantCulture);
            this.y = Double.Parse(y, System.Globalization.CultureInfo.InvariantCulture);
            this.z = Double.Parse(z, System.Globalization.CultureInfo.InvariantCulture);

        }
    }

    class FemDesignLine
    {
        public FemDesignXYZ startPoint;
        public FemDesignXYZ endPoint;
        public FemDesignXYZ normalVector;

        public FemDesignLine(FemDesignXYZ startPoint, FemDesignXYZ endPoint, FemDesignXYZ normalVector)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.normalVector = normalVector;
        }

    }
}
