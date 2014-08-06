namespace Rosieks.VisualStudio.Durandal.Commands
{
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Rosieks.VisualStudio.Durandal.Extensions;

    internal class GoToViewModelCmd
    {
        private DTE dte;

        public GoToViewModelCmd(OleMenuCommandService mcs, DTE dte)
        {
            OleMenuCommand oleMenuCommand = new OleMenuCommand(
                new EventHandler(this.GoToViewModel),
                null,
                new EventHandler(this.BeforeQueryStatus),
                new CommandID(GuidList.guidDurandalCmdSet2, (int)CommandId.GoToViewModel))
            {
                Enabled = false,
                Visible = false,
                Supported = true,
            };

            mcs.AddCommand(oleMenuCommand);
            this.dte = dte;
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            bool canGoToView = this.CanGoToView();
            menuCommand.Enabled = canGoToView;
            menuCommand.Visible = canGoToView;
        }

        private bool CanGoToView()
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            uint projectItemId;
            IVsMultiItemSelect mis;
            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
            if (hierarchy != null)
            {
                object value;
                hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_Name, out value);
                return value != null && value.ToString().EndsWith(".html");
            }
            else
            {
                return false;
            }
        }

        private void GoToViewModel(object sender, EventArgs e)
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            uint projectItemId;
            IVsMultiItemSelect mis;
            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
            if (hierarchy != null)
            {
                object value;
                hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtSelectedItem, out value);
                string path;
                hierarchy.GetCanonicalName(projectItemId, out path);
                string viewPath = path.Replace("view", "viewmodel").Replace(".html", ".js");
                this.dte.OpenFileInPreviewTab(viewPath);
            }
        }
    }
}
