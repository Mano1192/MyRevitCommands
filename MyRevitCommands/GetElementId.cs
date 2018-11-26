using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]

    public class GetElementId : IExternalCommand
    {
        public GetElementId(Element ele)
        {
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
          
            //Get UIDocument
            UIDocument uIDoc = commandData.Application.ActiveUIDocument;
            //Get Document
            Document Doc = uIDoc.Document;

            try
            { 

                //Pick Object
                Reference pickedObj = uIDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                //Retrieve Element
                ElementId eleId = pickedObj.ElementId;
                Element ele = Doc.GetElement(eleId);

                //Get ElementType
                ElementId eTypeId = ele.GetTypeId();
                ElementType eType = Doc.GetElement(eTypeId) as ElementType;

                //Display Element ID
                if (pickedObj != null)
                {
                    TaskDialog.Show("Element Id: ", pickedObj.ElementId.ToString() + Environment.NewLine
                        + "Category: " + ele.Category.Name + Environment.NewLine
                        + "Instance: " + ele.Name + Environment.NewLine
                        + "Symbol: " + eType.Name + Environment.NewLine
                        + "Family: " + eType.FamilyName);                     
                }

            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }


            return Result.Succeeded;
        }
    }
}
