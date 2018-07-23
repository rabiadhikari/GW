using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace REVIT_GW_WPFAddin
{
    public class DocumentUtils//Utility to get information from Revit
    {
        #region Utilites - Objects & Properties

        #region Data Object :: Revit UI Document

        //Revit UIDocument Property
        private static UIDocument _revitUIDocument;

        public static UIDocument RevitUIDocument
        {
            get { return _revitUIDocument; }
            set { _revitUIDocument = value; }
        }

        #endregion Data Object :: Revit UI Document

        #region Data Object :: Revit Document

        //Revit Document Property
        private static Document _revitDocument;

        public static Document RevitDocument
        {
            get { return _revitDocument; }
            set { _revitDocument = value; }
        }

        #endregion Data Object :: Revit Document

        #region Data Object :: From Revit Option Set Ids

        //OptionSet IDs Property
        private static ObservableCollection<ElementId> _fromRevitOptionSetIds;

        public static ObservableCollection<ElementId> FromRevit_OptionSetIds
        {
            get { return _fromRevitOptionSetIds; }
            set { _fromRevitOptionSetIds = value; }
        }

        #endregion Data Object :: From Revit Option Set Ids

        #region Data Object :: From Revit List of Design Options

        //Design Options Property
        private static List<DesignOption> _fromRevitListOfDesignOptions;

        public static List<DesignOption> FromRevit_ListOfDesignOptions
        {
            get { return _fromRevitListOfDesignOptions; }
            set { _fromRevitListOfDesignOptions = value; }
        }

        #endregion Data Object :: From Revit List of Design Options

        #region Data Object :: From Revit Selected Views

        //Selected Views Property
        private static List<View> _fromRevitSelectedViews = new List<View>();

        public static List<View> FromRevit_SelectedViews
        {
            get { return _fromRevitSelectedViews; }
            set { _fromRevitSelectedViews = value; }
        }

        #endregion Data Object :: From Revit Selected Views

        #region Data Object :: From Revit All Views

        //Get all Views in Revit
        private static List<View> _fromRevitAllViews;

        public static List<View> FromRevit_AllViews
        {
            get { return _fromRevitAllViews; }
            set { _fromRevitAllViews = value; }
        }

        #endregion Data Object :: From Revit All Views

        #endregion Utilites - Objects & Properties

        public static void LoadRevitDocumentAndProcess(UIDocument uidoc)
        {
            RevitUIDocument = uidoc;
            RevitDocument = uidoc.Document;

            //Until I find a better way to do the selection check for valid command operation use the following
            //Check if the views allow for design option. Present the window if not dismiss after informing the user about the selection
        }

        public static void GetAllDesignOptionsFromRevit(Document doc)
        {
            FromRevit_ListOfDesignOptions = new FilteredElementCollector(doc).OfClass(typeof(DesignOption)).Cast<DesignOption>().ToList();
        }

        public static void GetAllOptionSetIdsFromRevit()
        {
            FromRevit_OptionSetIds = new ObservableCollection<ElementId>();
            FromRevit_OptionSetIds.Clear();
            try
            {
                foreach (DesignOption dopt in FromRevit_ListOfDesignOptions)
                {
                    ElementId osId = dopt.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId();

                    if (!FromRevit_OptionSetIds.Contains(osId))
                        FromRevit_OptionSetIds.Add(osId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public static void GetAllSelectedViewsFromRevit(Document doc)
        {
            FromRevit_SelectedViews = new List<View>();
            Selection selSet = RevitUIDocument.Selection;
            List<ElementId> elementIds = selSet.GetElementIds().ToList();

            if (elementIds.Count == 0)
            {
                FromRevit_SelectedViews.Add(RevitUIDocument.ActiveGraphicalView);
                ViewModels.App_ViewModel.Instance.UpdateStatusMessage("Only active view is selected.");
            }
            else
            {
                FromRevit_SelectedViews = new FilteredElementCollector(doc, elementIds).OfClass(typeof(View)).Cast<View>().ToList();
                int count = FromRevit_SelectedViews.Count;
                ViewModels.App_ViewModel.Instance.UpdateStatusMessage(string.Concat(count.ToString(), " plan views were selected."));
            }
            selSet.SetElementIds(new List<ElementId>());
        }

        public static void GetAllViewsFromRevit(Document doc)
        {
            FromRevit_AllViews = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().ToList();
        }
    }
}