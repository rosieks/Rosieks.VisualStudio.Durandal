using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Rosieks.VisualStudio.Extensions;

namespace Rosieks.VisualStudio.Durandal
{
    public class ModulesIndex
    {
        private IList<string> modules;
        private IDictionary<string, string> paths = new Dictionary<string, string>();

        public IEnumerable<string> GetModules()
        {
            if (this.modules == null)
            {
                this.modules = this.InitModules().ToArray();
            }

            return this.modules;
        }

        private IEnumerable<string> InitModules()
        {
            var dte = DurandalPackage.DTE;
            foreach (var item in dte.GetActiveProject().ProjectItems.Cast<ProjectItem>())
            {
                if (item.IsProjectFolder() && item.Name.Equals("app", System.StringComparison.OrdinalIgnoreCase))
                {
                    return LookForModules(item, Enumerable.Empty<string>());
                }
            }

            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> LookForModules(ProjectItem parent, IEnumerable<string> names)
        {
            foreach (var item in  parent.ProjectItems.Cast<ProjectItem>())
            {
                if (item.IsProjectFolder())
                {
                    foreach (var subItem in LookForModules(item, names.Concat(new[] { item.Name } )))
                    {
                        yield return subItem;
                    }
                }
                else
                {
                    if (item.Name.EndsWith(".js"))
                    {
                        string name = string.Join("/", names.Concat(new[] { item.Name.Substring(0, item.Name.Length - 3) }));
                        this.paths.Add(name, item.FileNames[0]);
                        yield return name;
                    }
                }
            }
        }

        public string GetModulePath(string moduleId)
        {
            if (this.modules == null)
            {
                this.modules = InitModules().ToArray();
            }

            string path;
            this.paths.TryGetValue(moduleId, out path);
            return path;
        }
    }
}
