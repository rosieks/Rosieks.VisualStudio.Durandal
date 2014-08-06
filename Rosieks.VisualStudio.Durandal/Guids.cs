// Guids.cs
// MUST match guids.h
using System;
using System.Runtime.InteropServices;

namespace Rosieks.VisualStudio.Durandal
{
    static class GuidList
    {
        public const string guidDurandalPkgString = "e08ece08-8eac-471d-a56b-1775e9c53e78";
        public const string guidDurandalCmdSetString = "5dcb7129-b253-42af-b82b-559b696c1e5b";
        public const string guidDurandalCmdSetString2 = "76fa46d2-7692-45fb-a855-aacba6325c1d";

        public static readonly Guid guidDurandalCmdSet = new Guid(guidDurandalCmdSetString);
        public static readonly Guid guidDurandalCmdSet2 = new Guid(guidDurandalCmdSetString2);
    };

    [Guid(GuidList.guidDurandalCmdSetString)]
    enum CommandId
    {
        GoToView = 0x100,
        GoToViewModel = 0x101,
    }
}