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
using THOK.WES.Interface;
using THOK.WES.Interface.Model;

namespace THOK.PDA.View
{
    public partial class BillMasterForm : Form
    {
        #region//PDA���ݶ�ȡ��ʽ��Ӳ���
        private string url = @"http://59.61.87.212:8090/Task";
        #endregion
        private BillDal dal = new BillDal();
        private ConfigUtil configUtil = new ConfigUtil();

        private string billType = "";

        public BillMasterForm(string billType)
        {
            InitializeComponent();
            this.billType = billType;
        }

        private void BillMasterForm_Load(object sender, EventArgs e)
        {            
            switch (billType)
            {
                case "1":
                    this.label2.Text = "��������ݺ�";
                    break;
                case "2":
                    this.label2.Text = "���������ݺ�";
                    break;
                case "3":
                    this.label2.Text = "�̵������ݺ�";
                    break;
                case "4":
                    this.label2.Text = "�ƿ������ݺ�";
                    break;
            }
            DataTable tempTable = null;
            if (SystemCache.ConnetionType == "USB����")
            {              
                DataRow[] detailRows = SystemCache.MasterTable.Select("billType='"+billType+"'");
                tempTable = SystemCache.MasterTable.Clone();
                for (int i = 0; i < detailRows.Length; i++)
                {
                    tempTable.ImportRow(detailRows[i]);
                    
                }
                this.lbInfo.ValueMember = "masteId";
                this.lbInfo.DisplayMember = "masteId";
            }
            else
            {
                //tempTable = dal.GetBillMaster(this.billType);
                Task task = new Task(url);
                task.GetBillMasterCompleted += new Task.GetBillMasterCompletedEventHandler(delegate(bool isSuccess, string msg, BillMaster[] billMasters)
                {
                    this.lbInfo.DataSource = billMasters;
                    this.lbInfo.ValueMember = "BillNo";
                    this.lbInfo.DisplayMember = "BillNo";
                    task.SearchBillMaster(billType);
                });
                task.SearchBillMaster(billType);
            }

            //this.lbInfo.DataSource = tempTable;
            //if (tempTable.Rows.Count == 0)
            //{
            //    this.btnNext.Enabled = false;
            //}
            WaitCursor.Restore();            
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            SystemCache.MainFrom.Visible = true;
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            WaitCursor.Set();
            try
            {              
                BaseTaskForm baseTaskForm = new BaseTaskForm(this.billType, this.lbInfo.SelectedValue.ToString());
                baseTaskForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitCursor.Restore();
                MessageBox.Show("��ȡ����ʧ��!"+ex.Message);
            }
        }     

        private void BillMasterForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Escape")
            {
                this.btnHome_Click(null, null);
            }
            if (e.KeyCode.ToString() == "Return")
            {
                this.btnNext_Click(null, null);
            }
        }  
    }
}