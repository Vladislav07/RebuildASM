using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDM.Interop.epdm;

namespace RebuildComponentSW
{
    public class EdmCallbackGet: IEdmGetOpCallback
    {
        public void ProgressBegin(EdmProgressType eType, int lSteps)
        {
            MsgInfo info = new MsgInfo("", true, "", "CheckOut files", lSteps);
            PDM.NotifyOperation(2, info);
        }

        public bool ProgressStep(EdmProgressType eType, string bsMessage, int lProgressPos)
        {
            MsgInfo info = new MsgInfo();
            string nameFile=Path.GetFileName(bsMessage);
            info.numberCuby = nameFile;
            info.currentStep = lProgressPos + 1;
            PDM.NotifyOperation(3, info);
            return true;
        }

        public void ProgressEnd(EdmProgressType eType)
        {
            MsgInfo info = new MsgInfo();
            info.typeOperation = "Finish CheckOut";
            info.countStep = 0;
            info.currentStep = 0;
            PDM.NotifyOperation(2, info);
        }

        public void LogMessage(EdmGetOpMsg eMsgID, int hCode, string bsDetails)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmReplace(EdmGetConfirmReason eReason, string bsPath)
        {
            throw new NotImplementedException();
        }

        public bool ReportFailure(int lFileID, string bsPath, int hError, string bsDetails)
        {
            MsgInfo info = new MsgInfo();
            info.errorMsg=bsDetails;
            info.numberCuby= Path.GetFileName(bsPath);
            PDM.NotifyOperation(0, info);
            return true;
        }
    }
}
