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

namespace RebuildComponentSW
{
    [Title("CmdOpenPanel")]
    [Description("CmdOpen")]
    [Icon(typeof(Resources), nameof(Resources._01))]
    [CommandGroupInfo(0)]
    public enum Commands_e
    {
        [Title("CmdBuild")]
        [Description("CmdBuildassemble")]
        [Icon(typeof(Resources), nameof(Resources._002))]
        CmdBuild,

        [Title("CmdRebuild")]
        [Description("CmdRebuildAssemble")]
        [Icon(typeof(Resources), nameof(Resources._005))]
        CmdRebuild


    }
    public enum TaskPaneCommands_e
    {
        [Title("TaskPaneCnd")]
        [Icon(typeof(Resources), nameof(Resources._005))]
        CmdBuild,
        [Title("TaskPaneCnd2")]
        [Icon(typeof(Resources), nameof(Resources._002))]
        CmdRebuild
    }

    [Guid("15CE97A5-D10D-4169-98F5-091DFEC7A2D9"), ComVisible(true)]
    [AutoRegister("AddInSwRebuild", "AddInSWRebuldDoc", true)]
    public class SwAddIn:SwAddInEx
    {
        private SwDocumentsHandler swDocs;

        PanelTree ctrl;
        public override bool OnConnect()
        {
          /*   AddCommandGroup<Commands_e>(OnCommandClick);

            swDocs =(SwDocumentsHandler)CreateDocumentsHandler<SWDocHandler>();
            swDocs.HandlerCreated += SwDocs_HandlerCreated;*/
     
            var taskPaneView = CreateTaskPane<PanelTree, TaskPaneCommands_e>(OnTaskPaneCommandClick, out ctrl);

            return true;
        }

        private void SwDocs_HandlerCreated(SWDocHandler obj)
        {
            obj.Activated += Obj_Activated;
        }

        private void Obj_Activated(DocumentHandler docHandler)
        {
            throw new NotImplementedException();
        }

        private void OnTaskPaneCommandClick(TaskPaneCommands_e cmd)
        {
            switch (cmd)
            {
                case TaskPaneCommands_e.CmdBuild:

                    
                    break;
                case TaskPaneCommands_e.CmdRebuild:

                    App.SendMsgToUser("TaskPane Command2 clicked!");
                    break;

            }
        }
        
        private void OnCommandEnable(Commands_e cmd, ref CommandItemEnableState_e state)
        {
            if (cmd == Commands_e.CmdBuild)
            {
                if (state == CommandItemEnableState_e.DeselectEnable)
                {
                    if (App.IActiveDoc2?.ISelectionManager?.GetSelectedObjectCount2(-1) == 0)
                    {
                        state = CommandItemEnableState_e.DeselectDisable;
                    }
                }
            }
        }
        
        private void OnCommandClick(Commands_e cmd)
        {
            switch (cmd)
            {
                case Commands_e.CmdBuild:
                    App.SendMsgToUser("Command1 clicked!");
            
                    break;
                case Commands_e.CmdRebuild:
                    App.SendMsgToUser("Rebuild clicked!");
               
                    break;


            }
        }



        private void OnActivated(DocumentHandler docHandler)
        {

/*            App.SendMsgToUser2($"'{docHandler.Model.GetTitle()}' activated",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);*/
          
        }

        private void OnDestroyed(DocumentHandler handler)
        {
            handler.Activated -= OnActivated;
        }

        public override bool OnDisconnect()
        {
            return base.OnDisconnect();
        }
       
    }
}
