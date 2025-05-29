using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace RebuildComponentSW
{
    public class ActionControler
    {
        ISldWorks swApp;
        public ActionControler(ISldWorks app)            
        {
            swApp = app;
            PDM.NotifyPDM += PDM_NotifyPDM;
        }


        public void RebuildTree()
        {
             RebuildTreeLoopLevel();         
        }

        private void PDM_NotifyPDM(int stage, MsgInfo msg)
        {
            
        }


        private void RebuildTreeLoopLevel()
        {
            List<Part> listPart = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldprt" || d.Ext == ".SLDPRT")
                .ToList();

            List<Drawing> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .ToList();

            List<Part> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldasm" || d.Ext == ".SLDASM")
                .ToList();

            List<Drawing> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .ToList();

            List<Model> models = listAss.Cast<Model>().Concat(listAssDraw).ToList();
            var groupedModels = models.GroupBy(m => m.Level);

            int CountItemToCheckOut = listAssDraw.Count + listAss.Count + listPartDraw.Count + listPart.Count;



            PDM.CockSelList(CountItemToCheckOut);
            listPart.ForEach(d => d.AddItemToSelList());
            listPartDraw.ForEach(d => d.AddItemToSelList());
            listAss.ForEach(d => d.AddItemToSelList());
            listAssDraw.ForEach(d => d.AddItemToSelList());
            PDM.BatchGet();

            if (listPartDraw.Count > 0)
            {

                List<string> list = listPartDraw.Select(d => d.FullPath).ToList();
                loopFilesToRebuild(list);
                PDM.CockSelList(listPartDraw.Count + listPart.Count);
                listPart.ForEach(d => d.AddItemToSelList());
                listPartDraw.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }

            foreach (var group in groupedModels)
            {
                List<string> list = group.Select(d => d.FullPath).ToList();

                loopFilesToRebuild(list);
                PDM.CockSelList(list.Count);
                group.ToList().ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();

            }
        }

        private void loopFilesToRebuild(List<string> listFiles)
        {
            listFiles.ForEach(file => OpenFile(file));
        }

        private void OpenFile(string item)
        {
            ModelDoc2 swModelDoc = default(ModelDoc2);
            int errors = 0;
            int warnings = 0;
            
            IModelDocExtension extMod;
            string fileName = null;
            DrawingDoc swDraw = default(DrawingDoc);
            object[] vSheetName = null;
            string sheetName;
            int i = 0;
            bool bRet = false;
            int type = -1;

            try
            {

                fileName = item;
           
                string ext = Path.GetExtension(fileName);

                if (ext == ".sldprt" || ext == ".SLDPRT")
                {
                    type = (int)swDocumentTypes_e.swDocPART;
                }
                else if (ext == ".sldasm" || ext == ".SLDASM")
                {
                    type = (int)swDocumentTypes_e.swDocASSEMBLY;
                }
                else if (ext == ".slddrw" || ext == ".SLDDRW")
                {
                    type = (int)swDocumentTypes_e.swDocDRAWING;
                }

                swModelDoc = (ModelDoc2)swApp.OpenDoc6(fileName, type, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
                if (swModelDoc == null)
                {
                    MsgInfo msgInfo = new MsgInfo();
                    //msgInfo.errorMsg=errors.n
                    msgInfo.numberCuby = fileName;
                    RefreshFile(swModelDoc);
                    return;
                }
  

            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private void RefreshFile(ModelDoc2 swModelDoc)
        {
            int lErrors = 0;
            int lWarnings = 0;
            IModelDocExtension extMod;
            extMod = swModelDoc.Extension;
            // extMod.Rebuild((int)swRebuildOptions_e.swRebuildAll);
            extMod.ForceRebuildAll();
            swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
            swModelDoc.Close();
 
        }

    }
}
