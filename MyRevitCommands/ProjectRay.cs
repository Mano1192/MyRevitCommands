using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]

    //******************************* Main Method to move elements to ceilings ***************************
    public class ProjectRay : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument and Document
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            
            try
            {
                //Create Filtered Element Collector
                FilteredElementCollector atCollector = new FilteredElementCollector(doc);
                FilteredElementCollector lfCollector = new FilteredElementCollector(doc);
                //Create Filter - All Air Terminals in project | Lights in project
                ElementCategoryFilter terminalsFilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctTerminal);
                ElementCategoryFilter lightsFilter = new ElementCategoryFilter(BuiltInCategory.OST_LightingFixtures);

                //Create lists with the filters
                IList<Element> airTerminals = atCollector.WherePasses(terminalsFilter).WhereElementIsNotElementType().ToElements();
                IList<Element> lightingFixtures = lfCollector.WherePasses(lightsFilter).WhereElementIsNotElementType().ToElements();

                if (airTerminals != null)
                {
                    //Check for ceiling above and move air terminals up
                    foreach (Element ele in airTerminals)
                    {
                        //Get air terminal height
                        Parameter atHeight = ele.LookupParameter("Offset");

                        //Get air terminal name / Boolean for filtering out wall based
                        FamilyInstance familyInstance = ele as FamilyInstance;
                        FamilySymbol familySymbol = familyInstance.Symbol;
                        String familyName = familySymbol.FamilyName;
                        Boolean atWallBased = familyName.Contains("wall") || familyName.Contains("Wall") || familyName.Contains("Louver");

                        if (atWallBased != true)
                        {

                        

                        //Parameter atHeight = ele.get_Parameter(BuiltInParameter.offset)
                        //Create a Project Ray from element location
                        LocationPoint locP = ele.Location as LocationPoint;
                        XYZ p1 = locP.Point;

                        //Ray Direction Up
                        XYZ RayZ = new XYZ(0, 0, 1);

                        //******************************* Determine what the ray will intersect with***********************************
                        
                        //Only getting interseting elements from the active view not entire model | change to whole model once code is completed
                        //or open a predetermined view

                        //Filter 
                        ElementCategoryFilter filterC = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

                        ReferenceIntersector refI = new ReferenceIntersector(filterC, FindReferenceTarget.Element, (View3D)doc.ActiveView)
                        //ReferenceIntersector refI = new ReferenceIntersector((View3D)doc.ActiveView)
                        {
                            FindReferencesInRevitLinks = true
                        };
                        ReferenceWithContext refC = refI.FindNearest(p1, RayZ);

                            //******************************************************************************************************************

                            if (refC != null)
                            {

                                Reference refel = refC.GetReference();
                                XYZ intPoint = refel.GlobalPoint;
                                Double dist = p1.DistanceTo(intPoint);

                                RevitLinkInstance linkinstance = doc.GetElement(refel.ElementId) as RevitLinkInstance;
                                Element linkElem = linkinstance.GetLinkDocument().GetElement(refel.LinkedElementId);

                                Parameter pHeight = linkElem.LookupParameter("Height Offset From Level");

                                if (pHeight.AsValueString() != atHeight.AsValueString())
                                {
                                    //Set Parameter Value
                                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                                    {
                                        trans.Start();

                                        atHeight.SetValueString(pHeight.AsValueString());

                                        trans.Commit();

                                        //Category refCat = linkElem.Category;
                                        //TaskDialog.Show("Ray", string.Format("Intersecting Ceiling offset is {0}",
                                        //    pHeight.AsValueString() + Environment.NewLine
                                        //    + "Diffuser Height: " + ele.LookupParameter("Offset").AsValueString() + Environment.NewLine
                                        //    + "Category: " + ele.Category.Name + Environment.NewLine
                                        //    + "Instance: " + ele.Name + Environment.NewLine)
                                        //    );

                                    }
                                }

                            }   

                        }

                    }
                    //Check for ceiling below and move air terminals down
                    foreach (Element ele in airTerminals)
                    {
                        //Get air terminal height
                        Parameter atHeight = ele.LookupParameter("Offset");

                        //Get air terminal name / Boolean for filtering out wall based
                        FamilyInstance familyInstance = ele as FamilyInstance;
                        FamilySymbol familySymbol = familyInstance.Symbol;
                        String familyName = familySymbol.FamilyName;
                        Boolean atWallBased = familyName.Contains("wall") || familyName.Contains("Wall") || familyName.Contains("Louver");

                        if (atWallBased != true)
                        {



                        //Create a Project Ray from element location
                        LocationPoint locP = ele.Location as LocationPoint;
                        XYZ p1 = locP.Point;

                        //Ray Direction Up
                        XYZ RayZ = new XYZ(0, 0, -1);

                        //******************************* Determine what the ray will intersect with***********************************

                        //Only getting interseting elements from the active view not entire model | change to whole model once code is completed
                        //or open a predetermined view

                        //Filter 
                        ElementCategoryFilter filterC = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

                        ReferenceIntersector refI = new ReferenceIntersector(filterC, FindReferenceTarget.Element, (View3D)doc.ActiveView)
                        //ReferenceIntersector refI = new ReferenceIntersector((View3D)doc.ActiveView)
                        {
                            FindReferencesInRevitLinks = true
                        };
                        ReferenceWithContext refC = refI.FindNearest(p1, RayZ);

                            //******************************************************************************************************************

                            if (refC != null)
                            {

                                Reference refel = refC.GetReference();
                                XYZ intPoint = refel.GlobalPoint;
                                Double dist = p1.DistanceTo(intPoint);

                                RevitLinkInstance linkinstance = doc.GetElement(refel.ElementId) as RevitLinkInstance;
                                Element linkElem = linkinstance.GetLinkDocument().GetElement(refel.LinkedElementId);

                                Parameter pHeight = linkElem.LookupParameter("Height Offset From Level");

                                if (pHeight.AsValueString() != atHeight.AsValueString())
                                {
                                    //Set Parameter Value
                                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                                    {
                                        trans.Start();

                                        atHeight.SetValueString(pHeight.AsValueString());

                                        trans.Commit();

                                        //Category refCat = linkElem.Category;
                                        //TaskDialog.Show("Ray", string.Format("Intersecting Ceiling offset is {0}",
                                        //    pHeight.AsValueString() + Environment.NewLine
                                        //    + "Diffuser Height: " + ele.LookupParameter("Offset").AsValueString() + Environment.NewLine
                                        //    + "Category: " + ele.Category.Name + Environment.NewLine
                                        //    + "Instance: " + ele.Name + Environment.NewLine)
                                        //    );

                                    }
                                }

                            }

                        }

                    }
                }

                if (lightingFixtures != null)
                {
                    //Check for ceiling above and move air terminals up
                    foreach (Element ele in lightingFixtures)
                    {
                        //Get lighting fixture height
                        Parameter lfHeight = ele.LookupParameter("Offset");

                        //Get air terminal name / Boolean for filtering out wall based
                        FamilyInstance familyInstance = ele as FamilyInstance;
                        FamilySymbol familySymbol = familyInstance.Symbol;
                        String familyName = familySymbol.FamilyName;
                        Boolean lfWallBased = familyName.Contains("wall") || familyName.Contains("Wall");

                        if (lfWallBased != true)
                        {



                            //Parameter atHeight = ele.get_Parameter(BuiltInParameter.offset)
                            //Create a Project Ray from element location
                            LocationPoint locP = ele.Location as LocationPoint;
                            XYZ p1 = locP.Point;

                            //Ray Direction Up
                            XYZ RayZ = new XYZ(0, 0, 1);

                            //******************************* Determine what the ray will intersect with***********************************

                            //Only getting interseting elements from the active view not entire model | change to whole model once code is completed
                            //or open a predetermined view

                            //Filter 
                            ElementCategoryFilter filterC = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

                            ReferenceIntersector refI = new ReferenceIntersector(filterC, FindReferenceTarget.Element, (View3D)doc.ActiveView)
                            //ReferenceIntersector refI = new ReferenceIntersector((View3D)doc.ActiveView)
                            {
                                FindReferencesInRevitLinks = true
                            };
                            ReferenceWithContext refC = refI.FindNearest(p1, RayZ);

                            //******************************************************************************************************************

                            if (refC != null)
                            {

                                Reference refel = refC.GetReference();
                                XYZ intPoint = refel.GlobalPoint;
                                Double dist = p1.DistanceTo(intPoint);

                                RevitLinkInstance linkinstance = doc.GetElement(refel.ElementId) as RevitLinkInstance;
                                Element linkElem = linkinstance.GetLinkDocument().GetElement(refel.LinkedElementId);

                                Parameter pHeight = linkElem.LookupParameter("Height Offset From Level");

                                if (pHeight.AsValueString() != lfHeight.AsValueString())
                                {
                                    //Set Parameter Value
                                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                                    {
                                        trans.Start();

                                        lfHeight.SetValueString(pHeight.AsValueString());

                                        trans.Commit();

                                        //Category refCat = linkElem.Category;
                                        //TaskDialog.Show("Ray", string.Format("Intersecting Ceiling offset is {0}",
                                        //    pHeight.AsValueString() + Environment.NewLine
                                        //    + "Diffuser Height: " + ele.LookupParameter("Offset").AsValueString() + Environment.NewLine
                                        //    + "Category: " + ele.Category.Name + Environment.NewLine
                                        //    + "Instance: " + ele.Name + Environment.NewLine)
                                        //    );

                                    }
                                }

                            }

                        }

                    }
                    //Check for ceiling below and move air terminals down
                    foreach (Element ele in lightingFixtures)
                    {
                        //Get lighting fixture height
                        Parameter lfHeight = ele.LookupParameter("Offset");

                        //Get air terminal name / Boolean for filtering out wall based
                        FamilyInstance familyInstance = ele as FamilyInstance;
                        FamilySymbol familySymbol = familyInstance.Symbol;
                        String familyName = familySymbol.FamilyName;
                        Boolean lfWallBased = familyName.Contains("wall") || familyName.Contains("Wall");

                        if (lfWallBased != true)
                        {



                            //Parameter atHeight = ele.get_Parameter(BuiltInParameter.offset)
                            //Create a Project Ray from element location
                            LocationPoint locP = ele.Location as LocationPoint;
                            XYZ p1 = locP.Point;

                            //Ray Direction Up
                            XYZ RayZ = new XYZ(0, 0, -1);

                            //******************************* Determine what the ray will intersect with***********************************

                            //Only getting interseting elements from the active view not entire model | change to whole model once code is completed
                            //or open a predetermined view

                            //Filter 
                            ElementCategoryFilter filterC = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

                            ReferenceIntersector refI = new ReferenceIntersector(filterC, FindReferenceTarget.Element, (View3D)doc.ActiveView)
                            //ReferenceIntersector refI = new ReferenceIntersector((View3D)doc.ActiveView)
                            {
                                FindReferencesInRevitLinks = true
                            };
                            ReferenceWithContext refC = refI.FindNearest(p1, RayZ);

                            //******************************************************************************************************************

                            if (refC != null)
                            {

                                Reference refel = refC.GetReference();
                                XYZ intPoint = refel.GlobalPoint;
                                Double dist = p1.DistanceTo(intPoint);

                                RevitLinkInstance linkinstance = doc.GetElement(refel.ElementId) as RevitLinkInstance;
                                Element linkElem = linkinstance.GetLinkDocument().GetElement(refel.LinkedElementId);

                                Parameter pHeight = linkElem.LookupParameter("Height Offset From Level");

                                if (pHeight.AsValueString() != lfHeight.AsValueString())
                                {
                                    //Set Parameter Value
                                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                                    {
                                        trans.Start();

                                        lfHeight.SetValueString(pHeight.AsValueString());

                                        trans.Commit();

                                        //Category refCat = linkElem.Category;
                                        //TaskDialog.Show("Ray", string.Format("Intersecting Ceiling offset is {0}",
                                        //    pHeight.AsValueString() + Environment.NewLine
                                        //    + "Diffuser Height: " + ele.LookupParameter("Offset").AsValueString() + Environment.NewLine
                                        //    + "Category: " + ele.Category.Name + Environment.NewLine
                                        //    + "Instance: " + ele.Name + Environment.NewLine)
                                        //    );

                                    }
                                }

                            }

                        }

                    }
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }






        }


        //********************** GetParameterValue() ****************************
        public string GetParameterValue(Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return parameter.AsValueString();

                case StorageType.ElementId:
                    return parameter.AsElementId().IntegerValue.ToString();

                case StorageType.Integer:
                    return parameter.AsValueString();

                case StorageType.None:
                    return parameter.AsValueString();

                case StorageType.String:
                    return parameter.AsString();

                default:
                    return "";

            }

        }

    }

}
