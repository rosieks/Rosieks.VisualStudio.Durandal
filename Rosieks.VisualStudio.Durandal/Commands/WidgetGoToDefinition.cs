namespace Rosieks.VisualStudio.Durandal.Commands
{
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Rosieks.VisualStudio.Durandal.Extensions;
    using Rosieks.VisualStudio.Durandal.Helpers;

    internal class WidgetGoToDefinition : CommandTargetBase<VSConstants.VSStd97CmdID>
    {
        private readonly DTE dte;
        private readonly ModulesIndex modules;

        public WidgetGoToDefinition(IVsTextView adapter, IWpfTextView textView, ModulesIndex modulesIndex, DTE dte) : base(adapter, textView, VSConstants.VSStd97CmdID.GotoDefn)
        {
            this.modules = modulesIndex;
            this.dte = dte;
        }

        protected override bool IsEnabled()
        {
            return this.dte.ActiveDocument.Name.EndsWith(".html");
        }

        protected override bool Execute(VSConstants.VSStd97CmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            string moduleId = "widgets/" + RequireJSHelper.GetModule(this.TextView.Caret.Position.BufferPosition) + "/viewmodel";
            string modulePath = this.modules.GetModulePath(moduleId);
            if (!string.IsNullOrEmpty(modulePath))
            {
                modulePath = modulePath.Replace("viewmodel", "view").Replace(".js", ".html");

                this.dte.OpenFileInPreviewTab(modulePath);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
