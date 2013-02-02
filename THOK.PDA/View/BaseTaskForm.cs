using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using THOK.PDA.Util;
using THOK.PDA.Dal;

namespace THOK.PDA.View
{
    public partial class BaseTaskForm : Form
    {
        private BillDal dal = new BillDal();
        private string billType = "";
        private string billId = "";
        public int index;

        public BaseTaskForm(string billType, string billId)
        {
            InitializeComponent();
            this.billType = billType;
            this.billId = billId;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BaseTaskForm_Load(object sender, EventArgs e)
        {
            this.label2.Text = billId;
            switch (billType)
            {
                case "1":
                    this.label2.Text += "(入库)";
                    break;
                case "2":
                    this.label2.Text += "(出库)";
                    break;
                case "3":
                    this.label2.Text += "(盘点)";
                    break;
                case "4":
                    this.label2.Text += "(移库)";
                    break;
            }
            DataTable tempTable = null;
            if (SystemCache.ConnetionType == "USB连接")
            {
                tempTable = new DataTable();
                tempTable.Columns.Add("STORAGEID");
                tempTable.Columns.Add("OPERATENAME");
                tempTable.Columns.Add("TOBACCONAME");
                tempTable.Columns.Add("STATENAME");
                tempTable.Columns.Add("DETAILID");
                DataRow[] detailRows = SystemCache.DetailTable.Select("MASTER='" + billId + "' AND ConfirmState <> '3'");
                for (int i = 0; i < detailRows.Length; i++)
                {
                    DataRow row = tempTable.NewRow();
                    row["STORAGEID"] = detailRows[i]["STORAGEID"];
                    row["OPERATENAME"] = detailRows[i]["OPERATENAME"];
                    row["TOBACCONAME"] = detailRows[i]["TOBACCONAME"];
                    row["STATENAME"] = detailRows[i]["STATENAME"];
                    row["DETAILID"] = detailRows[i]["DETAILID"];
                    tempTable.Rows.Add(row);
                }
            }
            else
            {
                tempTable = dal.GetBillDetailListByBillId(billId);
            }
            this.dgInfo.DataSource = tempTable;
            if (tempTable.Rows.Count == 0)
            {
                this.btnNext.Enabled = false;
            }

            DataGridTableStyle gridStyle = new DataGridTableStyle();
            gridStyle.MappingName = tempTable.TableName;
            dgInfo.TableStyles.Add(gridStyle);
            GridColumnStylesCollection columnStyles = this.dgInfo.TableStyles[0].GridColumnStyles;

            columnStyles["STORAGEID"].HeaderText = "   货  位";
            columnStyles["STORAGEID"].Width = 80;

            columnStyles["OPERATENAME"].HeaderText = "  类  型";
            columnStyles["OPERATENAME"].Width = 50;
            columnStyles["TOBACCONAME"].HeaderText = "   烟  名";
            columnStyles["TOBACCONAME"].Width = 80;
            columnStyles["STATENAME"].HeaderText = "  状态";
            columnStyles["STATENAME"].Width = 50;
            columnStyles["DETAILID"].HeaderText = "单据号";

            if (tempTable.Rows.Count != 0)
            {
                if (tempTable.Rows.Count <= index)
                {
                    index = tempTable.Rows.Count-1;
                }
                dgInfo.Select(index);
                dgInfo.CurrentRowIndex = index;
                dgInfo.Focus();
            }
            WaitCursor.Restore();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            WaitCursor.Set();
            try
            {
                BillMasterForm billMasterForm = new BillMasterForm(billType);
                billMasterForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitCursor.Restore();
                MessageBox.Show(ex.Message);
            }
        }

        private void dgInfo_CurrentCellChanged(object sender, EventArgs e)
        {
            this.dgInfo.Select(this.dgInfo.CurrentCell.RowNumber);
            this.index = this.dgInfo.CurrentCell.RowNumber;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            WaitCursor.Set();
            try
            {
                if (!(SystemCache.ConnetionType == "USB连接"))
                {
                    dal.ApplyTask(billId, this.dgInfo[this.dgInfo.CurrentCell.RowNumber, 4].ToString());
                }

                BillDetailForm billDetailForm = new BillDetailForm(billType, this.dgInfo[this.dgInfo.CurrentCell.RowNumber, 4].ToString(), billId);
                billDetailForm.Index = this.index;
                billDetailForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitCursor.Restore();
                MessageBox.Show(ex.Message);
            }
        }

        private void BaseTaskForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Escape")
            {
                this.btnBack_Click(null, null);
            }
            if (e.KeyCode.ToString() == "Return")
            {
                this.btnNext_Click(null, null);
            }
        }
    }
}