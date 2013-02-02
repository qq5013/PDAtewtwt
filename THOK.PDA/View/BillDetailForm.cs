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
    public partial class BillDetailForm : Form
    {

        private BillDal dal = new BillDal();
        public int Index;
        private DataRow detailRow = null;


        private string billType = "";
        private string detailID = "";
        private string billID = "";

        public BillDetailForm(string billType, string detailID, string billID)
        {
            InitializeComponent();
            this.billType = billType;
            this.detailID = detailID;
            this.billID = billID;
            if (this.billType == "3")
            {
                this.button1.Visible = true;
                this.button2.Visible = true;
                this.button3.Visible = true;
                this.button4.Visible = true;
            }
            else
            {
                this.button1.Visible = false;
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button4.Visible = false;
            }
        }

        private void BillDetailForm_Load(object sender, EventArgs e)
        {
            switch (billType)
            {
                case "1":
                    this.label2.Text = "入库单据明细";
                    break;
                case "2":
                    this.label2.Text = "出库单据明细";
                    break;
                case "3":
                    this.label2.Text = "盘点单据明细";
                    break;
                case "4":
                    this.label2.Text = "移库单据明细";
                    break;
            }

            if (SystemCache.ConnetionType == "USB连接")
            {
                detailRow = SystemCache.DetailTable.Select("MASTER='" + billID + "' AND DETAILID='" + detailID + "'")[0];
            }
            else
            {
                detailRow = dal.GetBillDetailByDetailId(billID, detailID);
            }

            this.lbID.Text = detailRow["DETAILID"].ToString();
            this.lbStorageID.Text = detailRow["STORAGEID"].ToString();
            this.lbTobacconame.Text = detailRow["TOBACCONAME"].ToString();
            this.lbPiece.Text = detailRow["OPERATEPIECE"].ToString();
            this.lbItem.Text = detailRow["OPERATEITEM"].ToString();
            this.lbState.Text = detailRow["STATENAME"].ToString();
            this.lbType.Text = detailRow["OPERATENAME"].ToString();

            if (detailRow["MOVESTORAGE"].ToString() != "")
            {
                this.lbType.Text = this.lbType.Text + "->" + detailRow["MOVESTORAGE"].ToString();
            } 
            this.lbBillid.Text = billID;
            WaitCursor.Restore();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            WaitCursor.Set();
            try
            {
                if (SystemCache.ConnetionType == "USB连接")
                {

                }
                else
                {
                    dal.ConfirmTask(billID, detailRow["DETAILID"].ToString(), "1", Convert.ToInt32(detailRow["OPERATEPIECE"].ToString()),
                                                                Convert.ToInt32(detailRow["OPERATEITEM"].ToString()), "0", "", "pda");
                }

                BaseTaskForm baseTaskForm = new BaseTaskForm(this.billType, billID);
                baseTaskForm.index = this.Index;
                baseTaskForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitCursor.Restore();
                MessageBox.Show("读取数据失败!" + ex.Message);
            }
        }

        private void BillDetailForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Escape")
            {
                this.btnBack_Click(null, null);
            }
            if (e.KeyCode.ToString() == "Return")
            {
                this.btnComplete_Click(null, null);
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            WaitCursor.Set();
            try
            {
                if (SystemCache.ConnetionType == "USB连接")
                {
                    new XMLBillDal().UpdateBill(billID, detailID,lbPiece.Text,lbItem.Text);
                }
                else
                {
                    dal.ConfirmTask(billID, detailRow["DETAILID"].ToString(), "3", Convert.ToInt32(lbPiece.Text),Convert.ToInt32(lbItem.Text), "1", "执行成功", "pda");
                }
                MessageBox.Show("确认成功!");
                BaseTaskForm baseTaskForm = new BaseTaskForm(this.billType, billID);
                if (this.Index > 0)
                {
                     baseTaskForm.index = this.Index;
                }
                baseTaskForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitCursor.Restore();
                MessageBox.Show(ex.Message);
                this.Close();
                SystemCache.MainFrom.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbPiece.Text = Convert.ToString(Convert.ToInt32(lbPiece.Text) + 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lbPiece.Text = Convert.ToString(Convert.ToInt32(lbPiece.Text) - 1);
            if (Convert.ToInt32(lbPiece.Text) < 0)
            {
                lbPiece.Text = "0";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lbItem.Text = Convert.ToString(Convert.ToInt32(lbItem.Text) + 1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            lbItem.Text = Convert.ToString(Convert.ToInt32(lbItem.Text) - 1);
            if (Convert.ToInt32(lbItem.Text) < 0)
            {
                lbItem.Text = "0";
            }
        }
    }
}