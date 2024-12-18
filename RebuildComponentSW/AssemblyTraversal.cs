using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.IO;
using System.Text.RegularExpressions;


namespace RebuildComponentSW
{
   public  class AssemblyTraversal
    {
        
        ModelDoc2 swModel;
       
        public AssemblyTraversal(ModelDoc2 m)
        {
            swModel = m;
        }

        public void Main(ref List<FileRef> list)
        {
          
            Configuration swConf;
            Component2 swRootComp;
            FileRef rootComp;
            string rootName;
            string nameCuby; 
            swConf = (Configuration)swModel.GetActiveConfiguration();
            swRootComp = (Component2)swConf.GetRootComponent();
            rootName = swModel.GetPathName();
            nameCuby = Path.GetFileNameWithoutExtension(rootName);
            rootComp = new FileRef(nameCuby, rootName);
            rootComp.Description = ".SLDASM";
            TraverseComponent(swRootComp, 1, ref rootComp);
            ListFormationToRebuild(rootComp, ref list);

           // swApp.CloseAllDocuments(true);    
           // OpenAndRefresh();
            // pdm.DrawingsBatchUnLock();

        }

        public void TraverseComponent(Component2 swComp, int nLevel, ref FileRef fr)
        {
            object[] ChildComps;
            Component2 swChildComp;
            FileRef childFileRef;
            string childName;
            string e;
            string designation;

            ChildComps = (object[])swComp.GetChildren();

            for (int i = 0; i < ChildComps.Length; i++)
            {
                swChildComp = (Component2)ChildComps[i];
                if (!swChildComp.IsSuppressed()) continue;
                childName = swChildComp.GetPathName();
                e = Path.GetExtension(childName);
                designation = Path.GetFileNameWithoutExtension(childName);
                string regCuby = @"^CUBY-\d{8}$";
                bool IsCUBY = Regex.IsMatch(designation, regCuby);
                if (!IsCUBY) continue;

                childFileRef = new FileRef(designation, childName);
                childFileRef.Description = e;
                childFileRef.ComponentLevel = nLevel.ToString();
                fr.FileRefs.Add(childFileRef);
                TraverseComponent(swChildComp, nLevel + 1, ref childFileRef);
               
                // Debug.Print(sPadStr + swChildComp.Name2 + " <" + swChildComp.ReferencedConfiguration + ">");
            }
        }
        /*
        public void OpenAndRefresh()
        {
            ModelDoc2 swModelDoc = default(ModelDoc2);
            int errors = 0;
            int warnings = 0;
            int lErrors = 0;
            int lWarnings = 0;
            string fileName = null;
            // list.Sort();
            try
            {
                foreach (FileRef item in list)
                {
                    fileName = item.PathName;
                    if (item.Description == ".SLDASM" || item.Description == ".sldasm")
                    {
                        swModelDoc = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
                    }
                    else
                    {
                        swModelDoc = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
                    }

                    IModelDocExtension extMod = swModelDoc.Extension;

                    extMod.Rebuild((int)swRebuildOptions_e.swForceRebuildAll);
                    swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
                    swApp.CloseDoc(fileName);
                    swModelDoc = null;

                }
            }
            catch (System.Exception)
            {
                Debug.Print(errors.ToString());

            }
        }*/
        void ListFormationToRebuild(FileRef fr,ref List<FileRef>list)
        {
            if (fr.FileRefs == null) return;
            foreach (FileRef item in fr.FileRefs)
            {
                if (item.FileRefs.Count > 0) ListFormationToRebuild(item, ref list);
                if (list.Find(a => a.FileName == item.FileName) != null) continue;
                list.Add(item);
                // PrintTree(item);
            }
            list.Add(fr);
        }

        void PrintTree(FileRef fr)
        {
            Debug.Print(fr.FileName + '\n' + fr.ComponentLevel + '\n' + fr.PathName);
        }


        // The SldWorks swApp variable is pre-assigned for you.
        public SldWorks swApp;
    }
}
