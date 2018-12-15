# FEMRebarToRevit
Proof of concept import of rebar calculated in [StruSoft FEM Design](https://strusoft.com/products/fem-design) to [Autodesk Revit](https://www.autodesk.com/products/revit/overview).

## Prerequisite
You need the [StruSoft Revit Add-in](https://strusoft.com/products/fem-design/revit-link) to export a structure from Revit to a struXML-file.

## User guide
* Export a structure from Revit to a struXML-file (using the add-in from StruSoft).
* Open the struXML-file in FEM Design, analyze and add rebars.
* Save the structure as struXML
* Start FEMRebarToRevit and select the struXML file.
* The rebars are now created in Revit!

## Known limitations
* Rebar cover is not copied correctly
