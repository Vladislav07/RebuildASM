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
    [Title("CmdOpenPanel")]
    [Description("CmdOpen")]
    [Icon(typeof(Resources), nameof(Resources._01))]
    [CommandGroupInfo(1)]
    public enum Commands_e
    {
        [Title("CmdUpdateModel")]
        [Description("CmdUpdateModelActive")]
        [CommandItemInfo(true, false, swWorkspaceTypes_e.Assembly, false)]
        [Icon(typeof(Resources), nameof(Resources._01))]
        CmdConnect,
    }
  

    [Guid("15CE97A5-D10D-4169-98F5-091DFEC7A2D9"), ComVisible(true)]
    [AutoRegister("AddInSwRebuild", "AddInSWRebuldDoc", true)]
    public class SwAddIn:SwAddInEx
    {
        
        PanelTree ctrl;
        TaskpaneView taskPaneView;
        GetControler getCtrl;
        SldWorks sld;
        public override bool OnConnect()
        {         
             AddCommandGroup<Commands_e>(OnCommandClick, OnCommandEnable);
             //taskPaneView = (TaskpaneView)CreateTaskPane<PanelTree, TaskPaneCommands_e>(OnTaskPaneCommandClick, out ctrl);
             taskPaneView = (TaskpaneView)CreateTaskPane<PanelTree>(out ctrl);
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Ok,
              "Rebuild");
             taskPaneView.AddStandardButton((int)swTaskPaneBitmapsOptions_e.swTaskPaneBitmapsOptions_Close, "Close");
             taskPaneView.SetButtonState(1,false);
             taskPaneView.SetButtonState(0, true);
        
            return true;
        }


        private int Sld_ActiveModelDocChangeNotify()
        {
            ctrl.Clear();
            Tree.ClearCollection();
            getCtrl=null;
            return 0;
        }


        private void OnCommandEnable(Commands_e cmd, ref CommandItemEnableState_e state)
        {
   

        }
        
        private void OnCommandClick(Commands_e cmd)
        {
            bool isShow = false;
            switch (cmd)
            { 
               
                case Commands_e.CmdConnect:

                    getCtrl = new GetControler(ctrl, App.IActiveDoc2);
                    if (!isShow)
                        {
                            taskPaneView.TaskPaneToolbarButtonClicked += TaskPaneView_TaskPaneToolbarButtonClicked;
                            taskPaneView.ShowView();
                            isShow = true;
                        }
                    else
                        {
                            taskPaneView.TaskPaneToolbarButtonClicked -= TaskPaneView_TaskPaneToolbarButtonClicked;
                            taskPaneView.HideView();
                        }
                    
                    if (!getCtrl.GetData())
                        {
                            getCtrl = null;
                            return;
                        }
                        sld = (SldWorks)App;
                   
                        sld.ActiveModelDocChangeNotify += Sld_ActiveModelDocChangeNotify;
                        isShow = false;
                    
                    break;
        
                

            }
        }

        private int TaskPaneView_TaskPaneToolbarButtonClicked(int ButtonIndex)
        {
            switch ((ButtonIndex + 1))
            {
                case 1:
                    sld.ActiveModelDocChangeNotify -= Sld_ActiveModelDocChangeNotify;
                    ActionControler actionContr = new ActionControler(sld);
                    ctrl.Clear();
                    ctrl.DataAcquisitionProcess();
                    actionContr.RebuildTree();
                    getCtrl.GetUpdatedData();
                    break;
                case 2:
                    ctrl.Clear();
                    Tree.ClearCollection();
                    getCtrl = null;
                    break;

            }
            return 1;
        }

        public override bool OnDisconnect()
        {
            taskPaneView.DeleteView();
            return base.OnDisconnect();
        }
       
    }
}
