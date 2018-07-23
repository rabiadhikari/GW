using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace REVIT_GW_WPFAddin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            //Document doc = uidoc.Document;
            Globals.RevitVersion = app.VersionNumber;

            //Process the Window and populate data for display
            DocumentUtils.LoadRevitDocumentAndProcess(uidoc); //Get RevitUIDocument be loaded into a Static reference for ViewModel class to deal with.
            DocumentUtils.GetAllDesignOptionsFromRevit(uidoc.Document); //Get DesignOptions to the static resource
            DocumentUtils.GetAllOptionSetIdsFromRevit(); //Get OptionSet Ids to the static resource
            DocumentUtils.GetAllSelectedViewsFromRevit(uidoc.Document); //Get Views from the selection and check if the view types allows for Design options
            DocumentUtils.GetAllViewsFromRevit(uidoc.Document);

            Views.GenslerWindow designOptionsViewManagerWindow = new Views.GenslerWindow(); //Create an instance of Interface window. This is the XAML instance.
            designOptionsViewManagerWindow.DataContext = ViewModels.App_ViewModel.Instance; //And Finally Present the dialog box to the user. Here the logic to validate users current document and selection in case Design options views are invalid.
            ViewModels.App_ViewModel.Instance.ProcessData();

            ViewModels.App_ViewModel.Instance.GetAllSelectedDesignOptions(); //Selected Design Options
            ViewModels.App_ViewModel.Instance.GetProposedViewNames(); //Prepopulate proposed View names
            ViewModels.App_ViewModel.Instance.CheckViewNamesForDuplicates(); //Check the view names if duplicates. This is also checked in GetProposedViewNames(). Check for redundant calls later.

            //The following CloseAction is to make window interact with the iCommand in the View model.
            if (ViewModels.App_ViewModel.Instance.CloseAction == null)
                ViewModels.App_ViewModel.Instance.CloseAction += designOptionsViewManagerWindow.Close;

            designOptionsViewManagerWindow.ShowDialog(); //Show Dialog will automatically parent to the Revit Application. This is MS recommended method.

            ViewModels.App_ViewModel.Instance.CloseAction -= designOptionsViewManagerWindow.Close;
            //Clear all instances to null
            ViewModels.App_ViewModel.Instance.CloseAction = null;
            ViewModels.App_ViewModel.Instance.Dispose();
            DocumentUtils.RevitDocument = null;
            DocumentUtils.RevitUIDocument = null;
            ViewModels.App_ViewModel.Instance.DesignOptionSetInRevit.Clear();

            return Result.Succeeded;
        }
    }
}