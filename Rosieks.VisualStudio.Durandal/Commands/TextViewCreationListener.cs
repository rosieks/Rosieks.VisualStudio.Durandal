using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Rosieks.VisualStudio.Durandal.Commands
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class TextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            var modules = new ModulesIndex();

            textView.Properties.GetOrCreateSingletonProperty(() => new ModuleGoToDefinition(textViewAdapter, textView, modules, DurandalPackage.DTE));
            textView.Properties.GetOrCreateSingletonProperty(() => new WidgetGoToDefinition(textViewAdapter, textView, modules, DurandalPackage.DTE));
        }
    }
}
