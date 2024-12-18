using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RebuildComponentSW
{
    public partial class PanelTree : UserControl
    {
        public PanelTree()
        {
            InitializeComponent();
        }
        public void  Displey(List<FileRef> list)
        {
            foreach (FileRef item in list)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = item.FileName;
                listView1.Items.Add(lvi);
            }
        }
    }
}
