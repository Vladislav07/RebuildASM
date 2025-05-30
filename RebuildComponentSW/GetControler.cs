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
            ctrl.DataAcquisitionProcess();
            sw.ReadTree();
            Tree.SearchParentFromChild();
            Tree.FillCollection();
            Tree.ReverseTree();
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            ctrl.userView = GetInfo();
            ctrl.LoadData();
            return true;
        }

        private List<InfoView> GetInfo()
        {
            List<InfoView> infoViews = new List<InfoView>();
            List<Model> models = new List<Model>();
            models.AddRange(Tree.listComp);
            models.AddRange(Tree.listDraw);
            foreach (Model item in models) {
                infoViews.Add(new InfoView
                {
                    NameComp = item.CubyNumber,
                    TypeComp = item.Section,
                    Ext = item.Ext,
                    Level = item.Level.ToString(),
                    StPDM = item.File.CurrentState.Name.ToString(),
                    State = item.condition.stateModel.ToString(),
                    IsLocked = item.File.IsLocked.ToString(),

                });
            }
            return infoViews;

        }

    }
}
