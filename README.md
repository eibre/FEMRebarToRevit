# FEMRebarToRevit
Proof of concept import of rebar calculated in [StruSoft FEM Design](https://strusoft.com/products/fem-design) to [Autodesk Revit](https://www.autodesk.com/products/revit/overview).

## Prerequisite
You need the [StruSoft Revit Add-in](https://strusoft.com/products/fem-design/revit-link) to export a structure from Revit to a struXML-file.

## User guide
* Export a structure from Revit to a struXML-file (using the add-in from StruSoft).
* Save the Revit file to save the FEM-Design guid in the Revit element.
* Open the struXML-file in FEM Design, analyze and add rebars.
* Save the FEM-Design model as struXML
* Start FEMRebarToRevit and select the struXML file. (Add-ins > External Tools)
* The rebars are now created in Revit!
