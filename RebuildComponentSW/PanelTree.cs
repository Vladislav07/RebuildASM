using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace RebuildComponentSW
{
    public partial class PanelTree : UserControl
    {
        DataTable dt;
        private DataGridView dataGridView;
        public List<ViewUser> userView { get; set; }
        public PanelTree()
        {
            InitializeComponent();
            dt = new DataTable();
        }
        private byte[] GetImageData(int i)
        {

            Image image;
            switch (i)
            {
                case 0:
                    image = Properties.Resources.part;
                    break;
                case 1:
                    image = Properties.Resources.assembly;
                    break;
                case 2:
                    image = Properties.Resources.SWXUiSWV1Drawings;
                    break;
                default:
                    image = Properties.Resources.x;
                    break;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public bool LoadData()
        {
            GenerateDataGridView();
            FillToListIsRebuild();
            return true;
        }
        private void FillToListIsRebuild()
        {
            dt.Clear();

            if (userView.Count == 0) return;
            foreach (ViewUser v in userView)
            {
               /* if (v.State == "Rebuild" && !isDispleyRebuild) continue;
                if (v.State == "Clean" && !isClean) continue;
                if (v.State == "Blocked" && !isBlocked) continue;
                if (v.State == "Manufacturing" && !isImpossible) continue;*/

                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                if (v.Ext == ".sldprt" || v.Ext == ".SLDPRT")
                {
                    dr[1] = GetImageData(0);
                }
                else
                {
                    dr[1] = GetImageData(1);
                }

                dr[2] = v.Level;
                dr[3] = v.StPDM;
                dr[4] = v.State;
                dr[5] = v.IsLocked;
                if (v.DrawState != "")
                {
                    dr[6] = GetImageData(2);
                }
                else
                {
                    dr[6] = GetImageData(3);
                }
                dr[7] = v.StDrPDM;
                dr[8] = v.DrawVersRev;
                dr[9] = v.DrawNeedRebuild;
                dr[10] = v.DrawState;

                dt.Rows.Add(dr);
            }

        }
        private void GenerateDataGridView()
        {
            dataGridView = new DataGridView();
           // this.Width = 1000;

            dataGridView.Location = new Point(0, 50);
            dataGridView.BackgroundColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.Width = this.Width;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.GridColor = Color.White;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.DataBindingComplete += (sender, e) =>
            {
                int rowHeight1 = dataGridView.RowTemplate.Height;
                int headerHeight1 = dataGridView.ColumnHeadersHeight;
                int rowsHeight1 = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                int totalHeight1 = rowsHeight1 + headerHeight1;
                int maxAllowedHeight1 = 500;
                int desiredHeight1 = Math.Min(totalHeight1, maxAllowedHeight1);
                int minHeight1 = rowHeight1 + headerHeight1;

                dataGridView.Height = Math.Max(desiredHeight1, minHeight1);
                this.Height = 100 + dataGridView.Height;
            };
            Controls.Add(dataGridView);

            dt = new DataTable();
            dt.Columns.Add("Cuby_Number", typeof(string));
            dt.Columns.Add("Type", typeof(byte[]));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("Current Version", typeof(string));
            dt.Columns.Add("IsLocked", typeof(string));
            dt.Columns.Add("DrawType", typeof(byte[]));
            dt.Columns.Add("DrawState", typeof(string));
            dt.Columns.Add("DrawVersRev", typeof(string));
            dt.Columns.Add("DrawNeedRebuild", typeof(string));
            dt.Columns.Add("DrawIsLocked", typeof(string));

            dataGridView.DataSource = dt;

           // SetPropertiesGrig();
        }
    }
}
