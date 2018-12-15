using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMRebarToRevit
{
    class FemDesignRebar
    {
        public FemDesignRebar()
        {

        }

        // Connections to other elements
        public string hostGuid;
        public string rebarParametesrGuid;

        //Rebar property
        public string layer;
        public string direction;
        public double spacing;
        public double cover;
        public double diameter;

        //Rebar geometry
        public List<FemDesignLine> regionCurves = new List<FemDesignLine>();
        public FemDesignXYZ majorDirection;
        
        public void SetSpacing(string spacing)
        {
            this.spacing = Double.Parse(spacing, System.Globalization.CultureInfo.InvariantCulture);
        }

        public void SetCover(string cover)
        {
            this.cover = Double.Parse(cover, System.Globalization.CultureInfo.InvariantCulture);
        }

        public void SetDiameter(string diameter)
        {
            this.diameter = Double.Parse(diameter, System.Globalization.CultureInfo.InvariantCulture);
        }



    }
}
