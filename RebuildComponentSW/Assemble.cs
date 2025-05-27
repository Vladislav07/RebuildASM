
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RebuildComponentSW
{

    internal class Assemble : Part
    {
        public Dictionary<string, int> listRefChild;
        public Dictionary<string, string> listRefChildError;
        internal Assemble(string cn, string fn) : base(cn, fn)
        {
            listRefChild = new Dictionary<string, int>();
            listRefChildError = new Dictionary<string, string>();
        }

        public override void SetState()
        {
            listRefChildError.Clear();
            base.SetState();
            this.GetReferenceFromAssemble();

            bool isRebuildAsm = isNeedsRebuld();

            if (isRebuildAsm) condition = condition.GetState(true);

        }
        public void CascadingUpdate(StateModel isBlocrdChild)
        {

            condition = condition.GetStateFromChild(isBlocrdChild);

        }

        bool isNeedsRebuld()
        {
            if (listRefChild.Count == 0) return false;

            foreach (KeyValuePair<string, int> item in listRefChild)
            {

                int isVers = Tree.Part_IsChild(item.Key, item.Value);

                if (isVers != -1)
                {
                    listRefChildError.Add(item.Key, item.Value.ToString() + "/" + isVers.ToString());
                }


            }
            return (listRefChildError.Count > 0) ? true : false;

        }

    }
}