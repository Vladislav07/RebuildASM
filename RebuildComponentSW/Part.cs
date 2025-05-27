using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RebuildComponentSW;

namespace RebuildComponentSW
{
    public class Part : Model
    {
        public virtual event Action<string, StateModel> NotificationParent;


        internal Part(string cn, string fn) : base(cn, fn)
        {
            listParent = new List<string>();
        }



        public void NotificationState(StateModel mode)
        {

            foreach (string item in listParent)
            {
                Notification(item, mode);
            }
        }


        protected void Notification(string item, StateModel isBloced)
        {
            try
            {
                NotificationParent.Invoke(item, isBloced);
            }
            catch (Exception)
            {
                MessageBox.Show("Parent - " + item.ToString());

            }

        }



    }
}
