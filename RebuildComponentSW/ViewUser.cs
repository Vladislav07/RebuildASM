using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebuildComponentSW
{
    public struct ViewUser
    {
        internal string NameComp { get; set; }
        internal string TypeComp { get; set; }
        internal string Ext { get; set; }
        internal string Level { get; set; }
        internal string State { get; set; }
        internal string StPDM { get; set; }
        internal string VersionModel { get; set; }
        internal string IsLocked { get; set; }
        internal string IsChildRefError { get; set; }
        internal string DrawState { get; set; }
        internal string StDrPDM { get; set; }
        internal string DrawVersRev { get; set; }
        internal string DrawNeedRebuild { get; set; }
        internal string DrawIsLocked { get; set; }
    }
}
