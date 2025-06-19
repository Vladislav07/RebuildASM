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
using static System.Net.Mime.MediaTypeNames;

namespace RebuildComponentSW
{
    public partial class PanelTree : UserControl
    {
        DataTable dt;
        private bool isClean = false;
        private bool isDispleyRebuild = true;
        private bool isImpossible = false;
        private bool isBlocked = false;
        private DataGridView dataGridView;
        private CheckBox chB_ToRebuild;
        private CheckBox chB_Clean;
        private CheckBox checkBox1;
        private CheckBox chB_Impossible;

        private Label lbMsg;
        private ProgressBar progressBar1;
        private Label lbStart;
        private Label lbCount;
        private Label lbNumber;

        public List<InfoView> userView { get; set; }
        public PanelTree()
        {
            InitializeComponent();
            dt = new DataTable();
        }
        private byte[] GetImageData(int i)
        {

            System.Drawing.Image image;
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

        public void DataAcquisitionProcess()
        {
            GenerateLabelsAndProgressBar();
        }
        public bool LoadData()
        {
            DestroyLabelsAndProgressBar();
            GenerateDataGridView();
            FillToListIsRebuild();
            GenerateNamedCheckBoxes();
            SetStateForm();
            return true;
        }
        private void FillToListIsRebuild()
        {
            dt.Clear();

            if (userView.Count == 0) return;
            foreach (InfoView v in userView)
            {
                if (v.State == "Rebuild" && !isDispleyRebuild) continue;
                if (v.State == "Clean" && !isClean) continue;
                if (v.State == "Blocked" && !isBlocked) continue;
                if (v.State == "Manufacturing" && !isImpossible) continue;

                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                switch (v.Ext)
                {
                    case ".sldprt":
                    case ".SLDPRT":
                        dr[1] = GetImageData(0);
                        break;
                    case ".sldasm":
                    case ".SLDASM":
                        dr[1] = GetImageData(1);
                        break;
                    case ".slddrw":
                    case ".SLDDRW":
                        dr[1] = GetImageData(2);
                        break;
                    default:
                        break;
                }
             

                dr[2] = v.Level;
                dr[3] = v.StPDM;
                dr[4] = v.State;
                dr[5] = v.IsLocked;

                dt.Rows.Add(dr);
            }
            
            this.Refresh();
        }
        private void GenerateDataGridView()
        {
            dataGridView = new DataGridView();

            dataGridView.Location = new Point(0, 70);
            dataGridView.BackgroundColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.Width = this.Width;
      
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.GridColor = Color.White;
            dataGridView.DataBindingComplete += (sender, e) =>
            {
                int rowHeight1 = dataGridView.RowTemplate.Height;
                int headerHeight1 = dataGridView.ColumnHeadersHeight;
                int rowsHeight1 = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                int totalHeight1 = rowsHeight1 + headerHeight1;
                int maxAllowedHeight1 = 800;
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
            dataGridView.DataSource = dt;

            SetPropertiesGrig();
        }
        private void SetPropertiesGrig()
        {
            dataGridView.Columns[0].Width = 150;
            dataGridView.Columns[1].Width = 50;
            dataGridView.Columns[2].Width = 40;
            dataGridView.Columns[3].Width = 100;
            dataGridView.Columns[4].Width = 40;
            dataGridView.Columns[5].Width = 70;

        }
        private void GenerateNamedCheckBoxes()
        {
            chB_ToRebuild = new CheckBox();
            chB_ToRebuild.Checked = true;
            chB_ToRebuild.Text = "To Rebuild";
            chB_ToRebuild.Name = "chB_ToRebuild";
            chB_ToRebuild.Location = new Point(50, 50);
            chB_ToRebuild.Size = new Size(100, 20);
            chB_ToRebuild.CheckedChanged += ChB_ToRebuild_CheckedChanged;
            this.Controls.Add(chB_ToRebuild);

            chB_Clean = new CheckBox();
            chB_Clean.Text = "Clean";
            chB_Clean.Name = "chB_Clean";
            chB_Clean.Location = new Point(150, 50);
            chB_Clean.Size = new Size(100, 20);
            chB_Clean.CheckedChanged += ChB_Clean_CheckedChanged;
            this.Controls.Add(chB_Clean);

            checkBox1 = new CheckBox();
            checkBox1.Text = "Blocked";
            checkBox1.Name = "checkBox1";
            checkBox1.Location = new Point(250, 50);
            checkBox1.Size = new Size(100, 20);
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            this.Controls.Add(checkBox1);

            chB_Impossible = new CheckBox();
            chB_Impossible.Text = "Manufacturing";
            chB_Impossible.Name = "chB_Impossible";
            chB_Impossible.Location = new Point(350, 50);
            chB_Impossible.Size = new Size(100, 20);
            chB_Impossible.CheckedChanged += ChB_Impossible_CheckedChanged;
            this.Controls.Add(chB_Impossible);
        }
        private void ChB_Impossible_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isImpossible = true;
            }
            else
            {
                isImpossible = false;
            }
            RefreshForm();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isBlocked = true;
            }
            else
            {
                isBlocked = false;
            }
            RefreshForm();
        }

