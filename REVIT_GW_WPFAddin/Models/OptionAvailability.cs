using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace REVIT_GW_WPFAddin
{
    internal class OptionAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication a, CategorySet b)
        {
            if (a.ActiveUIDocument != null && new FilteredElementCollector(a.ActiveUIDocument.Document)
                .OfCategory(BuiltInCategory.OST_DesignOptionSets)
                .GetElementCount() != 0)
            {
                List<ElementId> elementsIds = new List<ElementId>();
                Selection selectionSet = a.ActiveUIDocument.Selection;
                elementsIds = selectionSet.GetElementIds().ToList();

                if (elementsIds.Count != 0)
                {
                    //Create an element collector and see if selection has any sheets
                    FilteredElementCollector SheetsInSelection = new FilteredElementCollector(a.ActiveUIDocument.Document, elementsIds);
                    FilteredElementCollector ViewsInSelection = new FilteredElementCollector(a.ActiveUIDocument.Document, elementsIds);

                    ElementClassFilter UnWantedSheetsElements = new ElementClassFilter(typeof(ViewSheet));
                    SheetsInSelection.WherePasses(UnWantedSheetsElements);

                    ElementClassFilter WantedViewsElements = new ElementClassFilter(typeof(View));
                    ViewsInSelection.WherePasses(WantedViewsElements);

                    List<Element> selectedSheets = SheetsInSelection.ToElements().ToList();
                    List<Element> selectedViews = ViewsInSelection.ToElements().ToList();

                    if (elementsIds.Count == selectedViews.Count && selectedSheets.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (elementsIds.Count == 0 && a.ActiveUIDocument.ActiveGraphicalView.ViewType != ViewType.DrawingSheet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}