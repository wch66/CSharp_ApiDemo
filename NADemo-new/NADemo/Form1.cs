using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NADemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NASDK currsdk = null;
        private void btnGetToken_Click(object sender, EventArgs e)
        {
            this.txtToken.Text = "......";
              currsdk = new NASDK(this.txtPltIP.Text, Convert.ToInt32(this.txtPort.Text), this.txtAppid.Text, this.txtAppPwd.Text,txtCertFile.Text, txtCertPwd.Text);
            TokenResult token = currsdk.getToken();
            if (token == null)
            {
                MessageBox.Show("获取失败，请看日志");
              
            }
            else
            {
                
                this.txtToken.Text = token.accessToken;
            }
     
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtPltIP.Text = "180.101.147.89";
            this.txtAppid.Text= "TWuD_BIfEGh4VqJMxMqTr49TlNIa";
            this.txtAppPwd.Text = "T_JmdPUzFlq6SrERB0goaC0E_c0a";
            this.txtCertPwd.Text = "IoM@1234";
            this.txtCertFile.Text = "iot3rd.p12";
        }
        private bool check()
        {
            if (currsdk == null)
            {
                MessageBox.Show("请先获取token");
                return false;
            }
            return true;
        }
        private void btnRegDevice_Click(object sender, EventArgs e)
        {
            this.txtDeviceID.Text = "......";
            this.txtPSK.Text = "......";
           
            if (!check()) { return; }
            RegDeviceResult rdr = currsdk.regDevice(txtToken.Text, txtVerifyCode.Text);
            if (rdr == null)
            {
                MessageBox.Show("获取失败，请看日志");

            }
            else
            {
                this.txtDeviceID.Text = rdr.deviceId;
                this.txtPSK.Text = rdr.psk;
                this.txtVerifyCode.Text = rdr.verifyCode;
            }
        }
        private void btnModifyDev_Click(object sender, EventArgs e)
        {
            this.txtModifyResult.Text = "......";
            if (!check()) { return; }
            string result = currsdk.modifyDevice(txtToken.Text, txtModDevID.Text, txtMid.Text, txtDModel.Text);
            if (result == null)
            {

                MessageBox.Show("获取失败，请看日志");

            }
            else
            {
                this.txtModifyResult.Text = result;
            }

        }
        private void btnSubcribe_Click(object sender, EventArgs e)
        {
            this.txtSubResult.Text = "......";
            if (!check()) { return; }
            string result = currsdk.subscribe(txtToken.Text,cbNotifyType.Text,txtCallbackURL.Text);
            if (result == null)
            {

                MessageBox.Show("获取失败，请看日志");
                return;
            }
            this.txtSubResult.Text = result;

        }

        private void btnQueryHistoryData_Click(object sender, EventArgs e)
        {
            txtHDCount.Text = "......";
            txtHDPageNo.Text = "......";
            txtHDPageSize.Text = "......";
            if (!check()) { return; }
            string startTime = dtpHDQueryStart.Value.ToString("yyyyMMddTHHmmssZ");
            string endTime=dtpHDQueryEnd.Value.ToString("yyyyMMddTHHmmssZ");
            int pageno = 0; int pagesize = 0;
            try
            {
                pageno = Convert.ToInt16(txtHDQueryPageNo.Text);
                pagesize = Convert.ToInt16(txtHDQueryPageSize.Text);
            }
            
         catch(Exception ex)
            {

            }
            HistoryDataResult result = currsdk.queryHistoryData(txtToken.Text, this.txtHDDevId.Text,txtHDServiceID.Text,pageno,pagesize,startTime,endTime);
            if (result == null)
            {

                MessageBox.Show("获取失败，请看日志");
                return;

            }
            txtHDCount.Text = result.totalCount.ToString();
            txtHDPageNo.Text = result.pageNo.ToString();
            txtHDPageSize.Text = result.pageSize.ToString();
            dgvHisData.DataSource = result.deviceDataHistoryDTOs;
         
        }









        private void cbhttp_CheckedChanged(object sender, EventArgs e)
        {
            if (currsdk != null)
            {
                currsdk.isHttp = cbhttp.Checked;
            }
        }

        private void btnAddPara_Click(object sender, EventArgs e)
        {
            string paraName = txtParaName.Text;
            string paraValue = txtParaValue.Text;
            
            lbCmdParas.Items.Add(paraName + ":" + paraValue + ":" + cbIsParaNum.Checked.ToString());
            cbIsParaNum.Checked = false;
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            txtSendCmdResult.Text = "......";
            if (!check()) { return; }
            List<CommandPara> lsCmdPars = new List<CommandPara>();
            for (int i = 0; i < lbCmdParas.Items.Count; i++)
            {
                string currPara = lbCmdParas.Items[i].ToString();
                string[] paras = currPara.Split(':');
                CommandPara currCmdPara = new CommandPara();
                currCmdPara.isNum = Convert.ToBoolean(paras[2]);
                currCmdPara.paraName = paras[0];
                currCmdPara.paraValue = paras[1];
                lsCmdPars.Add(currCmdPara);

            }
            string result = currsdk.sendCommand(txtToken.Text, this.txtSendCmdDevID.Text,txtCmdCallbackURL.Text,txtSendCMDServiceID.Text,txtCmdID.Text, lsCmdPars);
            if (result == null)
            {

                MessageBox.Show("获取失败，请看日志");
                return;

            }
            txtSendCmdResult.Text = result;
        }

        private void btnDelPara_Click(object sender, EventArgs e)
        {
            if (lbCmdParas.SelectedItem == null) { return; }
            lbCmdParas.Items.Remove(lbCmdParas.SelectedItem);
        }

        private void btnSetCert_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofg = new OpenFileDialog();
            if(ofg.ShowDialog()!= DialogResult.OK)
            {
                return;
            }
            txtCertFile.Text = ofg.FileName;
        }
    }
}
