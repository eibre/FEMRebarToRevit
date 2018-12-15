using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace FEMRebarToRevit
{
    //Set the attributes
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CreateRebarFromXml : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application app = commandData.Application.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FemDesignRebar> femRebars = ReadRebarFromXML.GetFemRebars();

            using (Transaction t1 = new Transaction(doc, "new area reinforcement"))
            {
                t1.Start();
                foreach(FemDesignRebar rebar in femRebars)
                {
                    FemDesignRebarToRevit(doc, rebar);
                }

                t1.Commit();
            }
            return Result.Succeeded;
        }

        private AreaReinforcement FemDesignRebarToRevit(Document doc, FemDesignRebar femRebar)
        {
            string hostGuid = femRebar.hostGuid;
            double meterToFeet = UnitUtils.ConvertToInternalUnits(1, DisplayUnitType.DUT_METERS);
            double diameter = femRebar.diameter * meterToFeet;
            double spacing = femRebar.spacing * meterToFeet;
            XYZ majorDirection = new XYZ(femRebar.majorDirection.x, femRebar.majorDirection.y, femRebar.majorDirection.z);
            
            Dictionary<string, int> isActiveDirection = new Dictionary<string, int>()
                {
                {"x top", 0},
                {"x bottom", 0},
                {"y top", 0},
                {"y bottom", 0}
            };

            isActiveDirection[femRebar.direction + " " + femRebar.layer] = 1;

            Element rebarHost = null;
            FilteredElementCollector fec = new FilteredElementCollector(doc).OfClass(typeof(AnalyticalModelSurface)).WhereElementIsNotElementType();
            foreach (AnalyticalModelSurface s in fec)
            {
                //if(e.get_Parameter(new Guid("7f162ddd-61bb-43f5-9394-846db8aef825")) == null) continue;
                //if(e.get_Parameter(new Guid("7f162ddd-61bb-43f5-9394-846db8aef825")).AsString() == hostGuid) {
                //if (s.LookupParameter("StruXML Guid") == null) continue;
                //if (s.LookupParameter("StruXML Guid").AsString() == hostGuid)
                //{
                //    rebarHost = doc.GetElement(s.GetElementId());
                //    break;
                //}

                // FEM-design guid is stored in Extensible Storage
                if (s.GetEntity(Schema.Lookup(new Guid("3a97c049-093e-46e6-b854-0e2323d640d0"))) == null) continue;
                Entity ent = s.GetEntity(Schema.Lookup(new Guid("3a97c049-093e-46e6-b854-0e2323d640d0")));
                if (ent.Get<Guid>("Guid").ToString() == hostGuid)
                {
                    rebarHost = doc.GetElement(s.GetElementId());
                    break;
                }
             }
            
            //The boundary curves of the area rebar system
            IList<Curve> curveList = new List<Curve>();
            foreach (FemDesignLine femLine in femRebar.regionCurves)
            {
                XYZ p1 = new XYZ(femLine.startPoint.x * meterToFeet, femLine.startPoint.y * meterToFeet, femLine.startPoint.z * meterToFeet);
                XYZ p2 = new XYZ(femLine.endPoint.x * meterToFeet, femLine.endPoint.y * meterToFeet, femLine.endPoint.z * meterToFeet);
                Line l1 = Line.CreateBound(p1, p2);
                curveList.Add(l1);
            }

            ElementId rebarTypeId = new FilteredElementCollector(doc).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().FirstOrDefault(x => Math.Abs(x.BarDiameter - diameter) < 0.0001).Id;
            ElementId areaReinfType = new FilteredElementCollector(doc).OfClass(typeof(AreaReinforcementType)).FirstOrDefault().Id;

            //Create area reinforcement
            AreaReinforcement areaReinf1 = AreaReinforcement.Create(doc, rebarHost, curveList, majorDirection, areaReinfType, rebarTypeId, ElementId.InvalidElementId);

            //set spacing
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_1_GENERIC).Set(spacing);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_2_GENERIC).Set(spacing);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_1_GENERIC).Set(spacing);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_2_GENERIC).Set(spacing);

            //Area reinforcement in Revit can have up to 4 layers and FEM-Design considers one at a time. Deactivating the three unused layers:
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1_GENERIC).Set(isActiveDirection["x top"]);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2_GENERIC).Set(isActiveDirection["x bottom"]);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1_GENERIC).Set(isActiveDirection["y top"]);
            areaReinf1.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2_GENERIC).Set(isActiveDirection["y bottom"]);

            //TODO: Cover

            //TODO: Option do remove area system, and correct spacing

            return areaReinf1;
        }
    } //Class
} //namespace