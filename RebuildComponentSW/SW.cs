using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace RebuildComponentSW
{
    public  class SW
    {
        public event Action<int, MsgInfo> NotifySW;

        string TemplateName = "C:\\CUBY_PDM\\library\\templates\\Спецификация.sldbomtbt";
        private ModelDoc2 swMainModel;
        private AssemblyDoc swMainAssy;
        private Configuration swMainConfig;
        public SW(ModelDoc2 swDoc)
        {
            swMainModel = swDoc;
            swMainAssy = (AssemblyDoc)swMainModel;
           
        }
        private void ResolvedLigthWeiht(AssemblyDoc ass)
        {
            int countLigthWeiht = ass.GetLightWeightComponentCount();
            NotifyBeginOperation(countLigthWeiht, "Resolved assemble to LigthWeiht");
            if (countLigthWeiht > 0)
            {
                ass.ResolveAllLightweight();
            }
        }
        private void GetRootComponent()
        {
            try
            {
             string rootPath = swMainModel.GetPathName();
             string nameRoot = Path.GetFileNameWithoutExtension(rootPath);
             swMainConfig = (Configuration)swMainModel.GetActiveConfiguration();
            
             Tree.AddNode("0", nameRoot, rootPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
               
            }
            
        }
        private void GetBomTable()
        {
            IModelDocExtension Ext;
            BomFeature swBOMFeature;
            BomTableAnnotation swBOMAnnotation;
            string Configuration;
            TableAnnotation swTableAnn;
            ExtractomTable(out Ext, out swBOMFeature, out swBOMAnnotation, out Configuration, out swTableAnn);

            int nNumRow = 0;
            int J = 0;


            nNumRow = swTableAnn.RowCount;
            NotifyBeginOperation(nNumRow, "Reading TableBOM");
            for (J = 0; J <= nNumRow - 1; J++)
            {
                ExtractItem(swBOMAnnotation, Configuration, J);
            }


            string BomName = swBOMFeature.Name;
            bool boolstatus = TableBomClose(Ext, BomName);

        }

        public bool ReadTree()
        {
            
            ResolvedLigthWeiht(swMainAssy);
            GetRootComponent();
            GetBomTable();
            return true;
        }

        private void ExtractItem(BomTableAnnotation swBOMAnnotation, string Configuration, int J)
        {
            string ItemNumber;
            string PartNumber;
            string PathName;
            string e;
            string designation;
            string[] result = new string[2];
            swBOMAnnotation.GetComponentsCount2(J, Configuration, out ItemNumber, out PartNumber);

            if (PartNumber == null) return;
            string PartNumberTrim = PartNumber.Trim();
            if (PartNumberTrim == "") return;

            string[] str = (string[])swBOMAnnotation.GetModelPathNames(J, out ItemNumber, out PartNumber);

            PathName = str[0];
            designation = Path.GetFileNameWithoutExtension(PathName);
            string regCuby = @"^CUBY-\d{8}$";
            bool IsCUBY = Regex.IsMatch(PartNumberTrim, regCuby);

            if (!IsCUBY)
            {
                PartNumberTrim = designation;
            }

            e = Path.GetExtension(PathName);
            string AddextendedNumber = "0." + ItemNumber;
            if (e == ".SLDPRT" || e == ".sldprt" || e == ".SLDASM" || e == ".sldasm")
            {

                Tree.AddNode(AddextendedNumber, PartNumberTrim, PathName);
                NotifyStepOperation(PathName);
            }
        }

        private bool TableBomClose(IModelDocExtension Ext, string BomName)
        {
            bool boolstatus;
            string numberTable = BomName.Substring(17);
            boolstatus = Ext.SelectByID2("DetailItem" + numberTable + "@Annotations", "ANNOTATIONTABLES", 0, 0, 0, false, 0, null, 0);
            swMainModel.EditDelete();
            swMainModel.ClearSelection2(true);
            return boolstatus;
        }

        private void ExtractomTable(out IModelDocExtension Ext, out BomFeature swBOMFeature, out BomTableAnnotation swBOMAnnotation, out string Configuration, out TableAnnotation swTableAnn)
        {
            NotifyBeginOperation(0, "Obtaining specifications");
            Ext = default(IModelDocExtension);
            Ext = swMainModel.Extension;

            swBOMFeature = default(BomFeature);
            swBOMAnnotation = default(BomTableAnnotation);
            Configuration = swMainConfig.Name;
            int nbrType = (int)swNumberingType_e.swNumberingType_Detailed;
            int BomType = (int)swBomType_e.swBomType_Indented;
            try
            {
                swBOMAnnotation = Ext.InsertBomTable3(TemplateName, 0, 0, BomType, Configuration, true, nbrType, false);
                swBOMFeature = swBOMAnnotation.BomFeature;
                swTableAnn = (TableAnnotation)swBOMAnnotation;
               // NotifyBeginOperation(0, "Extraction TableBOM");
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void NotifyBeginOperation(int count, string nameOper)
        {
            y = 1;
            MsgInfo msgInfo = new MsgInfo();
            msgInfo.typeOperation = nameOper;
            msgInfo.countStep = count;
            NotifySW?.Invoke(2, msgInfo);

        }
        int y;
        private void NotifyStepOperation(string file)
        {
           
            MsgInfo msgInfo = new MsgInfo();
            string numberCuby = Path.GetFileName(file);
            msgInfo.numberCuby = numberCuby;
            msgInfo.countStep = y;
            NotifySW?.Invoke(3, msgInfo);
            y++;
        }
    }
}
