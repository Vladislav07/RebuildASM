using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace RebuildComponentSW
{
   
    public class GetControler:IDisposable
    {
        private PanelTree ctrl;
        private SW sw;
        ModelDoc2 model;
        public GetControler(PanelTree panelTree, ModelDoc2 _model)
        {
            
            model = _model;
            ctrl = panelTree;
            sw = new SW(model);
           // ctrl.DataAcquisitionProcess();
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
            try
            {
                sw.ReadTree();
                Tree.SearchParentFromChild();
                Tree.FillCollection();
                Tree.ReverseTree();
                Tree.GetInfoPDM();
                Tree.CompareVersions();
                ctrl.userView = Tree.GetInfo();
                ctrl.LoadData();
                ctrl.Refresh();
                return true;
            }
            catch (Exception)
            {

               return false;
            }
         
        }

        public void Dispose()
        {
            sw = null;
        }
    }
}
