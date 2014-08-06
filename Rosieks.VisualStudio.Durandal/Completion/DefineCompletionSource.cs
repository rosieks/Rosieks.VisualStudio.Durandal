using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Intel = Microsoft.VisualStudio.Language.Intellisense;

namespace Rosieks.VisualStudio.Durandal.Completion
{
    class DefineCompletionSource : StringCompletionSource
    {
        private readonly ImageSource moduleIcon;
        private readonly ImageSource namespaceIcon;
        private readonly ModulesIndex index;

        public DefineCompletionSource(IGlyphService glyphService, ModulesIndex index)
        {
            this.index = index;
            this.moduleIcon = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupClass, StandardGlyphItem.GlyphItemPublic);
            this.namespaceIcon = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
        }

        public override IEnumerable<Intel.Completion> GetEntries(char quote, SnapshotPoint caret)
        {
            var text = caret.Snapshot.GetText();
            var startIndex = Math.Max(text.LastIndexOf('"', caret.Position - 1), text.LastIndexOf('\'', caret.Position - 1));
            var moduleId = text.Substring(startIndex + 1, caret.Position - startIndex - 1);
            var x = moduleId.LastIndexOf("/") + 1;
            var prefix = moduleId.Substring(0, x);

            var modules = this.index.GetModules().Where(m => m.StartsWith(prefix)).Select(m =>
            {
                bool isNamespace = true;
                int nextStop = m.IndexOf('/', x);
                if (nextStop < 0)
                {
                    nextStop = m.Length;
                    isNamespace = false;
                }

                m = m.Substring(x, nextStop - x);
                return new { Name = m, IsNamespace = isNamespace };
            }).Distinct().ToArray();

            return modules.Select(m =>
            {
                return new Intel.Completion(
                m.Name,
                m.Name + (m.IsNamespace ? "/" : string.Empty),
                m.Name,
                m.IsNamespace ? this.namespaceIcon :  this.moduleIcon,
                null);
            });
        }

        public override Span? GetInvocationSpan(string text, int linePosition, SnapshotPoint position)
        {
            int startIndex = text.LastIndexOf("define(", linePosition, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0)
            {
                return null;
            }

            startIndex = Math.Max(Math.Max(text.LastIndexOf('"', linePosition - 1), text.LastIndexOf('\'', linePosition - 1)), text.LastIndexOf('/', linePosition - 1));
            if (startIndex < 0)
            {
                return null;
            }

            var endIndex = startIndex + 1 + text.Skip(startIndex + 1).TakeWhile(c => c != '\'' && c != '"').Count();
            if (endIndex < text.Length && (text[endIndex] == text[startIndex] || text[startIndex] == '/'))
            {
                endIndex++;
            }

            return Span.FromBounds(startIndex + 1, endIndex - 1);
        }
    }
}
