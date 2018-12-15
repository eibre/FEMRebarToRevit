using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace FEMRebarToRevit
{
    class ReadRebarFromXML
    {
        public static List<FemDesignRebar> GetFemRebars()
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "struxml files (*.struxml)|*.struxml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }

            XDocument xmlDoc = XDocument.Load(filePath);
            //XNamespace ns = "urn:strusoft";
            XNamespace ns = xmlDoc.Root.GetDefaultNamespace();

            IEnumerable<XElement> rebarElements = xmlDoc.Root.Element(ns + "entities").Elements(ns + "surface_reinforcement");
            List<FemDesignRebar> femRebarList = new List<FemDesignRebar>();

            foreach (XElement xe in rebarElements)
            {
                FemDesignRebar femRebar = new FemDesignRebar()
                {
                    hostGuid = xe.Element(ns + "base_shell").Attribute("guid").Value,
                    rebarParametesrGuid = xe.Element(ns + "surface_reinforcement_parameters").Attribute("guid").Value,
                    layer = xe.Element(ns + "straight").Attribute("face").Value,
                    direction = xe.Element(ns + "straight").Attribute("direction").Value,
                };
                femRebar.SetSpacing(xe.Element(ns + "straight").Attribute("space").Value);
                femRebar.SetCover(xe.Element(ns + "straight").Attribute("cover").Value);
                femRebar.SetDiameter(xe.Element(ns + "wire").Attribute("diameter").Value);
            
                // Get geometry
                IEnumerable<XElement> edges = xe.Element(ns + "region").Element(ns + "contour").Elements(ns + "edge");
                foreach (XElement edge in edges)
                {
                    XElement startPoint = edge.Elements(ns + "point").First();
                    XElement endPoint = edge.Elements(ns + "point").ElementAt(1);
                    XElement normal = edge.Element(ns+"normal");
                    FemDesignXYZ startPt = new FemDesignXYZ(startPoint.Attribute("x").Value, startPoint.Attribute("y").Value, startPoint.Attribute("z").Value);
                    FemDesignXYZ endPt = new FemDesignXYZ(endPoint.Attribute("x").Value, endPoint.Attribute("y").Value, endPoint.Attribute("z").Value);
                    FemDesignXYZ normalVector = new FemDesignXYZ(normal.Attribute("x").Value, normal.Attribute("y").Value, normal.Attribute("z").Value);
                    femRebar.regionCurves.Add(new FemDesignLine(startPt, endPt, normalVector));
                }

                // Get major direction
                XElement reinforcementParameters = xmlDoc.Root.Element(ns + "entities").Elements(ns + "surface_reinforcement_parameters").First(e => e.Attribute("guid").Value == femRebar.rebarParametesrGuid);
                XElement xVec = reinforcementParameters.Element(ns + "x_direction");
                FemDesignXYZ xDirection = new FemDesignXYZ(xVec.Attribute("x").Value, xVec.Attribute("y").Value, xVec.Attribute("z").Value);
                femRebar.majorDirection = xDirection;
                femRebarList.Add(femRebar);
            }
            return femRebarList;
            
        }   
    }
}
