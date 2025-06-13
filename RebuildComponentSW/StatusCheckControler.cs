using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebuildComponentSW
{
    internal class StatusCheckControler
    {
        PanelTree ctrl;
        public StatusCheckControler(PanelTree panelTree) { 
            ctrl = panelTree;
        }
       
        public bool GetUpdatedData()
        {
            ctrl.DataAcquisitionProcess();
            Tree.RefreshFileFromPDM();
            Tree.CompareVersions();
            ctrl.userView.Clear();
            ctrl.userView = Tree.GetInfo();
            ctrl.LoadData();
            return true;
        }

    }
}
