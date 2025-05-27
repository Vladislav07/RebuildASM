using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebuildComponentSW
{
    internal abstract class ModelCondition
    {
        public StateModel stateModel;
        public abstract ModelCondition GetState(bool isMode);
        public abstract ModelCondition GetStateFromChild(StateModel isModeFromChild);
    }


    internal class ModeClear : ModelCondition
    {

        public ModeClear()
        {
            stateModel = StateModel.Clean;
        }
        public override ModelCondition GetState(bool isMode)
        {
            ModelCondition mode = null;
            if (isMode)
            {
                mode = new ModeRebuild();
            }
            else
            {
                mode = this;
            }
            return mode;
        }

        public override ModelCondition GetStateFromChild(StateModel isModeFromChild)
        {
            ModelCondition mode = null;
            if (isModeFromChild == StateModel.Blocked)
            {
                mode = new ModeBloced();
            }
            else
            {
                mode = new ModeRebuild();
            }
            return mode;
        }
    }
    internal class ModeBloced : ModelCondition
    {

        public ModeBloced()
        {
            stateModel = StateModel.Blocked;
        }
        public override ModelCondition GetState(bool isMode)
        {
            return this;
        }

        public override ModelCondition GetStateFromChild(StateModel isModeFromChild)
        {
            return this;
        }
    }

    internal class ModeRebuild : ModelCondition
    {

        public ModeRebuild()
        {
            stateModel = StateModel.Rebuild;
        }
        public override ModelCondition GetState(bool isMode)
        {
            return this;
        }

        public override ModelCondition GetStateFromChild(StateModel isModeFromChild)
        {
            ModelCondition mode = null;
            if (isModeFromChild == StateModel.Blocked)
            {
                mode = new ModeBloced();
            }
            else
            {
                mode = this;
            }
            return mode;
        }
    }
    internal class ModeManufacturing : ModelCondition
    {

        public ModeManufacturing()
        {
            stateModel = StateModel.Manufacturing;
        }
        public override ModelCondition GetState(bool isMode)
        {
            ModelCondition mode = null;
            if (isMode)
            {
                mode = new ModeBloced();
            }
            else
            {
                mode = this;
            }
            return mode;
        }

        public override ModelCondition GetStateFromChild(StateModel isModeFromChild)
        {
            return new ModeBloced();
        }
    }
}
