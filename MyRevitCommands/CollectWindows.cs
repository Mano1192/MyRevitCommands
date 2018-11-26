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

    public class CollectWindows : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
          
            //Get UIDocument
            UIDocument uIDoc = commandData.Application.ActiveUIDocument;
            //Get Document
            Document Doc = uIDoc.Document;

            try
            {

                //Create Filtered Element Collector
                FilteredElementCollector collector = new FilteredElementCollector(Doc);

                //Create Filter
                ElementQuickFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);

                //Apply Filter
                IList<Element> windows = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

                TaskDialog.Show("Windows", string.Format("{0} windows counted!,", windows.Count));

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
