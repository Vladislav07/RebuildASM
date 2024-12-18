using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Core;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.AddIn.Helpers;
using SolidWorksTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using CodeStack.SwEx.AddIn.Base;
using System.Xml.Serialization;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.AddIn.Delegates;
using RebuildComponentSW.Properties;
using CodeStack.SwEx.AddIn;

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
        [Icon(typeof(Resources), nameof(Resources.command1_icon))]
        CmdBuild

      
    }
    public enum TaskPaneCommands_e
    {
        [Title("TaskPaneCnd")]
        [Icon(typeof(Resources), nameof(Resources.command1_icon))]
        CmdBuild   
    }

    [Guid("15CE97A5-D10D-4169-98F5-091DFEC7A2D9"), ComVisible(true)]
    [AutoRegister("AddInSwRebuild", "AddInSWRebuldDoc", true)]
    public class SwAddIn:SwAddInEx
    {
        private IDocumentsHandler<DocumentHandler> swDocsHandler;
        public List<FileRef> list=null;
        private AssemblyTraversal tree = null;
        ModelDoc2 model = null;
        public override bool OnConnect()
        {
             AddCommandGroup<Commands_e>(OnCommandClick, OnCommandEnable);


            swDocsHandler = CreateDocumentsHandler();
            swDocsHandler.HandlerCreated += OnHandlerCreated;
            PanelTree ctrl;
            var taskPaneView = CreateTaskPane<PanelTree, TaskPaneCommands_e>(OnTaskPaneCommandClick, out ctrl);

            return true;
        }

        private void OnTaskPaneCommandClick(TaskPaneCommands_e cmd)
        {
            switch (cmd)
            {
                case TaskPaneCommands_e.CmdBuild:
                    App.SendMsgToUser("TaskPane Command1 clicked!");
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
                    list = new List<FileRef>();
                    if (model == null) return;
                    tree = new AssemblyTraversal(model);
                    tree.Main(ref list);
                    break;

    
            }
        }

        private void OnHandlerCreated(DocumentHandler handler)
        {
            handler.Activated += OnActivated;
        }

        private void OnActivated(DocumentHandler docHandler)
        {

            App.SendMsgToUser2($"'{docHandler.Model.GetTitle()}' activated",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
            model = (ModelDoc2)docHandler.Model;
        }

        private void OnDestroyed(DocumentHandler handler)
        {
            handler.Activated -= OnActivated;
        }

        public override bool OnDisconnect()
        {
            return base.OnDisconnect();
        }
        private void Displey(List<FileRef> list)
        {
           
        }
    }
}