        private void ChB_Clean_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isClean = true;
            }
            else
            {
                isClean = false;
            }
            RefreshForm();
        }

        private void ChB_ToRebuild_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isDispleyRebuild = true;
            }
            else
            {
                isDispleyRebuild = false;

            }
            RefreshForm();
        }

        private void SetStateForm()
        {
            if (userView == null) return;
            if (userView.Any(v => v.State == "Rebuild"))
            {
                chB_ToRebuild.Enabled = true;

            }
            else
            {
                chB_ToRebuild.Enabled = false;

            }
            if (userView.Any(v => v.State == "Clean"))
            {
                chB_Clean.Enabled = true;
            }
            else
            {
                chB_Clean.Enabled = false;
            }
            if (userView.Any(v => v.State == "Blocked"))
            {
                checkBox1.Enabled = true;

            }
            else
            {
                checkBox1.Enabled = false;

            }
            if (userView.Any(v => (v.State == "Manufacturing")))
            {
                chB_Impossible.Enabled = true;
            }
            else
            {
                chB_Impossible.Enabled = false;
            }
            this.Refresh();

        }
        private void RefreshForm()
        {
            SetStateForm();
            FillToListIsRebuild();
            this.Refresh();
        }
        public void GenerateLabelMsgError()
        {
            lbMsg = new Label();
            lbMsg.Location = new Point(25, 5);
            lbMsg.AutoSize = true; 
            lbMsg.ForeColor = Color.Green;
            lbMsg.Font = new Font(lbMsg.Font, FontStyle.Bold);
            lbMsg.Font = new Font(lbMsg.Font.FontFamily, 12); 
            lbMsg.TextAlign = ContentAlignment.TopLeft; 
            this.Controls.Add(lbMsg);
        }
        private void GenerateLabelsAndProgressBar()
        {
            GenerateLabelMsgError();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            progressBar1.Location = new Point(50, 100);
            progressBar1.Width = 250;
            progressBar1.Height = 15;
            this.Controls.Add(progressBar1);

            lbStart = new Label();
            lbStart.Text = "0";
            lbStart.Location = new Point(15, 100);
            this.Controls.Add(lbStart);

            lbCount = new Label();

            lbCount.Text = "0";
            lbCount.Location = new Point(320, 100);
            this.Controls.Add(lbCount);

            lbNumber = new Label();
            lbNumber.Text = "...";
            lbNumber.Width = 200;
            lbNumber.Location = new Point(100, 80);
            this.Controls.Add(lbNumber);
        }
        public void DestroyLabelsAndProgressBar()
        {
            lbMsg.Dispose();
            progressBar1.Dispose();
            lbStart.Dispose();
            lbCount.Dispose();
            lbNumber.Dispose();
        }
        public void DestroyGridView()
        {
            dataGridView.Dispose();
            chB_ToRebuild.Dispose();
            chB_Clean.Dispose();
            checkBox1.Dispose();
            chB_Impossible.Dispose();
            this.Refresh();
        }
        public void Notifacation(int typeEvent, MsgInfo msg)
        {
             switch (typeEvent)
             {
                 case 0:       //Error
                 
                     this.lbMsg.Text = msg.errorMsg;                 
                     lbMsg.ForeColor = Color.Red;

                     break;

                 case 1:      //LoadActiveModel
                     this.Text = msg.numberCuby;

                     break;
                 case 2:      //BeginOperation
                     lbMsg.ForeColor = Color.Green;
                     this.lbMsg.Text = msg.typeOperation;
                     this.progressBar1.Maximum = msg.countStep;
                     this.progressBar1.Minimum = 0;
                     this.lbStart.Text = "0";
                     this.lbCount.Text = msg.countStep.ToString();
                     this.Refresh();
                     break;
                 case 3:      //StepOperation
                     this.lbStart.Text = msg.currentStep.ToString();
                    try
                        {
                           this.progressBar1.Value = msg.currentStep;
                        }
                        catch (Exception)
                        {
                        
                        }
                     this.lbNumber.Text = msg.numberCuby;
                     this.Refresh();
                    break;
                 default:
                     break;
            }
            
        }
        public void Clear()
        {
            Controls.Clear();
            dt.Clear();
  
        }
    
    }
}

