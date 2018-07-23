using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Telerik.Windows.Controls;
using REVIT_GENSLER_WPFAddinTemplate1.Models;

namespace REVIT_GENSLER_WPFAddinTemplate1.ViewModels
{
    public class App_ViewModel : ViewModelBase
    {
        //Class Implementation. This is a singleton pattern to make one datacontext for the viewmodel.
        private static App_ViewModel _theInstance;

        public static App_ViewModel Instance
        {
            get
            {
                if (_theInstance == null)
                {
                    _theInstance = new App_ViewModel();
                }
                return _theInstance;
            }
        }

        public void ProcessData()
        {
            //Get all Design options set in Revit and populate the key object for databinding.
            foreach (ElementId fromRevitOptionSetId in DocumentUtils.FromRevit_OptionSetIds)
            {
                Data_OptionSet dataOptionSet = new Data_OptionSet(fromRevitOptionSetId);
                DesignOptionSetInRevit.Add(dataOptionSet);
            }

            //Initialize the list for Selected Design Options. This ties back to the TreeView items.

            SelectedDesignOptions = new List<Data_DesignOption>();
        }

        private ObservableCollection<Data_OptionSet> _designOptionSetInRevit = new ObservableCollection<Data_OptionSet>();

        public ObservableCollection<Data_OptionSet> DesignOptionSetInRevit
        {
            get { return _designOptionSetInRevit; }
            set { _designOptionSetInRevit = value; OnPropertyChanged("DesignOptionSetInRevit"); }
        }

        private ObservableCollection<Data_ProposedView> _proposedViewsList = new ObservableCollection<Data_ProposedView>();

        public ObservableCollection<Data_ProposedView> ProposedViewsList
        {
            get { return _proposedViewsList; }
            set { _proposedViewsList = value; OnPropertyChanged("ProposedViewsList"); }
        }

        public Action CloseAction { get; set; }

        private string _status_Message = "";

        public string Status_Message
        {
            get { return _status_Message; }
            set { _status_Message = value; OnPropertyChanged("Status_Message"); }
        }

        private List<Data_DesignOption> _selectedDesignOptions;

        public List<Data_DesignOption> SelectedDesignOptions
        {
            get { return _selectedDesignOptions; }
            set { _selectedDesignOptions = value; OnPropertyChanged("SelectedDesignOptions"); }
        }

        public void UpdateStatusMessage(string message)
        {
            Status_Message = message;
        }

        public void GetAllSelectedDesignOptions()
        {
            IEnumerable<Data_DesignOption> designOptionSetInRevit =
                from t in Instance.DesignOptionSetInRevit
                from x in t.DesignOptionsInSet
                select x;
            foreach (Data_DesignOption dataDesignOption in designOptionSetInRevit)
            {
                if (dataDesignOption.Selection)
                {
                    if (!SelectedDesignOptions.Exists(x => x.Id == dataDesignOption.Id))
                    {
                        SelectedDesignOptions.Add(dataDesignOption);
                    }
                }
                else if (SelectedDesignOptions.Exists(x => x.Id == dataDesignOption.Id))
                {
                    SelectedDesignOptions.Remove(dataDesignOption);
                }
            }
            UpdateStatusMessage(SelectedDesignOptions.Count + " Design Option Selected");
        }

        public void GetProposedViewNames()
        {
            ProposedViewsList = new ObservableCollection<Data_ProposedView>();
            foreach (View v in DocumentUtils.FromRevit_SelectedViews)
            {
                foreach (Data_DesignOption selectedDesignOption in SelectedDesignOptions)
                {
                    Data_ProposedView dataProposedView = new Data_ProposedView(v.Id, selectedDesignOption.SetId, selectedDesignOption.Id);
                    ProposedViewsList.Add(dataProposedView);
                }
            }
            CheckViewNamesForDuplicates();
        }

        public void CheckTreeViewOptionSet()
        {
            foreach (Data_OptionSet DOSInRevit in Instance.DesignOptionSetInRevit)
            {
                int int_NumOfDOInASet = DOSInRevit.DesignOptionsInSet.Count();
                if (DOSInRevit.Selection == true)
                {
                    for (int i = 0; i < int_NumOfDOInASet; i++)
                    {
                        DOSInRevit.DesignOptionsInSet[i].Selection = true;
                    }
                }
                else
                {
                    int int_NumOfCheckedDO = 0;
                    for (int i = 0; i < int_NumOfDOInASet; i++)
                    {
                        if (DOSInRevit.DesignOptionsInSet[i].Selection == true)
                        {
                            int_NumOfCheckedDO++;
                        }
                    }
                    if (int_NumOfCheckedDO != int_NumOfDOInASet)
                    {
                    }
                    else
                    {
                        for (int i = 0; i < int_NumOfDOInASet; i++)
                        {
                            DOSInRevit.DesignOptionsInSet[i].Selection = false;
                        }
                    }
                }
            }

            GetAllSelectedDesignOptions(); //update the SelectedDesignOptions
            GetProposedViewNames(); //update the ProposedViewNames
        }

        public void CheckTreeViewDesignOptions()
        {
            foreach (Data_OptionSet DOSInRevit in Instance.DesignOptionSetInRevit)
            {
                int? int_NumOfDOCheckedInASet = 0;
                int int_NumOfDOInASet = DOSInRevit.DesignOptionsInSet.Count();
                for (int i = 0; i < int_NumOfDOInASet; i++)
                {
                    if (DOSInRevit.DesignOptionsInSet[i].Selection == true)
                    {
                        int_NumOfDOCheckedInASet++;
                    }
                }
                if (int_NumOfDOInASet == int_NumOfDOCheckedInASet)
                {
                    DOSInRevit.Selection = true;
                }
                if (int_NumOfDOCheckedInASet < int_NumOfDOInASet)
                {
                    DOSInRevit.Selection = false;
                }
                if (int_NumOfDOCheckedInASet == 0)
                {
                    DOSInRevit.Selection = false;
                }
            }

            GetAllSelectedDesignOptions(); //update the SelectedDesignOptions
            GetProposedViewNames(); //update the ProposedViewNames
        }

