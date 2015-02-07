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
            var modules = new List<string>();
            var dte = DurandalPackage.DTE;
            foreach (var item in dte.GetActiveProject().ProjectItems.Cast<ProjectItem>())
            {
                if (item.IsProjectFolder())
                {
                    if (item.Name.Equals("app", System.StringComparison.OrdinalIgnoreCase))
                    {
                        modules.AddRange(LookForModules(item, Enumerable.Empty<string>()));
                    }
                    else if (item.Name.Equals("scripts", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var durandal = item.ProjectItems.Cast<ProjectItem>().FirstOrDefault(x => x.IsProjectFolder() && string.Equals(x.Name, "durandal", System.StringComparison.OrdinalIgnoreCase));
                        if (durandal != null)
                        {
                            var plugins = durandal.ProjectItems.Cast<ProjectItem>().FirstOrDefault(x => x.IsProjectFolder() && string.Equals(x.Name, "plugins", System.StringComparison.OrdinalIgnoreCase));
                            modules.AddRange(LookForModules(plugins, new[] { "plugins" }));
                        }

                        modules.AddRange(LookForModules(durandal, new[] { "durandal" }));
                    }
                }
            }

            return modules;
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
