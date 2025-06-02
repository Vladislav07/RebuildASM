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

namespace RebuildComponentSW
{
    [Title("CmdOpenPanel")]
    [Description("CmdOpen")]
    [Icon(typeof(Resources), nameof(Resources._01))]
    [CommandGroupInfo(0)]
    public enum Commands_e
    {
        [Title("CmdUpdateModel")]
        [Description("CmdUpdateModelActive")]
        [CommandItemInfo(true, true, swWorkspaceTypes_e.Assembly, true)]
        [Icon(typeof(Resources), nameof(Resources._01))]
        CmdConnect,

        [Title("CmdBuild")]
        [Description("CmdBuildassemble")]
        [CommandItemInfo(true, true, swWorkspaceTypes_e.Assembly, true)]
        [Icon(typeof(Resources), nameof(Resources._002))]
        CmdBuild,

        [Title("CmdRebuild")]
        [Description("CmdRebuildAssemble")]
        [CommandItemInfo(true, true, swWorkspaceTypes_e.Assembly, true)]
        [Icon(typeof(Resources), nameof(Resources._005))]
        CmdRebuild


    }
    public enum TaskPaneCommands_e
    {
        [Title("Update")]
        [Icon(typeof(Resources), nameof(Resources._01))]
        CmdConnect,
        [Title("Build")]
        [Icon(typeof(Resources), nameof(Resources._005))]
        CmdBuild,
        [Title("Rebuild")]
        [Icon(typeof(Resources), nameof(Resources._002))]
        CmdRebuild
    }

    [Guid("15CE97A5-D10D-4169-98F5-091DFEC7A2D9"), ComVisible(true)]
    [AutoRegister("AddInSwRebuild", "AddInSWRebuldDoc", true)]
    public class SwAddIn:SwAddInEx
    {
        
        PanelTree ctrl;
   
        public override bool OnConnect()
        {
            AddCommandGroup<Commands_e>(OnCommandClick, OnCommandEnable);
            var taskPaneView = CreateTaskPane<PanelTree, TaskPaneCommands_e>(OnTaskPaneCommandClick, out ctrl);
            taskPaneView.ShowView();
            return true;
        }

        private void OnTaskPaneCommandClick(TaskPaneCommands_e cmd)
        {
            
            SldWorks sld=(SldWorks)App;
            switch (cmd)
            {
  
                case TaskPaneCommands_e.CmdBuild:             
                    sld.ActiveModelDocChangeNotify += Sld_ActiveModelDocChangeNotify;
                    GetControler getCtrl;
                    getCtrl=new GetControler(ctrl, App.IActiveDoc2);
                    getCtrl.GetData();
          
                    break;
                case TaskPaneCommands_e.CmdRebuild:
                    sld.ActiveModelDocChangeNotify -= Sld_ActiveModelDocChangeNotify;
                    ActionControler actionContr = new ActionControler(App);
            
                    break;

            }
        }

        private int Sld_ActiveModelDocChangeNotify()
        {
            App.SendMsgToUser2("Model updated",
               (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
            return 0;
        }


        private void OnCommandEnable(Commands_e cmd, ref CommandItemEnableState_e state)
        {
            if (cmd == Commands_e.CmdBuild)
            {
                state = CommandItemEnableState_e.DeselectEnable;
            }
        }
        
        private void OnCommandClick(Commands_e cmd)
        {
            switch (cmd)
            {
                case Commands_e.CmdBuild:
                    GetControler getCtrl = new GetControler(ctrl, App.IActiveDoc2);
                    getCtrl.GetData();
                   
                    break;
                case Commands_e.CmdRebuild:
                    App.SendMsgToUser("Rebuild clicked!");
               
                    break;


            }
        }



        public override bool OnDisconnect()
        {
            return base.OnDisconnect();
        }
       
    }
}
