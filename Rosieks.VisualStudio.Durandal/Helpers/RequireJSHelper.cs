using Microsoft.VisualStudio.Text;

namespace Rosieks.VisualStudio.Durandal.Helpers
{
    public static class RequireJSHelper
    {
        public static string GetModule(SnapshotPoint caret)
        {
            var text = caret.Snapshot.GetText();
            int startIndex = -1;
            char stringChar = default(char);
            for (int i = caret.Position - 1; i >= 0; i--)
            {
                if (text[i] == '"' || text[i] == '\'')
                {
                    startIndex = i;
                    stringChar = text[i];
                    break;
                }
            }

            if (startIndex >= 0)
            {
                var endIndex = text.IndexOf(stringChar, caret.Position);
                var moduleId = text.Substring(startIndex + 1, endIndex - startIndex - 1);

                return moduleId;
            }
            else
            {
                return null;
            }
        }
    }
}
