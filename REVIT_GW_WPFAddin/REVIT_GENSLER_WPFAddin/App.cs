using System;
using Autodesk.Revit.UI;
using System.Reflection;

namespace REVIT_GW_WPFAddin
{
    internal class App : IExternalApplication
    {
        //Comment code below for Firmwide Integration
        public const string Caption = "Design Options Panel";

        public const string Caption2 = "Design Options Manage Button";
        private static string _path = Assembly.GetExecutingAssembly().Location;
        //Comment Upto here.

        public Result OnStartup(UIControlledApplication a)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //Comment code below for Firmwide Integration
            PushButtonData d = new PushButtonData(Caption2, Caption2, _path, "REVIT_GW_WPFAddin.Command");
            d.AvailabilityClassName = "REVIT_GW_WPFAddin.Models.OptionAvailability";
            RibbonPanel p = a.CreateRibbonPanel(Caption);
            PushButton b = p.AddItem(d) as PushButton;
            b.ToolTip = "Select Views to Duplicate for Design Options";
            //Comment Upto here.

            return Result.Succeeded;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("REVIT_GW_WPFAddin.System.Windows.Interactivity.dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}