        public bool CreateViews()
        {
            using (Transaction t = new Transaction(DocumentUtils.RevitDocument))
            {
                if (t.Start("Create duplicated views with design options") == TransactionStatus.Started)
                {
                    try
                    {
                        foreach (Data_ProposedView dpv in ProposedViewsList)
                        {
                            if (dpv.Selection && dpv.IsEnabled)
                            {
                                View v = DocumentUtils.RevitDocument.GetElement(dpv.ParentViewElementId) as View;
                                if ((v.ViewType == ViewType.Elevation ||
                                    v.ViewType == ViewType.FloorPlan ||
                                    v.ViewType == ViewType.ThreeD ||
                                    v.ViewType == ViewType.AreaPlan ||
                                    v.ViewType == ViewType.CeilingPlan ||
                                    v.ViewType == ViewType.Section) && v.CanBePrinted)
                                {
                                    if (v.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
                                    {
                                        View newView = DocumentUtils.RevitDocument.GetElement(v.Duplicate(ViewDuplicateOption.WithDetailing)) as View;
                                        //newView.Name = dpv.Name.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(":", "").Replace(";", "").Replace("<", "").Replace(">", "").Replace("\\", "").Replace("?", "");
                                        newView.Name = dpv.Name;
                                        newView.get_Parameter(BuiltInParameter.VIEWER_OPTION_VISIBILITY).Set(dpv.DesignOptionElementId);
                                    }
                                    //UpdateStatusMessage("Views duplicated and Design Option were set.");
                                }

                                if ((v.ViewType == ViewType.Schedule ||
                                    v.ViewType == ViewType.ColumnSchedule))
                                {
                                    if (v.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
                                    {
                                        View newView = DocumentUtils.RevitDocument.GetElement(v.Duplicate(ViewDuplicateOption.Duplicate)) as View;
                                        //newView.Name = dpv.Name.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(":", "").Replace(";", "").Replace("<", "").Replace(">", "").Replace("\\", "").Replace("?", "");
                                        newView.Name = dpv.Name;
                                        //newView.get_Parameter(BuiltInParameter.VIEWER_OPTION_VISIBILITY).Set(dpv.DesignOptionElementId); //Because Schedule don't have this parameter available through API.
                                    }
                                    //UpdateStatusMessage("Schedule duplicated but Design Option not set.");
                                }
                            }
                        }
                        return (TransactionStatus.Committed == t.Commit());
                    }
                    catch (Exception e)
                    {
                        TaskDialog taskDialog = new TaskDialog("Revit");
                        taskDialog.MainContent = "The proposed view name resulted in a duplicated view. Revit needs unique names for views.\n Please repeat and avoid duplicate names. \n" + e.Message;
                        TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok;
                        taskDialog.CommonButtons = buttons;

                        if (TaskDialogResult.Ok == taskDialog.Show())
                        {
                            t.RollBack();
                        }
                        return false;
                        //throw;
                    }
                }
            }
            return false;
        }

        private void CheckIfProposedViewListHasDuplicates()
        {
            var set = new HashSet<string>();

            foreach (var dpv in ProposedViewsList)
            {
                try
                {
                    if (!set.Add(dpv.Name.Trim()))
                    {
                        dpv.IsDuplicateName = true;
                        UpdateStatusMessage("Edit resulted in duplicate view names. Fix name to enable row item.");
                    }
                    else
                    {
                        dpv.IsDuplicateName = false;
                    }
                }
                catch (Exception e) { Debug.WriteLine(e.Message); }
            }
        }

        public void CheckViewNamesForDuplicates()
        {
            CheckIfProposedViewListHasDuplicates(); //Check if the list has duplicates and if so assign the property IsDuplicateName to true.

            foreach (var dpv in ProposedViewsList)
            {
                try
                {
                    if (dpv.Name.Trim() != "") //Check if the text box is blank
                    {
                        if (!dpv.IsDuplicateName)
                        {
                            if (NamingUtils.IsValidName(dpv.Name)) //Check for non printable characters in the proposed view names and replace any invalid characters.
                            {
                                if (DocumentUtils.FromRevit_AllViews.Any(vName => vName.Name == dpv.Name.Trim()))
                                {
                                    dpv.IsEnabled = false;
                                    UpdateStatusMessage("Duplicate view names detected. Fix name to enable row item.");
                                }
                                else
                                {
                                    dpv.IsEnabled = true;
                                }
                            }
                            else
                            {
                                dpv.IsEnabled = false;
                            }
                        }
                        else
                        {
                            dpv.IsEnabled = false;
                        }
                    }
                    else
                    {
                        dpv.IsEnabled = false;
                    }
                }
                catch (Exception e) { Debug.WriteLine(e.Message); }
            }
        }

        public void CheckUnCheckAllTreeViewItems(bool option)
        {
            IEnumerable<Data_DesignOption> DOS =
                from t in Instance.DesignOptionSetInRevit
                from x in t.DesignOptionsInSet
                select x;
            foreach (Data_DesignOption DO in DOS)
            {
                if (option)
                {
                    DO.Selection = true;
                }
                else
                {
                    DO.Selection = false;
                }
            }
            CheckTreeViewDesignOptions();
            CheckTreeViewOptionSet();
            GetProposedViewNames();
            UpdateStatusMessage(SelectedDesignOptions.Count + " Selection Updated!");
        }
    }
}