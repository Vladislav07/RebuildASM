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
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Back, "Read");
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Ok,"Rebuild");
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Next, "Update");
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
                    taskPaneView.SetButtonState(1, false);
                    taskPaneView.SetButtonState(2, true);
                    taskPaneView.SetButtonState(3, true);
                    taskPaneView.SetButtonState(4, true);
                    break;
                case 1:
                    taskPaneView.SetButtonState(1, true);
                    taskPaneView.SetButtonState(2, false);
                    taskPaneView.SetButtonState(3, true);
                    taskPaneView.SetButtonState(4, false);
                    break;
                case 2:
                    taskPaneView.SetButtonState(1, true);
                    taskPaneView.SetButtonState(2, true);
                    taskPaneView.SetButtonState(3, false);
                    taskPaneView.SetButtonState(4, false);
                    break;

            }
        }


        private int Sld_ActiveModelDocChangeNotify()
        {
            ctrl.Clear();
            Tree.ClearCollection();
            return 0;
        }


        private int TaskPaneView_TaskPaneToolbarButtonClicked(int ButtonIndex)
        {

            switch ((ButtonIndex + 1))
            {
                case 1:
                    GetControler getCtrl;
                    getCtrl = new GetControler(ctrl, App.IActiveDoc2);
                    if (!getCtrl.GetData())
                    {
                        getCtrl = null;
                        return 0;
                    }
                    sld = (SldWorks)App;
                    sld.ActiveModelDocChangeNotify += Sld_ActiveModelDocChangeNotify;
                    st = StateApp.Read;
                    getCtrl =null;
                    break;
                case 2:
                    sld.ActiveModelDocChangeNotify -= Sld_ActiveModelDocChangeNotify;
                    ActionControler actionContr = new ActionControler(sld);
                    ctrl.DataAcquisitionProcess();
                    actionContr.RebuildTree();
                    st = StateApp.Rebuild;
                    actionContr = null;
                    break;
                break;
                case 3:
                    StatusCheckControler checkContrl=new StatusCheckControler(ctrl);
                    st = StateApp.Refresh;
                    break;

                case 4:
                    ctrl.Clear();
                    Tree.ClearCollection();
                    getCtrl = null;
                break;

            }
            return 1;
        }

        public override bool OnDisconnect()
        {
            taskPaneView.TaskPaneToolbarButtonClicked -= TaskPaneView_TaskPaneToolbarButtonClicked;
            taskPaneView.DeleteView();
            return base.OnDisconnect();
           
        }
       
    }
}
