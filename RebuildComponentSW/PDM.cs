using EPDM.Interop.epdm;
using RebuildComponentSW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RebuildComponentSW
{

    public static class PDM
    {
        static string NAME_PDM = "CUBY_PDM";


        static IEdmVault5 vault1 = null;
        static IEdmVault7 vault = null;
        static IEdmBatchGet batchGetter;
        static EdmSelItem[] ppoSelection = null;
        static IEdmBatchUnlock2 batchUnlocker;
        static IEdmEnumeratorVariable5 enumVar = default(IEdmEnumeratorVariable5);

        public static event Action<int, MsgInfo> NotifyPDM;
        private static int i;



        static PDM()
        {
            vault1 = new EdmVault5();
            ConnectingPDM();
        }
        public static void NotifyOperation(int stage, MsgInfo msgInfo)
        {
            NotifyPDM?.Invoke(stage, msgInfo);
        }

        public static void GetEdmFile(this Model item)
        {

            IEdmFile7 File = null;
            IEdmFolder5 ParentFolder = null;
            try
            {
                File = (IEdmFile7)vault1.GetFileFromPath(item.FullPath, out ParentFolder);
                item.File = File;
                item.bFolder = ParentFolder.ID;
                item.bFile = File.ID;

                enumVar = File.GetEnumeratorVariable();
                object val = null;
                EdmStrLst5 listConf = File.GetConfigurations(0);

                bool error = enumVar.GetVar("Раздел", ".", out val);
                if (val != null)
                {
                    item.Section = (string)val;
                }
                else
                {
                    error = enumVar.GetVar("Раздел", "@", out val);
                    item.Section = val != null ? (string)val : "none";
                }
            }
            catch (Exception)
            {

            }
        }

        internal static void GetReferenceFromAssemble(this Assemble ass)
        {
            ass.listRefChild.Clear();
            IEdmReference5 ref5 = ass.File.GetReferenceTree(ass.bFolder);
            IEdmReference10 ref10 = (IEdmReference10)ref5;
            IEdmPos5 pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
            string cubyNumber = null;
            int verChildRef = -1;
            try
            {
                while (!pos.IsNull)
                {
                    IEdmReference10 @ref = (IEdmReference10)ref5.GetNextChild(pos);
                    string extension = Path.GetExtension(@ref.Name);
                    if (extension == ".sldasm" || extension == ".sldprt" || extension == ".SLDASM" || extension == ".SLDPRT")
                    {
                        cubyNumber = Path.GetFileNameWithoutExtension(@ref.Name);

                        string regCuby = @"^CUBY-\d{8}$";
                        bool IsCUBY = Regex.IsMatch(cubyNumber.Trim(), regCuby);
                        // if (!IsCUBY) continue;

                        verChildRef = @ref.VersionRef;
                        if (ass.listRefChild.ContainsKey(cubyNumber.Trim())) continue;
                        ass.listRefChild.Add(cubyNumber.Trim(), verChildRef);
                    }
                }
            }

            catch (Exception p)
            {

                // MessageBox.Show("uuu" + p.Message);
            }


        }

        public static void CockSelList(int count)
        {
            ppoSelection = new EdmSelItem[count];
            i = 0;
        }

        public static void AddItemToSelList(this Model item)
        {

            try
            {
                ppoSelection[i] = new EdmSelItem();
                ppoSelection[i].mlDocID = item.bFile;
                ppoSelection[i].mlProjID = item.bFolder;
                i++;

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void BatchGet()
        {
            try
            {
                EdmCallbackGet cbGet = new EdmCallbackGet();
                batchGetter = (IEdmBatchGet)vault.CreateUtility(EdmUtility.EdmUtil_BatchGet);

                batchGetter.AddSelection((EdmVault5)vault1, ppoSelection);
                if ((batchGetter != null))
                {
                    batchGetter.CreateTree(0, (int)EdmGetCmdFlags.Egcf_Lock + (int)EdmGetCmdFlags.Egcf_SkipOpenFileChecks);// + (int)EdmGetCmdFlags.Egcf_IncludeAutoCacheFiles);  
                    batchGetter.GetFiles(0, cbGet);
                }

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DocBatchUnLock()
        {
            try
            {
                EDMCallback cb = new EDMCallback();
                batchUnlocker = (IEdmBatchUnlock2)vault.CreateUtility(EdmUtility.EdmUtil_BatchUnlock);
                batchUnlocker.AddSelection((EdmVault5)vault1, ref ppoSelection);
                batchUnlocker.CreateTree(0, (int)EdmUnlockBuildTreeFlags.Eubtf_MayUnlock);
                batchUnlocker.Comment = "Refresh";
                batchUnlocker.UnlockFiles(0, cb);

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        static void ConnectingPDM()
        {
            try
            {
                if (!vault1.IsLoggedIn)
                {
                    vault1.LoginAuto(NAME_PDM, 0);
                }
                vault = (IEdmVault7)vault1;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "connecting");
            }

        }

        public static bool IsDrawings(this Model comp)
        {
            IEdmFolder5 bFolder;
            IEdmFile7 bFile = null;
            try
            {
                string p = Path.Combine(Path.GetDirectoryName(comp.FullPath), comp.CubyNumber + ".SLDDRW");
                bFile = (IEdmFile7)vault.GetFileFromPath(p, out bFolder);
                if (bFile == null)
                {
                    p = Path.Combine(Path.GetDirectoryName(comp.FullPath), comp.CubyNumber + ".slddrw");
                    bFile = (IEdmFile7)vault.GetFileFromPath(p, out bFolder);
                }
                if (bFile != null)
                {
                    Drawing draw = new Drawing(comp.CubyNumber, p, comp, bFile, bFolder.ID);
                    draw.Level = comp.Level;
                    Tree.listDraw.Add(draw);

                    return true;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "connecting to CUBY_PDM");
            }
            return false;
        }

        public static int GetRefVersion(this Drawing draw)
        {
            int refDrToModel = -1;
            IEdmReference5 ref5 = draw.File.GetReferenceTree(draw.bFolder);
            IEdmReference10 ref10 = (IEdmReference10)ref5;
            IEdmPos5 pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
            while (!pos.IsNull)
            {
                IEdmReference10 @ref = (IEdmReference10)ref5.GetNextChild(pos);
                string extension = Path.GetExtension(@ref.Name);
                if (extension == ".sldasm" || extension == ".sldprt" || extension == ".SLDASM" || extension == ".SLDPRT")
                {
                    refDrToModel = @ref.VersionRef;
                    break;
                }
                else
                {
                    ref5.GetNextChild(pos);
                }
            }
            return refDrToModel;
        }

        public static void RefreshFile(this Model model)
        {
            model.File.Refresh();
        }
    }
}