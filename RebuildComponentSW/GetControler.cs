using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace RebuildComponentSW
{
   
    public class GetControler
    {
        private PanelTree ctrl;
        private SW sw;
        ModelDoc2 model;
        public GetControler(PanelTree panelTree, ModelDoc2 _model)
        {

            ctrl = panelTree;
            ctrl.DataAcquisitionProcess();
           
            model = _model;
           
            sw = new SW(model);
            sw.NotifySW += Sw_NotifySW;
            PDM.NotifyPDM += PDM_NotifyPDM;
            Tree.NotifyTree += Tree_NotifyTree;
        }

        private void Tree_NotifyTree(int arg1, MsgInfo arg2)
        {
            ctrl.Notifacation(arg1, arg2);
        }

        private void Sw_NotifySW(int arg1, MsgInfo arg2)
        {
            ctrl.Notifacation(arg1, arg2);
        }

        private void PDM_NotifyPDM(int arg1, MsgInfo arg2)
        {
            ctrl.Notifacation(arg1, arg2);
        }

        public bool GetData()
        {
            if (model == null)
            {
                ctrl.Notifacation(0, new MsgInfo("Could not acquire an active document"));
                return false;
            }
            sw.ReadTree();
            Tree.SearchParentFromChild();
            Tree.FillCollection();
            Tree.ReverseTree();
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            ctrl.userView = Tree.GetInfo();
            ctrl.LoadData();
            return true;
        }
      


    }
}
