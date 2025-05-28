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
        private SwDocumentsHandler swDocs;
        private SW sw;
        PanelTree ctrl;
        public override bool OnConnect()
        {
            AddCommandGroup<Commands_e>(OnCommandClick, OnCommandEnable);

            swDocs =(SwDocumentsHandler)CreateDocumentsHandler<SWDocHandler>();
            swDocs.HandlerCreated += SwDocs_HandlerCreated;          
            var taskPaneView = CreateTaskPane<PanelTree, TaskPaneCommands_e>(OnTaskPaneCommandClick, out ctrl);
            taskPaneView.ShowView();
            return true;
        }


        private void SwDocs_HandlerCreated(SWDocHandler obj)
        {
            obj.Activated += Obj_Activated;            
        }

        private  void Obj_Activated(DocumentHandler docHandler)
        {
            int lErrors = 0;
            int lWarnings = 0;
            ModelDoc2 swModelDoc = (ModelDoc2)docHandler.Model;
            IModelDocExtension extMod;
            extMod = swModelDoc.Extension;
            // extMod.Rebuild((int)swRebuildOptions_e.swRebuildAll);
            extMod.ForceRebuildAll();
            swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
            swModelDoc.Close();
            docHandler.Dispose();

        }

        private void OnTaskPaneCommandClick(TaskPaneCommands_e cmd)
        {
            switch (cmd)
            {
                case TaskPaneCommands_e.CmdBuild:
                    sw = new SW(App.IActiveDoc2);
                    sw.ReadTree();
                    Tree.SearchParentFromChild();
                    Tree.FillCollection();
                    Tree.ReverseTree();
                    Tree.GetInfoPDM();
                    Tree.CompareVersions();
                    ctrl.userView = Tree.JoinCompAndDraw();
                    ctrl.LoadData();
                    break;
                case TaskPaneCommands_e.CmdRebuild:
                    ActionControler actionContr = new ActionControler(App);

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
