using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDM.Interop.epdm;

namespace RebuildComponentSW
{
    internal class EDMCallback: IEdmUnlockOpCallback 
    {
       
        public void ProgressBegin(EdmProgressType eType, EdmUnlockEvent eEvent, int lSteps)
        {
            MsgInfo info = new MsgInfo("",true, "", "CheckIn files", lSteps);
            PDM.NotifyOperation(2, info);
        }

        public bool ProgressStep(EdmProgressType eType, string bsText, int lProgressPos)
        {
            MsgInfo info = new MsgInfo();
            string nameFile = Path.GetFileName(bsText);
            info.numberCuby = nameFile;
            info.currentStep = lProgressPos+1;
            PDM.NotifyOperation(3, info);
            return true;
        }

        public bool ProgressStepEvent(EdmProgressType eType, EdmUnlockEventMsg eText, int lProgressPos)
        {

            return true;
        }

        public void ProgressEnd(EdmProgressType eType)
        {
            MsgInfo info = new MsgInfo();
            info.typeOperation = "Finish CheckIn";
            info.countStep = 0;
            PDM.NotifyOperation(2, info);
        }

        public EdmUnlockOpReply MsgBox(EdmUnlockOpMsg eMsg, int lDocID, int lProjID, string bsPath, ref EdmUnlockErrInfo poError)
        {
            throw new NotImplementedException();
        }
    }
    public struct MsgInfo
    {
        public string errorMsg {get;set;}
        public bool isResult { get; set; }
        public string typeError { get; set; }
        public string typeOperation { get; set; }
        public int countStep { get; set; }
        public string numberCuby { get; set; }
        public int currentStep { get; set; }

        public MsgInfo(string errorMsg,
            bool isResult = true,
            string typeError = "",
            string typeOperation = "",
            int countStep = -1,
            string numberCuby = "",
            int currentStep = -1)
        {
            this.errorMsg = errorMsg;
            this.isResult = isResult;
            this.typeError = typeError;
            this.typeOperation = typeOperation;
            this.countStep = countStep;
            this.numberCuby = numberCuby;
            this.currentStep = currentStep;
        }
    }
}
