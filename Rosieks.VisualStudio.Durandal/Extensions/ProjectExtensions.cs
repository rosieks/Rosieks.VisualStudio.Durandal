namespace Rosieks.VisualStudio.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;

    internal static class ProjectExtensions
    {
        public static Project GetActiveProject(this System.IServiceProvider serviceProvider)
        {
            DTE dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            return dte.ActiveDocument.ProjectItem.ContainingProject;
        }

        public static Solution GetSolution(this System.IServiceProvider serviceProvider)
        {
            DTE dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            return dte.Solution;
        }

        public static IEnumerable<Project> GetProjects(this Solution solution)
        {
            List<Project> projects = new List<Project>();
            for (int i = 1; i <= solution.Projects.Count; i++)
            {
                Project proj = solution.Projects.Item(i);
                if (proj.IsSolutionFolder())
                {
                    projects.AddRange(IterateSubProjects(proj));
                }
                else
                {
                    projects.Add(proj);
                }
            }

            return projects;
        }

        public static Project GetActiveProject(this DTE dte)
        {
            Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            if (activeProject == null)
            {
                activeProject = dte.ActiveDocument.ProjectItem.ContainingProject;
            }

            return activeProject;
        }

        /// <summary>
        ///     Checks if the kind GUID is that of solution folder
        /// </summary>
        /// <param name = "kind"></param>
        /// <returns></returns>
        public static bool IsSolutionFolder(this Project project)
        {
            return project.Kind.Equals("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Checks if the kind GUID is that of solution subfolder
        /// </summary>
        /// <param name = "kind"></param>
        /// <returns></returns>
        public static bool IsSolutionSubFolder(this ProjectItem project)
        {
            return project.Kind.Equals("{66A26722-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Checks if the kind GUID is that of solution folder
        /// </summary>
        /// <param name = "kind"></param>
        /// <returns></returns>
        public static bool IsProjectFolder(this ProjectItem projectItem)
        {
            return projectItem.Kind.Equals("{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}", StringComparison.OrdinalIgnoreCase);
        }

        public static CodeType GetCodeTypeFromFullName(this Project project, string typeName)
        {
            if (project.CodeModel != null)
            {
                return project.CodeModel.CodeTypeFromFullName(typeName.Split(',')[0]);
            }
            else
            {
                return null;
            }
        }

        public static CodeType GetCodeTypeFromFullName(this Solution solution, string typeName)
        {
            typeName = typeName.Split(',')[0];
            foreach (var cm in solution.GetProjects().Where(p => p.CodeModel != null).Select(s => s.CodeModel))
            {
                CodeType codeType = cm.CodeTypeFromFullName(typeName);
                try
                {
                    var projectItem = codeType.ProjectItem;
                    return codeType;
                }
                catch
                {
                }
            }

            return null;
        }

        private static IEnumerable<Project> IterateSubProjects(Project project)
        {
            List<Project> projects = new List<Project>();
            if (project.ProjectItems != null)
            {
                for (int i = 1; i <= project.ProjectItems.Count; i++)
                {
                    ProjectItem item = project.ProjectItems.Item(i);
                    if (item.IsSolutionSubFolder())
                    {
                        if (item.SubProject != null)
                        {
                            if (item.SubProject.IsSolutionFolder())
                            {
                                projects.AddRange(IterateSubProjects(item.SubProject));
                            }
                            else
                            {
                                projects.Add(item.SubProject);
                            }
                        }
                    }
                }
            }

            return projects;
        }
    }
}
