using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timothy_Anondo.Models;

namespace Timothy_Anondo
{
    public partial class DataEntryForm : Form
    {

        public DataEntryForm()
        {
            InitializeComponent();

            

           foreach (string table in Program._UnitOfWork.GetTables())
            {
                TabPage tabPage = new TabPage();
                // 
                // tabPage1
                // 
                tabPage.Name = table;
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = new System.Drawing.Size(633, 403);
                tabPage.Text = string.Format("{0} Table", new CultureInfo("en-US", false).TextInfo.ToTitleCase(table.ToLower()));
                tabPage.UseVisualStyleBackColor = true;


                this.tabControl1.Controls.Add(tabPage); 
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl _TabControl = sender as TabControl;

            var data = Program._UnitOfWork.RawSql(_TabControl.SelectedTab.Name) as IEnumerable<object>;



            var dgv = new DataGridView()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                //Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                //Location = new System.Drawing.Point(245, 14),
                //Margin = new System.Windows.Forms.Padding(3, 13, 3, 3),
                Name = "Table",
                //Tag = new VariableCarier { BookMarkData = thisnode, TreeViewModel = nodedata },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            };
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = data.ToList();

            TabControl InnerTab = loadinnercontrols();
            TabPage AllData = InnerTab.Controls.Find("AllData", true).FirstOrDefault() as TabPage;
            AllData.Controls.Add(dgv);

            PropertyInfo[] propertyInfos = Type.GetType(string.Format("Timothy_Anondo.Models.{0}", _TabControl.SelectedTab.Name), true, true).GetProperties();
           // var f = propertyInfos.Cast<Dictionary<object, object>>();
            TableLayoutPanel _TableLayoutPanel = createtablelayout(propertyInfos, _TabControl.SelectedTab.Name);
            TabPage NewData = InnerTab.Controls.Find("NewData", true).FirstOrDefault() as TabPage;
            NewData.Controls.Add(_TableLayoutPanel);

            _TabControl.SelectedTab.Controls.Clear();
            _TabControl.SelectedTab.Controls.Add(InnerTab);
        }

        public TableLayoutPanel createtablelayout(IEnumerable<PropertyInfo> properties, string name)
        {
            TableLayoutPanel _TableLayoutPanel = new TableLayoutPanel();
             _TableLayoutPanel.ColumnCount = 2;
            _TableLayoutPanel.RowCount = 0;
            _TableLayoutPanel.Dock = DockStyle.Top;

            _TableLayoutPanel.AutoSize = true;
            _TableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            _TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            _TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            string guid = Guid.NewGuid().ToString("D");

            foreach (var property in properties)
            {
                _TableLayoutPanel.RowCount = _TableLayoutPanel.RowCount + 1;
                _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                _TableLayoutPanel.Controls.Add(new Label()
                {
                    AutoSize = true,
                    //BackColor = System.Drawing.Color.LightGray,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Location = new System.Drawing.Point(11, 16),
                    Margin = new System.Windows.Forms.Padding(10, 15, 10, 15),
                    Size = new System.Drawing.Size(220, 18),
                    TabIndex = 0,
                    Text = property.Name,
                }, 0, _TableLayoutPanel.RowCount - 1);

                _TableLayoutPanel.Controls.Add(new TextBox()
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Location = new System.Drawing.Point(245, 14),
                    Margin = new System.Windows.Forms.Padding(3, 13, 3, 3),
                    Name =  guid,
                    Tag = property.Name,
                    Size = new System.Drawing.Size(381, 22)
                }, 1, _TableLayoutPanel.RowCount - 1);
            }
            _TableLayoutPanel.RowCount = _TableLayoutPanel.RowCount + 1;
            _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var saveButton = new Button()
            {
                AutoSize = true,
                Dock = System.Windows.Forms.DockStyle.Left,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Location = new System.Drawing.Point(11, 16),
                Text = "Save Data",
                Name = name,
                Tag = guid
            };
            saveButton.Click += SaveButton_Click;
            _TableLayoutPanel.Controls.Add(saveButton, 1, _TableLayoutPanel.RowCount - 1);

            return _TableLayoutPanel;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Button saveButton = sender as Button;

            var data = saveButton.Parent.Controls.Find(saveButton.Tag.ToString(), true).Where(i => i is TextBox).Select(i => new KeyValuePair<string, string>(i.Tag.ToString(), i.Text)).ToList();

            Program._UnitOfWork.ExecuteSqlCommand(saveButton.Name, data);
            
            
        }

        public TabControl loadinnercontrols()
        {
            TabControl InnertabControl = new TabControl();
            TabPage AllData = new TabPage();
            TabPage NewData = new TabPage();

            InnertabControl = new System.Windows.Forms.TabControl();
            AllData = new TabPage();
            NewData = new System.Windows.Forms.TabPage();
            InnertabControl.SuspendLayout();
            
            // 
            // InnertabControl
            // 
            InnertabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            InnertabControl.Controls.Add(AllData);
            InnertabControl.Controls.Add(NewData);
            InnertabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            InnertabControl.Location = new Point(0, 0);
            InnertabControl.Multiline = true;
            InnertabControl.Name = "InnertabControl";
            InnertabControl.SelectedIndex = 0;
            InnertabControl.Size = new System.Drawing.Size(521, 306);
            InnertabControl.TabIndex = 0;
            // 
            // AllData
            // 
            AllData.Location = new Point(4, 4);
            AllData.Name = "AllData";
            AllData.Padding = new System.Windows.Forms.Padding(3);
            AllData.Size = new System.Drawing.Size(513, 280);
            AllData.TabIndex = 0;
            AllData.Text = "All Data";
            AllData.UseVisualStyleBackColor = true;
            // 
            // NewData
            // 
            NewData.Location = new Point(4, 4);
            NewData.Name = "NewData";
            NewData.Padding = new System.Windows.Forms.Padding(3);
            NewData.Size = new System.Drawing.Size(513, 280);
            NewData.TabIndex = 1;
            NewData.Text = "New Data";
            NewData.UseVisualStyleBackColor = true;
            

            return InnertabControl;
        }

        private void DataEntryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
