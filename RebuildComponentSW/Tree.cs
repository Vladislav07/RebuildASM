using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace RebuildComponentSW

{

    public static class Tree
    {

        static Dictionary<string, Part> ModelTree;
        public static List<Part> listComp;
        public static List<Drawing> listDraw;

        static Dictionary<string, string> structuralNumbers;
        internal static event Action<int,MsgInfo> NotifyTree;
        static Tree()
        {

            ModelTree = new Dictionary<string, Part>();
            listComp = new List<Part>();
            listDraw = new List<Drawing>();
            structuralNumbers = new Dictionary<string, string>();
        }
        public static void AddNode(string NodeNumber, string cubyNumber, string pathNode)
        {

            ModelTree.Add(NodeNumber, GetModelFromNumber(cubyNumber, pathNode));
            structuralNumbers.Add(NodeNumber, cubyNumber);
        }

        private static Part GetModelFromNumber(string numberCuby, string path)
        {
            Part comp = null;
            foreach (KeyValuePair<string, Part> item in ModelTree)
            {
                comp = item.Value;
                if (numberCuby == comp.CubyNumber) return comp;
            }
            string e = Path.GetExtension(path);
            if (e == ".SLDPRT" || e == ".sldprt")
            {
                comp = new Part(numberCuby, path);

            }
            else if (e == ".SLDASM" || e == ".sldasm")
            {
                comp = new Assemble(numberCuby, path);

            }
            else
            {
                return null;
            }
            comp.NotificationParent += Comp_NotificationParent;

            return comp;
        }

        private static void Comp_NotificationParent(string cubyNumber, StateModel isBlocedchild)
        {
            Assemble comp = (Assemble)listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return;
            comp.CascadingUpdate(isBlocedchild);

        }



        public static int Part_IsChild(string cubyNumber, int VersChild)
        {

            Model comp = listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return -1;
            if (comp.File.CurrentVersion != VersChild) return comp.File.CurrentVersion;
            return -1;
        }

        public static void SearchParentFromChild()
        {
            string StructureNumberChild;
            string ParentStructurenumber;
            int index = 0;
            char separate = new char[] { '.' }[0];
            Part child = null;
            string parentNumber;
            foreach (KeyValuePair<string, string> item in structuralNumbers)
            {

                child = ModelTree[item.Key];
                StructureNumberChild = item.Key;
                if (StructureNumberChild == "0") continue;
                if (child == null && child.CubyNumber != item.Value) continue;
                index = StructureNumberChild.LastIndexOf(separate);
                ParentStructurenumber = StructureNumberChild.Substring(0, index);
                if (!structuralNumbers.ContainsKey(ParentStructurenumber)) continue;
                parentNumber = structuralNumbers[ParentStructurenumber];
                if (child.listParent.Contains(parentNumber)) continue;
                child.listParent.Add(parentNumber);
            }
        }

        public static void FillCollection()
        {
            listComp.Clear();
            char s = new char[] { '.' }[0];
            int level_ = 0;
            var uniqueModelByGroup = ModelTree
            .GroupBy(pair => pair.Key.Count(o => o == s))
            .Select(group => group.Select(g => g.Value).Distinct().ToList()).ToList();

            foreach (var item in uniqueModelByGroup)
            {
                foreach (Part comp in item)
                {

                    comp.Level = level_;
                    if (listComp.Contains(comp)) continue;
                    listComp.Add(comp);
                }
                level_++;
            }
        }

        public static void GetInfoPDM()
        {
            int i = 1;
            InfoAboutProcessing("Extract from storage PDM", listComp.Count);
            foreach (Model comp in listComp)
            {
                comp.GetEdmFile();
                comp.IsDrawings();
                InfoDataProcessing(comp.CubyNumber, i);
                i++;
            }

        }

        public static void RefreshFileFromPDM()
        {
            int i = 1;
            List<Model> models = listComp.Cast<Model>().Concat(listDraw).ToList();
            InfoAboutProcessing("Refresh from storage PDM", models.Count);
   
            foreach (Model comp in models)
            {
                InfoDataProcessing(comp.CubyNumber, i);
                PDM.RefreshFile(comp);
                comp.condition = null;

                i++;
            }

        }

        public static void ReverseTree()
        {
            listComp.Reverse();
            listDraw.Reverse();
        }
        public static void CompareVersions()
        {
            listComp.ForEach(prt => prt.SetState());
            listDraw.ForEach(dr => dr.SetState());
            listComp.ForEach(prt => {
                if (prt.condition.stateModel == StateModel.Clean || prt.condition.stateModel == StateModel.Manufacturing) return;
                prt.NotificationState(prt.condition.stateModel);
            });
            listDraw.ForEach(dr => dr.CompareStateFromModel());

        }

        private static void InfoAboutProcessing(string nameOper, int countCycl)
        {
            MsgInfo mno = new MsgInfo();
            mno.typeOperation = nameOper;
            mno.countStep = countCycl;
            NotifyTree.Invoke(2,mno);
        }
        private static void InfoDataProcessing(string nameCuby, int i)
        {

            MsgInfo mdata = new MsgInfo();
            mdata.numberCuby = nameCuby;
            mdata.currentStep = i;
            NotifyTree.Invoke(3,mdata);
        }
        public static List<ViewUser> JoinCompAndDraw()
        {
            List<Part> compList = Tree.listComp;
            List<Drawing> drawList = Tree.listDraw;
            List<ViewUser> lv = new List<ViewUser>();
            foreach (Part item in compList)
            {
                Drawing dr = drawList.FirstOrDefault(d => d.CubyNumber == item.CubyNumber);

                lv.Add(new ViewUser
                {
                    NameComp = item.CubyNumber,
                    TypeComp = item.Section,
                    Ext = item.Ext,
                    Level = item.Level.ToString(),
                    StPDM = item.File.CurrentState.Name.ToString(),
                    State = item.condition.stateModel.ToString(),
                    VersionModel = item.File?.CurrentVersion.ToString() ?? "",
                    IsLocked = item.File?.IsLocked.ToString() ?? "",
                    IsChildRefError = item is Assemble ? (item as Assemble).listRefChildError.Count.ToString() : "",

                    DrawState = dr != null ? dr.condition.stateModel.ToString() : "",
                    StDrPDM = dr != null ? dr.File.CurrentState.Name : "",

                    DrawNeedRebuild = dr != null ? dr.NeedsRebuild.ToString() : "",
                    DrawVersRev = dr != null ? dr.msgRefVers : "",

                    DrawIsLocked = dr != null ? dr.File.IsLocked.ToString() : ""

                });
            }


            return lv;
        }

    }
}