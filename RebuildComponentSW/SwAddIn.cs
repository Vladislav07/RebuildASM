using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Core;
using CodeStack.SwEx.AddIn.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using CodeStack.SwEx.AddIn.Base;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.AddIn;
using RebuildComponentSW.Properties;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace RebuildComponentSW
{
    public enum StateApp
    {
        None = 0,
        Read = 1,
        Rebuild= 2,
        Refresh= 3,

    }

    [Guid("15CE97A5-D10D-4169-98F5-091DFEC7A2D9"), ComVisible(true)]
    [AutoRegister("AddInSwRebuild", "AddInSWRebuldDoc", true)]
    public class SwAddIn:SwAddInEx
    {
        
        PanelTree ctrl;
        TaskpaneView taskPaneView;
        SldWorks sld;
        StateApp st;
        public override bool OnConnect()
        {         
             taskPaneView = (TaskpaneView)CreateTaskPane<PanelTree>(out ctrl);
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Options, "Read");
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Ok,"Rebuild");
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Close, "Close");
             st=StateApp.None;
             SetState();
             taskPaneView.TaskPaneToolbarButtonClicked += TaskPaneView_TaskPaneToolbarButtonClicked;
             return true;
        }

        private void SetState()
        {
            switch ((int)st)
            {
                case 0:
                    taskPaneView.SetButtonState(0, true);
                    taskPaneView.SetButtonState(1, false);
                    taskPaneView.SetButtonState(2, false);
                    break;
                case 1:
                case 3:
                    taskPaneView.SetButtonState(0, false);
                    taskPaneView.SetButtonState(1, true);
                    taskPaneView.SetButtonState(2, true);
                    break;
                case 2:
                    taskPaneView.SetButtonState(0, false);
                    taskPaneView.SetButtonState(1, false);
                    taskPaneView.SetButtonState(2, true);
                    break;
            }
        }



        private int TaskPaneView_TaskPaneToolbarButtonClicked(int ButtonIndex)
        {
            DSldWorksEvents_ActiveDocChangeNotifyEventHandler activeDocChangeNotifyAddin = Sld_ActiveDocChangeNotify;
            switch ((ButtonIndex + 1))
            {
                case 1:
                    sld = (SldWorks)App;
                    ModelDoc2 model = (ModelDoc2)sld.ActiveDoc;
                    swDocumentTypes_e swDocType;
                   // ctrl.GenerateLabelMsgError();
                    ctrl.DataAcquisitionProcess();
                    if (model == null)
                    {
                        ctrl.Notifacation(0, new MsgInfo("Could not acquire an active document"));
                        return 0;
                    }
                    swDocType = (swDocumentTypes_e)model.GetType();


                    if (swDocType != swDocumentTypes_e.swDocASSEMBLY)
                    {
                        ctrl.Notifacation(0, new MsgInfo("This program only works with assemblies"));
                        return 0;
                    }
                    GetControler getCtrl;
                    getCtrl = new GetControler(ctrl, App.IActiveDoc2);

                    if (!getCtrl.GetData())
                    {
                        getCtrl.Dispose();
                        getCtrl=null;
                        return 0;
                        
                    }                  
                    sld.ActiveDocChangeNotify += activeDocChangeNotifyAddin;
                    st = StateApp.Read;
                    SetState();
                    getCtrl.Dispose();
                    getCtrl =null;
                    break;
                case 2:

                    sld.ActiveDocChangeNotify -= activeDocChangeNotifyAddin;
                    ActionControler actionContr = new ActionControler(ctrl,sld);
                    actionContr.RebuildTree();
                    st = StateApp.Rebuild;
                    SetState();
                    actionContr = null;
                    StatusCheck();
                    break;
                break;
                case 3:
                    ctrl.Clear();
                    Tree.ClearCollection();
                    st = StateApp.None;
                    SetState();
                    getCtrl = null;
                break;

            }
            return 1;
        }

        private void StatusCheck()
        {
            ctrl.Clear();
            StatusCheckControler checkContrl = new StatusCheckControler(ctrl);
            checkContrl.GetUpdatedData();
            st = StateApp.Refresh;
            SetState();
            checkContrl = null;
            
        }

        private int Sld_ActiveDocChangeNotify()
        {
            ctrl.Clear();
            Tree.ClearCollection();
            st = StateApp.None;
            SetState();
            return 0;
        }

        public override bool OnDisconnect()
        {
            taskPaneView.TaskPaneToolbarButtonClicked -= TaskPaneView_TaskPaneToolbarButtonClicked;
            taskPaneView.DeleteView();
            return base.OnDisconnect();
           
        }
        
       
    }
}
