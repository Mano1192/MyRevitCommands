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
    [TransactionAttribute(TransactionMode.Manual)]

    public class DeleteElements : IExternalCommand
    {

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

                if (pickedObj != null)
                {
                    using (Transaction trans = new Transaction(Doc, "Delete Element"))
                    {
                        trans.Start();
                        Doc.Delete(pickedObj.ElementId);

                        TaskDialog tDialog = new TaskDialog("Delete Element");
                        tDialog.MainContent = "Are you sure you want to delete this element?";
                        tDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.Cancel;
                        
                        if (tDialog.Show() == TaskDialogResult.Yes)
                        {
                            trans.Commit();
                            TaskDialog.Show("Delete", pickedObj.ElementId.ToString() + " deleted");
                        }
                        else
                        {
                            trans.RollBack();
                            TaskDialog.Show("Delete", pickedObj.ElementId.ToString() + " not deleted");
                        }

                    }
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
