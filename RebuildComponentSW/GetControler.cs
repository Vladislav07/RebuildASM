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
        }

        public bool GetData()
        {
            sw = new SW(model);
            sw.ReadTree();
            Tree.SearchParentFromChild();
            Tree.FillCollection();
            Tree.ReverseTree();
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            ctrl.userView = Tree.JoinCompAndDraw();
            ctrl.LoadData();
            return true;
        }

    }
}
