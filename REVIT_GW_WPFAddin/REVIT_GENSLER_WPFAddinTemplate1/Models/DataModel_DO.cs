using Autodesk.Revit.DB;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.Windows.Controls;

namespace REVIT_GENSLER_WPFAddinTemplate1.Models
{
    #region Data Model - Data_OptionSet

    public class Data_OptionSet : ViewModelBase
    {
        //Default Implementation
        public Data_OptionSet()
        {
        }

        #region Fields & Properties

        //Private Fields
        private string _name;

        private ElementId _id;
        private bool _selectionMode;
        private ObservableCollection<Data_DesignOption> _designOptionsInSet;
        private Data_DesignOption tempDesignOption;

        //Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public ElementId Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public bool Selection
        {
            get { return _selectionMode; }
            set { _selectionMode = value; OnPropertyChanged("Selection"); }
        }

        public ObservableCollection<Data_DesignOption> DesignOptionsInSet
        {
            get { return _designOptionsInSet; }
            set { _designOptionsInSet = value; OnPropertyChanged("DesignOptionsInSet"); }
        }

        #endregion Fields & Properties

        #region Implementation Option Set ID

        //Implementation to process Option Set Id from Revit
        public Data_OptionSet(ElementId optSetId)
        {
            Name = DocumentUtils.RevitDocument.GetElement(optSetId).Name.ToString();  //Since Option Set is an ID name is extracted from Revit Document
            Id = optSetId; //assign the element ID to become a part of the data model
            Selection = true; //Make the selection unchecked by default
            DesignOptionsInSet = new ObservableCollection<Data_DesignOption>(); // All the design options that matches the design option set ID.
            //create a new observable collection while iterating through the design options and assign if set ID matches
            try
            {
                foreach (DesignOption dopt in DocumentUtils.FromRevit_ListOfDesignOptions)  //iterate all of design option in the static class
                {
                    ElementId osID = dopt.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId(); //get the element ID of design option set to check the pre populated static option set collection

                    if (osID == optSetId) //check if the design option id matches
                    {
                        tempDesignOption = new Data_DesignOption(dopt); //create a temp data_design option object to store additional information
                        DesignOptionsInSet.Add(tempDesignOption); //Add data_design option into the data_option set.
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        #endregion Implementation Option Set ID
    }

    #endregion Data Model - Data_OptionSet

    #region Data Model - Data_DesignOption

    public class Data_DesignOption : ViewModelBase
    {
        //Default Implementation
        public Data_DesignOption()
        {
        }

        #region Fields & Properties

        //Private Fields
        private string _name;

        private ElementId _id;
        private ElementId _SetId;
        private bool _selectionMode;

        //Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public ElementId Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public ElementId SetId
        {
            get { return _SetId; }
            set { _SetId = value; OnPropertyChanged("SetId"); }
        }

        public bool Selection
        {
            get { return _selectionMode; }
            set { _selectionMode = value; OnPropertyChanged("Selection"); }
        }

        #endregion Fields & Properties

        #region Implementation Design Options

        //Implemtation to process Design Options in Revit
        public Data_DesignOption(DesignOption designOption)
        {
            Id = designOption.Id; //assign the element ID to become a part of the data model
            Name = designOption.Name.ToString(); // Since Design Option is an object it has Name properties
            SetId = designOption.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId();
            Selection = true; //Make the selection
        }

        #endregion Implementation Design Options
    }

    #endregion Data Model - Data_DesignOption

    #region Data Model - Data_ProposedView

    public class Data_ProposedView : ViewModelBase
    {
        //Default Implementation
        public Data_ProposedView()
        {
        }

        #region Fields & Properties

        //Private Fields
        private string _name;

        private bool _selection;
        private bool _isEnabled;
        private bool _isDuplicateName;
        private ElementId _designOptionElementId;
        private ElementId _designOptionSetId;
        private ElementId _parentViewElementId;

        //Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); ViewModels.App_ViewModel.Instance.CheckViewNamesForDuplicates(); }
        }

        public bool Selection
        {
            get { return _selection; }
            set { _selection = value; OnPropertyChanged("Selection"); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        public bool IsDuplicateName
        {
            get { return _isDuplicateName; }
            set { _isDuplicateName = value; OnPropertyChanged("IsDuplicateName"); }
        }

        public ElementId DesignOptionElementId
        {
            get
            {
                return this._designOptionElementId;
            }
            set
            {
                this._designOptionElementId = value;
                this.OnPropertyChanged("DesignOptionElementId");
            }
        }

        public ElementId DesignOptionSetId
        {
            get { return _designOptionSetId; }
            set { _designOptionSetId = value; OnPropertyChanged("DesignOptionSetId"); }
        }

        public ElementId ParentViewElementId
        {
            get
            {
                return this._parentViewElementId;
            }
            set
            {
                this._parentViewElementId = value;
                this.OnPropertyChanged("ParentViewElementId");
            }
        }

        #endregion Fields & Properties

        #region Implementation Data Views

        public Data_ProposedView(ElementId parentViewId, ElementId parentViewOptionSetId, ElementId doptId)
        {
            try
            {
                Name = DocumentUtils.RevitDocument.GetElement(parentViewId).Name.Replace("{", "").Replace("}", "");
                if (CheckCaseMethods.CaseIsUpper(Name))
                {
                    Name = string.Concat(Name, " - ",
                        DocumentUtils.RevitDocument.GetElement(parentViewOptionSetId).Name.ToUpper(), " - ",
                        DocumentUtils.RevitDocument.GetElement(doptId).Name.Replace("(primary)", "").ToUpper());
                }
                else
                {
                    Name = string.Concat(DocumentUtils.RevitDocument.GetElement(parentViewId).Name.Replace("{", "").Replace("}", ""), " - ",
                                              DocumentUtils.RevitDocument.GetElement(parentViewOptionSetId).Name, " - ",
                                              DocumentUtils.RevitDocument.GetElement(doptId).Name.Replace("(primary)", ""));
                }

                Selection = true;
                IsEnabled = true;
                DesignOptionElementId = doptId;
                ParentViewElementId = parentViewId;
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
        }

        #endregion Implementation Data Views
    }

    #endregion Data Model - Data_ProposedView
}