using System;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Rosieks.VisualStudio.Durandal.Extensions;
using Rosieks.VisualStudio.Durandal.Helpers;

namespace Rosieks.VisualStudio.Durandal.Commands
{
    internal class ModuleGoToDefinition : CommandTargetBase<VSConstants.VSStd97CmdID>
    {
        private readonly DTE dte;
        private readonly ModulesIndex modules;

        public ModuleGoToDefinition(IVsTextView adapter, IWpfTextView textView, ModulesIndex modulesIndex, DTE dte) : base(adapter, textView, VSConstants.VSStd97CmdID.GotoDefn)
        {
            this.modules = modulesIndex;
            this.dte = dte;
        }

        protected override bool IsEnabled()
        {
            return this.dte.ActiveDocument.Name.EndsWith(".js");
        }

        protected override bool Execute(VSConstants.VSStd97CmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            string moduleId = RequireJSHelper.GetModule(this.TextView.Caret.Position.BufferPosition);
            string modulePath = this.modules.GetModulePath(moduleId);
            if (!string.IsNullOrEmpty(modulePath))
            {
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
