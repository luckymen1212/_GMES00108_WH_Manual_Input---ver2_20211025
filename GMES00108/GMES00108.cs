using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.IO;

using JPlatform.Client.Library6.interFace;
using JPlatform.Client;
using JPlatform.Client.Controls6;
using JPlatform.Client.JBaseForm6;
using JPlatform.Client.JERPBaseForm6;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using System.Globalization;


namespace CSI.MES.P
{
    public partial class GMES00108 : JERPBaseForm
    {
        public GMES00108()
        {
            InitializeComponent();
        }
        double valid_date = 0;
        string[] strGridOrderSize = new string[40];
        #region[base]
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            picWaiting.Visible = false;     
            AddButton = false;
            DeleteRowButton = false;
            //SaveButton = false;
            //DeleteButton = false;
            PreviewButton = false;
            PrintButton = false;
            SetLookUp(cboCont_size, "", "L_COM0108_5", "");
            SetLookUp(cboPC, "", "L_COM0108_4", "");
            SetLookUp(cboForw, "", "L_COM0108_3", "");
            SetLookUp(cboForw2, "", "L_COM0108_6", "");
            SetLookUp(cboMline, "", "L_COM_MLINE", "MINI_LINE_CD <='004'");
            

            loadControl();

            fnQRY_P_GMES00108_Q_Grid1();    // 1번째 그리드
            fnQRY_P_GMES00108_Q("Q");
            fnQRY_P_GMES00108_Q_Grid3("Q2");    // 3번째 그리드
            fnQRY_P_GMES00108_Q_Grid4("Q3");    // 4번째 그리드
            init_Set_Form_Auth();  
        }

        public override void QueryClick()
        {
            if (!ValidateControls(panTop))
                return;
            InitControls(grdBase_Detail);

            if (chkID_B_Overrun.Checked == false)
            {

                if (cboPlant.EditValue == null || cboPlant.EditValue.ToString().Equals(""))
                {
                    cboPlant.Focus();
                    this.MessageBoxW("You have to select Plant");
                    return;
                }
                if (cboInOut.EditValue == null || cboInOut.EditValue.ToString().Equals(""))
                {
                    cboInOut.Focus();
                    this.MessageBoxW("You have to select In/Out");
                    return;
                }
                if (txtOBSNU.Text.Trim().Equals(""))
                {
                    txtOBSNU.Focus();
                    this.MessageBoxW("You have to input PO number");
                    return;
                }
                if (cboSeq.EditValue.ToString().Equals(""))
                {
                    if (cboSeq.EditValue == null)
                    {
                        SetLookUp(cboSeq, "", "L_COM001108_1", "OBS_NU='" + txtOBSNU.Text + "'");
                    }
                    cboSeq.Focus();
                    this.MessageBoxW("You have to select Item seq");
                    return;
                }
                txt_TotCartQty.Text = "";
                txtInputBefore.Text = "";
            }

            picWaiting.Visible = true;
            fnQRY_P_GMES00108_Q_HEAD("Q_H");        // 1번째의 header
            fnQRY_P_GMES00108_Q_InputBefore();
            txtOA.Text = fnMSR_OBS_OA_CHECK();
            //fnQRY_P_GMES00108_Q_DETAIL("Q_D");      // 2번째의 detail data
            fnQRY_P_GMES00108_Q("Q");
            fnQRY_P_GMES00108_Q_DETAIL_ORDER("Q_D");
            fnQRY_P_GMES00108_Q_DETAIL_TOT_IN("Q_T");
            fnQRY_P_GMES00108_Q_DETAIL_INPUT("Q_I");
           
            fnQRY_P_GMES00108_Q_Grid3("Q2");
            fnQRY_P_GMES00108_Q_Grid4("Q3");
            if (chkID_B_Overrun.Checked == true)
            {
                fnQRY_P_GMES00108_Q_COMPONENT("Q3");
            }

            picWaiting.Visible = false;

            int data_cnt = ((DataTable)gridControlEx01.DataSource).Rows.Count;

            if (data_cnt > 0)
            {
                init_Set_Form_Auth();
            }
            else
            {
                SaveButton = false;
                DeleteButton = false;
            }
        }

        public override void NewClick()
        {
            //////base.NewClick();
            ////////this.InitControls(panTop);
            ////////InitControls(grdBase);
            ////////buildHeader();

            ////////pcwait.Visible = true;
            //////this.Cursor = Cursors.WaitCursor;

            //////Hashtable ht = new Hashtable();

            //////ht.Add("LINE_CD", cboPlant.EditValue.ToString());
            //////ht.Add("AREA_CD", cbo_area.EditValue.ToString());
            //////ht.Add("GRADE_CD", cboGrade.EditValue.ToString());
            //////ht.Add("STYLE_CD", cboStyle.EditValue.ToString());
            //////ht.Add("DATE_S", cboDate1.DateTime.ToString("yyyy-MM-dd"));
            //////ht.Add("DATE_E", cboDate2.DateTime.ToString("yyyy-MM-dd"));

            //////object obj = OpenChildForm(@"\POPUP\CSI.MES.P.GMES00108_POP.dll", ht, OpenType.Modal);
            //////if (obj is String)
            //////{
            //////    if (Convert.ToString(obj) == "1")
            //////    {
            //////        //fn_Search(lean, "DETAIL_SIZE");
            //////        //fn_Search(lean, "");
            //////    }
            //////}

            ////////pcwait.Visible = false;
            //////this.Cursor = Cursors.Default;
        }

        public override void DeleteRowClick()
        {
            //try
            //{
            //    GridDeleteRow(grdBase_Detail);
            //    fnSAVE_P_GMES00108_U_1("D");
            //    QueryClick();
            //}
            //catch (Exception ex)
            //{
            //    this.MessageBoxW("DeleteRowClick" + ex.Message);
            //}
        }
        public override void DeleteClick()
        {
            try
            {
                if (cboPlant.EditValue == null || cboPlant.EditValue.ToString().Equals(""))
                {
                    cboPlant.Focus();
                    this.MessageBoxW("You have to select Plant");
                    return;
                }
                if (cboInOut.EditValue == null || cboInOut.EditValue.ToString().Equals(""))
                {
                    cboInOut.Focus();
                    this.MessageBoxW("You have to select In/Out");
                    return;
                }
                if (txtOBSNU.Text.Trim().Equals(""))
                {
                    txtOBSNU.Focus();
                    this.MessageBoxW("You have to input PO number");
                    return;
                }
                if (cboSeq.EditValue.ToString().Equals(""))
                {
                    cboSeq.Focus();
                    this.MessageBoxW("You have to select Item seq");
                    return;
                }

                if (fnWH_CLOSE() == "Y")
                {
                    this.MessageBoxW(" Already closing, can't delete !");
                    return;
                }

                if (chkID_B_Overrun.Checked == true)
                {
                    return;
                }
                else
                {
                    string strIN_OUT = string.Empty;
                    if (cboInOut.EditValue != null) strIN_OUT = cboInOut.EditValue.ToString();

                    if (strIN_OUT.Equals("I") && fnTIME_CLOSE() == "N")
                    {
                        this.MessageBoxW(" Time is over... !");
                        return;
                    }
                    //if (txtUserID.Text.Trim().Equals("") || txtUserPW.Text.Trim().Equals(""))
                    //{
                    //    this.MessageBoxW("Please input User/Password");
                    //    return;
                    //}
                    //if (fnQRY_P_GMES00108_Q_IDPW() == false)
                    //{
                    //    this.MessageBoxW("You entered the wrong User/Password");
                    //    return;
                    //}

                    //GridDeleteRow(grdBase_Detail);
                    fnSAVE_P_GMES00108_U_1("D");
                    QueryClick();
                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("DeleteClick" + ex.Message);
            }
            finally
            {
                init_Set_Form_Auth();
            }
        }
        public override void AddClick()
        {
            try
            {
                //if (_dtMCS_Color_Info.Rows.Count == 0)
                //{
                //    load_MCS_Color_Info();
                //}
                //GridAddNewRow(grdBase);
            }
            catch (Exception ex)
            {
                this.MessageBoxW("AddClick" + ex.Message);
            }
        }

        public override void SaveClick()
        {
            try
            {
                string[] strTempParam = new string[11]; 
                for (int kk = 0; kk < strTempParam.Length; kk++)
                {
                    strTempParam[kk] = "";
                }
                strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                if (cboPlant.EditValue != null) strTempParam[1] = cboPlant.EditValue.ToString();
                if (cboInOut.EditValue != null) strTempParam[2] = cboInOut.EditValue.ToString();

                string strServerDate = fnSMW_Get_ServerDate();
                if (strTempParam[0].Equals(strServerDate) == false)
                {
                    this.MessageBoxW(" Wrong this date... !");
                    return;
                }

                if (chkID_B_Overrun.Checked == false)
                {
                    //'---KIEM TRA DA DONG CLOSING FGW CHUA---
                    if (strTempParam[2].Equals("I"))
                    {
                        if (strTempParam[1].Equals("000") && Convert.ToInt32(fnSMW_Count_Line000()) > 0)
                        {
                            this.MessageBoxW("WH closing this date... !");
                            return;
                        }
                        else
                        {
                            if (strTempParam[1].Equals("000") == false)
                            {
                                if (fnSMW_DAILY_CLOSE() == "Y")
                                {
                                    this.MessageBoxW("WH closing this date... !");
                                    return;
                                }
                            }
                        }
                    }
                    if (fnWH_CLOSE() == "Y")
                    {
                        this.MessageBoxW(" Already closing, can't save !");
                        return;
                    }

                    //'----END KIEM TRA----------------
                    //if (txtUserID.Text.Trim().Equals("") || txtUserPW.Text.Trim().Equals(""))
                    //{
                    //    this.MessageBoxW("Please input User/Password");
                    //    return;
                    //}
                    //if (fnQRY_P_GMES00108_Q_IDPW() == false)
                    //{
                    //    this.MessageBoxW("You entered the wrong User/Password");
                    //    return;
                    //}
                    if (cboPlant.EditValue == null || cboPlant.EditValue.ToString().Equals(""))
                    {
                        cboPlant.Focus();
                        this.MessageBoxW("You have to select Plant");
                        return;
                    }
                    if (cboInOut.EditValue == null || cboInOut.EditValue.ToString().Equals(""))
                    {
                        cboInOut.Focus();
                        this.MessageBoxW("You have to select In/Out");
                        return;
                    }
                    if (txtOBSNU.Text.Trim().Equals(""))
                    {
                        txtOBSNU.Focus();
                        this.MessageBoxW("You have to input PO number");
                        return;
                    }
                    if (cboSeq.EditValue.ToString().Equals(""))
                    {
                        cboSeq.Focus();
                        this.MessageBoxW("You have to select Item seq");
                        return;
                    }
                    if (txt_TotCartQty.Text.Trim().Equals(""))
                    {
                        txt_TotCartQty.Focus();
                        this.MessageBoxW("You have to input carton q'ty");
                        return;
                    }
                    if (cboInOut.EditValue.ToString().Equals("O"))
                    {
                        if (txtCONT_CD.Text.Trim().Equals(""))
                        {
                            txtCONT_CD.Focus();
                            this.MessageBoxW("You have to input Container Code");
                            return;
                        }
                        if (cboCont_size.EditValue== null)
                        {
                            this.MessageBoxW("You have to input Container Size");
                            return;
                        }
                    }
                    // ##########################################################################
                    int numTotalQty = 0;
                    bool isNum = int.TryParse(txt_TotCartQty.Text.Trim(), out numTotalQty);
                    if (!isNum)
                    {   //숫자가 아님
                        txt_TotCartQty.Focus();
                        this.MessageBoxW("You have to input carton q'ty");
                        return;
                    }
                    else
                    {   //숫자
                        //if (numTotalQty < 1)
                        //{
                        //    txt_TotCartQty.Focus();
                        //    this.MessageBoxW("You have to input carton q'ty");
                        //    return;
                        //}
                    }
                    // ########
                    //this.MessageBoxW("222");
                    int numBeforeQty = 0;
                    bool isNum2 = int.TryParse(txtInputBefore.Text.Trim(), out numBeforeQty);

                    DataTable dtOrderq = (DataTable)gridControlEx01.DataSource;
                    if (dtOrderq != null)
                    {
                        if (dtOrderq.Rows.Count > 0)
                        {
                            int val_CarQty = Convert.ToInt32(dtOrderq.Rows[0][5].ToString().Replace(",", ""));
                            if (val_CarQty < (numTotalQty + numBeforeQty))
                            {
                                txt_TotCartQty.Focus();
                                this.MessageBoxW(" Over carton q'ty");
                                return;
                            }
                        }
                    }
                    //this.MessageBoxW("333");
                    // ##########################################################################
                    int data_cnt = 0;
                    DataTable dtModified = (DataTable)grdBase_Detail.DataSource;
                    if (dtModified != null)
                    {
                        if (dtModified.Rows.Count > 0)
                        {
                            for (int i = 0; i <= dtModified.Rows.Count - 1; i++)
                            {
                                for (int j = 1; j < gvwBase_Detail.Columns.Count; j++)
                                {
                                    if (dtModified.Rows[i][j].ToString().Equals("0") == false)
                                    {
                                        data_cnt += 1;
                                    }
                                }
                            }
                        }
                    }
                    if (data_cnt < 1)
                    {
                        this.MessageBoxW("Input the size QTY!");
                        return;
                    }
                    //this.MessageBoxW("444");
                    // ##########################################################################
                    // Checking MSR_DIV, OBS_TYPE
                    if (cboInOut.EditValue.ToString().Equals("I"))
                    {
                        string str_OBS_type = "";
                        DataTable dtOrder = (DataTable)gridControlEx01.DataSource;
                        if (dtOrder != null)
                        {
                            if (dtOrder.Rows.Count > 0 && dtOrder.Rows[0][0].ToString().Length > 7)
                            {
                                str_OBS_type = dtOrder.Rows[0][0].ToString().Substring(7, 2);
                            }
                        }

                        if (str_OBS_type.Equals("FT") && fnMSR_SCAN_IN_CHECK() == "N" && (fnMSR_OBS_OA_CHECK().Equals("OA") == false || txtOA.Text.ToString().Equals("OA") == false))
                        {
                            this.MessageBoxW(" Can't save, Please scan incoming ! (" + str_OBS_type + ")");
                            return;
                        }
                    }
                    //this.MessageBoxW("555");
                    // Checking Shipping > Incoming
                    if (cboInOut.EditValue.ToString().Equals("O"))
                    {
                        // Checking Carton Qty
                        int val_TotalIn_QTY = Convert.ToInt32(fnMSR_CARTON_QTY_CHECK());
                        int val_TotalOut_QTY = Convert.ToInt32(txt_TotCartQty.Text) + Convert.ToInt32(txtInputBefore.Text);

                        if (val_TotalIn_QTY < val_TotalOut_QTY)
                        {
                            this.MessageBoxW(string.Format(" Can't save, Incoming Carton({0}) < Shipping Carton({1}) !", val_TotalIn_QTY, val_TotalOut_QTY));
                            return;
                        }

                        // Checking Prs Qty In
                        int val_Prs_IN_QTY = Convert.ToInt32(fnMSR_PRS_IN_CHECK());

                        // Checking Prs Qty Out
                        int val_Prs_OUT_QTY = Convert.ToInt32(fnMSR_PRS_OUT_CHECK());
                        // Grid 전체 입력한 합계 수량

                        init_Set_Form_Auth();

                        int val_Detail_Total = 0;
                        DataTable dtInput = (DataTable)grdBase_Detail.DataSource;
                        if (dtInput != null)
                        {
                            if (dtInput.Rows.Count > 0)
                            {
                                if (dtInput.Rows[0][35].ToString().Equals("") == false)
                                    val_Detail_Total = Convert.ToInt32(dtInput.Rows[0][35].ToString().Replace(",",""));
                            }
                        }
                        if (val_Prs_IN_QTY < (val_Prs_OUT_QTY + val_Detail_Total))
                        {
                            this.MessageBoxW(string.Format(" Can't save, Incoming Carton({0}) < Shipping Carton({1}) !!",val_Prs_IN_QTY, (val_Prs_OUT_QTY + val_Detail_Total)));
                            return;
                        }
                    }
                }
                else
                {   // ID_B_Overun_COMPONENT Shipping
                    for (int kk = 0; kk < strTempParam.Length; kk++)
                    {
                        strTempParam[kk] = "";
                    }
                    strTempParam[0] = cboDate2.DateTime.ToString("yyyyMMdd");
                    if (cboGradeS.EditValue != null) strTempParam[1] = cboGradeS.EditValue.ToString();
                    if (cboPO.EditValue != null) strTempParam[2] = cboPO.EditValue.ToString();
                    //if (cboStyle.EditValue != null) strTempParam[3] = cboStyle.EditValue.ToString();
                    if (txtStyleCD.Visible == true)
                    {
                        strTempParam[3] = txtStyleCD.Text;
                    }
                    else
                    {
                        strTempParam[3] = cboStyle.EditValue.ToString().Replace("-", "");
                    }

                    if (strTempParam[1].Equals(""))
                    {
                        this.MessageBoxW(" You have to select Grade !");
                        return;
                    }
                    if (strTempParam[1].Equals("I"))
                    {
                        strTempParam[4] = "O";
                        strTempParam[5] = "111";
                    } 
                    else if (strTempParam[1].Equals("CFS"))
                    {
                        strTempParam[1] = "A";
                        strTempParam[4] = "O";
                        strTempParam[5] = "112";
                    }
                    else if (strTempParam[1].Equals("CUP"))
                    {
                        strTempParam[1] = "A";
                        strTempParam[4] = "O";
                        strTempParam[5] = "113";
                    }
                    else if (strTempParam[1].Equals("CSL"))
                    {
                        strTempParam[1] = "A";
                        strTempParam[4] = "O";
                        strTempParam[5] = "114";
                    }
                    else if (strTempParam[1].Equals("CQD"))
                    {
                        strTempParam[1] = "A";
                        strTempParam[4] = "O";
                        strTempParam[5] = "115";
                    }

                    else
                    {
                        strTempParam[4] = "S";
                        strTempParam[5] = "___";
                    }
                    // ##########################################################################
                    int data_cnt = 0;
                    DataTable dtModified = (DataTable)gridControlEx03.DataSource;
                    if (dtModified != null)
                    {
                        if (dtModified.Rows.Count > 0)
                        {
                            for (int i = 0; i <= dtModified.Rows.Count - 1; i++)
                            {
                                for (int j = 1; j < gridViewEx03.Columns.Count; j++)
                                {
                                    if (dtModified.Rows[i][j].ToString().Equals("0") == false)
                                    {
                                        data_cnt += 1;
                                    }
                                }
                            }
                        }
                    }
                    if (data_cnt < 1)
                    {
                        this.MessageBoxW("Input the size QTY!");
                        return;
                    }
                    // ##########################################################################

                }
                //##################################################################################################################
                DialogResult rs = MessageBox.Show("Are you sure to save?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == System.Windows.Forms.DialogResult.No) return;

                picWaiting.Visible = true;
                if (chkID_B_Overrun.Checked == false)
                {
                    fnSAVE_P_GMES00108_U_1("S");
                }
                else
                {
                    fnSAVE_P_GMES00108_U_2("S");
                }

                QueryClick();

                picWaiting.Visible = false;
            }
            catch (Exception ex)
            {
                this.MessageBoxW("SaveClick" + ex.Message);
            }
            finally
            {
                init_Set_Form_Auth();
            }
        }
        #endregion

        // Checking Prs Qty Out
        private string fnMSR_PRS_OUT_CHECK()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_PRS_OUT_CNT", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "0";
        }
        // Checking Prs Qty In
        private string fnMSR_PRS_IN_CHECK()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_PRS_IN_CNT", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);

            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "0";
        }
        // Checking Carton Qty
        private string fnMSR_CARTON_QTY_CHECK()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_CTN_CNT", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "0";
        }
        private string fnMSR_OBS_OA_CHECK()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_OA1", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];

                double chk_cnt = 0;
                if (dtSource.Rows[0][0].ToString().Equals("") == false) chk_cnt = Convert.ToDouble(dtSource.Rows[0][0].ToString());
                if (chk_cnt > 0)
                {
                    return "OA";
                }
                else
                {
                    dtData = null;
                    dtData = cProc.SetParamData(dtData, "Q_OA2", strTempParam[0],
                                    txtOBSNU.Text,
                                    strTempParam[1], //cboSeq.EditValue.ToString(),
                                    strTempParam[2], //cboPlant.EditValue.ToString(),
                                    strTempParam[3], //cboInOut.EditValue.ToString(),
                                    "", //txtUserID.Text,
                                    "" //txtUserPW.Text,
                                    );
                    rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
                    if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable dtSource2 = rs.ResultDataSet.Tables[0];

                        double chk_cnt2 = 0;
                        if (dtSource.Rows[0][0].ToString().Equals("") == false) chk_cnt2 = Convert.ToDouble(dtSource.Rows[0][0].ToString());

                        if (chk_cnt2 > 0)
                        {
                            return "OA";
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            return "";
        }
        private string fnMSR_SCAN_IN_CHECK()
        {
            //Checking MSR_DIV, OBS_TYPE
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_CIN", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW(dtSource.Rows[0][0].ToString() + "," + dtSource.Rows[0][1].ToString());
                double chk_cnt = 0;
                double chk_sum = 0;
                if (dtSource.Rows[0][0].ToString().Equals("") == false) chk_cnt = Convert.ToDouble(dtSource.Rows[0][0].ToString());
                if (dtSource.Rows[0][1].ToString().Equals("") == false) chk_sum = Convert.ToDouble(dtSource.Rows[0][1].ToString());

                if (chk_sum < chk_cnt)
                    return "N";
                else
                    return "Y";
            }
            return "N";
        }
        private string fnSMW_Count_Line000()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "CL", strTempParam[0],
                            txtOBSNU.Text,   //txtOBSNU
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "0";
        }
        private string fnSMW_DAILY_CLOSE()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "CD", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "N";
        }
        private string fnWH_CLOSE()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMM");  // 년월만 체크 한다
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "C1", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "N";
        }
        private string fnTIME_CLOSE()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "CT", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                return dtSource.Rows[0][0].ToString();
            }
            return "N";
        }
        private void fnSMW_Get_StyleName()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            if (txtStyleCD.Visible == true)
            {
                strTempParam[2] = txtStyleCD.Text;
            } else {
                strTempParam[2] = cboStyle.EditValue.ToString().Replace("-","");
            }
            //this.MessageBoxW(strTempParam[2]);
            if (strTempParam[2].Equals("")) return;

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_STYLE", strTempParam[0],
                            txtOBSNU.Text,   //txtOBSNU
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(), Style_cd
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];

                lblStyleName.Text = dtSource.Rows[0][0].ToString();
                //return dtSource.Rows[0][0].ToString();
            }
            //return "";
        }

        private string fnSMW_Get_ServerDate()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_DATE", strTempParam[0],
                            txtOBSNU.Text,   //txtOBSNU
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(), Style_cd
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            "", //txtUserID.Text,
                            "" //txtUserPW.Text,
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];

                return dtSource.Rows[0][0].ToString();
            }
            else
            {
                return DateTime.Now.ToString("yyyyMMdd");
            }
        }
        private bool fnQRY_P_GMES00108_Q(string strWorkType)
        {
            try
            {
                //while (gvwBase_Detail.Columns.Count > 0)
                //{
                //    gvwBase_Detail.Columns.RemoveAt(0);
                //}
                string[] strTempParam = new string[10];
                for (int kk = 0; kk < strTempParam.Length; kk++)
                {
                    strTempParam[kk] = "";
                }
                strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
                if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
                if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, strWorkType, strTempParam[0],
                                txtOBSNU.Text,
                                strTempParam[1], //cboSeq.EditValue.ToString(),
                                strTempParam[2], //cboPlant.EditValue.ToString(),
                                strTempParam[3], //cboInOut.EditValue.ToString(),
                                "", //txtUserID.Text,
                                "" //txtUserPW.Text,
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, grdBase_Detail);
                ////////gvwBase_Detail.BestFitColumns();
                gvwBase_Detail.OptionsView.ColumnAutoWidth = false;

                //////for (int i = 0; i < gvwBase_Detail.Columns.Count; i++)
                //////{
                //////    gvwBase_Detail.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gvwBase_Detail.Columns[i].GetCaption().Replace("_", " ").ToLower());
                //////    gvwBase_Detail.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                //////}
                for (int kk = 1; kk < gvwBase_Detail.Columns.Count - 1; kk++)
                {
                    gvwBase_Detail.Columns[kk].OptionsColumn.ReadOnly = false;
                    gvwBase_Detail.Columns[kk].OptionsColumn.AllowEdit = true;
                    gvwBase_Detail.Columns[kk].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gvwBase_Detail.Columns[kk].DisplayFormat.FormatString = "#,###";

                    gvwBase_Detail.Columns[kk].Width = 50;
                }
                gvwBase_Detail.Columns[0].OptionsColumn.ReadOnly = true;
                gvwBase_Detail.Columns[0].OptionsColumn.AllowEdit = false;
                gvwBase_Detail.Columns[0].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gvwBase_Detail.Columns[35].OptionsColumn.ReadOnly = true;
                gvwBase_Detail.Columns[35].OptionsColumn.AllowEdit = false;
                gvwBase_Detail.Columns[35].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                grdBase_Detail.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q(): " + ex.Message);
                return false;
            }
            return true;
        }
        private bool fnSAVE_P_GMES00108_U_2(string strWorkType)
        {
            try
            {
                if (strWorkType.Equals("S") == true)
                {
                    SP_GMES1111_SAVE_Q dal = new SP_GMES1111_SAVE_Q();
                    DataTable dt = null;
                    //_ParamInfo.Add(new ParamInfo("@V_P_WORK_DATE", "Varchar", 10, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_GRADE", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_IO_DIV", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_LINE_CD", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_STYLE_CD", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_OBS_ID", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_SIZE_CD", "Varchar", 20, "Input", typeof(System.String)));
                    //_ParamInfo.Add(new ParamInfo("@V_P_PRS_QTY", "Varchar", 20, "Input", typeof(System.String)));

                    string[] strDatArray = new string[13];

                    for (int k = 0; k < strDatArray.Length; k++)
                    {
                        strDatArray[k] = "";
                    }
                    strDatArray[0] = cboDate2.DateTime.ToString("yyyyMMdd");//WORK_DATE
                    if (cboGradeS.EditValue != null) strDatArray[1] = cboGradeS.EditValue.ToString(); //GRADE
                    if (cboInOut.EditValue != null) strDatArray[2] = cboInOut.EditValue.ToString(); //IO_DIV
                    //if (cboStyle.EditValue != null) strDatArray[4] = cboStyle.EditValue.ToString();
                    if (txtStyleCD.Visible == true)
                    {
                        strDatArray[4] = txtStyleCD.Text; 
                    }
                    else
                    {
                        strDatArray[4] = cboStyle.EditValue.ToString().Replace("-", "");
                    }
                    if (cboPO.EditValue != null) strDatArray[5] = cboPO.EditValue.ToString();

                    if (strDatArray[1].Equals("I"))
                    {
                        strDatArray[2] = "O";
                        strDatArray[3] = "111";
                    }
                    else if (strDatArray[1].Equals("CFS"))
                    {
                        strDatArray[1] = "A";
                        strDatArray[2] = "O";
                        strDatArray[3] = "112";
                    }
                    else if (strDatArray[1].Equals("CUP"))
                    {
                        strDatArray[1] = "A";
                        strDatArray[2] = "O";
                        strDatArray[3] = "113";
                    }
                    else if (strDatArray[1].Equals("CSL"))
                    {
                        strDatArray[1] = "A";
                        strDatArray[2] = "O";
                        strDatArray[3] = "114";
                    }
                    else if (strDatArray[1].Equals("CQD"))
                    {
                        strDatArray[1] = "A";
                        strDatArray[2] = "O";
                        strDatArray[3] = "115";
                    }
                    else
                    {
                        strDatArray[2] = "S";
                        strDatArray[3] = "___";
                    }

                    //  + "/" + GetIPAddress()
                    dt = dal.SetParamData(dt, "S", // -- delete--
                            strDatArray,
                            SessionInfo.UserID
                    );
                    if (CommonProcessSave(ServiceInfo.LMESBizDB, dt, dal.ProcName, dal.GetParamInfo(), grdBase_Detail))
                    {
                        SP_GMES1111_SAVE_Q dal2 = new SP_GMES1111_SAVE_Q();
                        DataTable dtModified = (DataTable)gridControlEx03.DataSource; // this.BindingData(grdBase, true, false);
                        DataTable dt2 = null;
                        if (dtModified != null)
                        {
                            if (dtModified.Rows.Count > 0)
                            {
                                for (int k = 0; k < strDatArray.Length; k++)
                                {
                                    strDatArray[k] = "";
                                }
                                strDatArray[0] = cboDate2.DateTime.ToString("yyyyMMdd");
                                if (cboGradeS.EditValue != null) strDatArray[1] = cboGradeS.EditValue.ToString();
                                if (cboInOut.EditValue != null) strDatArray[2] = cboInOut.EditValue.ToString();
                                //if (cboStyle.EditValue != null) strDatArray[4] = cboStyle.EditValue.ToString();
                                if (txtStyleCD.Visible == true)
                                {
                                    strDatArray[4] = txtStyleCD.Text;
                                }
                                else
                                {
                                    strDatArray[4] = cboStyle.EditValue.ToString().Replace("-", "");
                                }
                                if (cboPO.EditValue != null) strDatArray[5] = cboPO.EditValue.ToString();
                                
                                

                                if (strDatArray[1].Equals("I"))
                                {
                                    strDatArray[2] = "O";
                                    strDatArray[3] = "111";
                                }
                                else if (strDatArray[1].Equals("CFS"))
                                {
                                    strDatArray[1] = "A";
                                    strDatArray[2] = "O";
                                    strDatArray[3] = "112";
                                }
                                else if (strDatArray[1].Equals("CUP"))
                                {
                                    strDatArray[1] = "A";
                                    strDatArray[2] = "O";
                                    strDatArray[3] = "113";
                                }
                                else if (strDatArray[1].Equals("CSL"))
                                {
                                    strDatArray[1] = "A";
                                    strDatArray[2] = "O";
                                    strDatArray[3] = "114";
                                }
                                else if (strDatArray[1].Equals("CQD"))
                                {
                                    strDatArray[1] = "A";
                                    strDatArray[2] = "O";
                                    strDatArray[3] = "115";
                                }
                                else
                                {
                                    strDatArray[2] = "S";
                                    strDatArray[3] = "___";
                                }
                                //--YEN THEM 2019.07.24--
                                if (cboMline.EditValue.ToString() != null)
                                {
                                    strDatArray[8] = cboMline.EditValue.ToString();
                                }
                                ///------END---
                                else
                                {
                                    this.MessageBoxW("Input mline");
                                    return false;
                                }

                                for (int ii = 0; ii < dtModified.Rows.Count; ii++)
                                {
                                    for (int jj = 0; jj < gridViewEx03.Columns.Count; jj++)
                                    {
                                        string V_P_SIZE = gridViewEx03.Columns[jj].FieldName;
                                        string V_P_QTY = dtModified.Rows[ii][jj].ToString();

                                        if (V_P_SIZE.Equals("") == false && V_P_QTY.Equals("") == false)
                                        {
                                            strDatArray[6] = V_P_SIZE;
                                            strDatArray[7] = V_P_QTY;

                                            

                                            dt2 = dal2.SetParamData(dt2, "SS",
                                                    strDatArray,
                                                    SessionInfo.UserID
                                            );
                                        }
                                    }
                                }

                                if (strDatArray[7].Equals("") ==true)
                                {
                                    MessageBox.Show("input qty!");
                                    return false;
                                }
                                if (CommonProcessSave(ServiceInfo.LMESBizDB, dt2, dal2.ProcName, dal2.GetParamInfo(), gridControlEx03))
                                {
                                    MessageBoxW("Save ok.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnSAVE_P_GMES00108_U_2(): " + ex.Message);
                return false;
            }
            return true;
        }

        private bool fnSAVE_CHECK_CONT_CD(string strWorkType)
        {
            try
            {
                if (strWorkType.Equals("CHECK") == true)
                {
                    SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                    DataTable dtData = null;
                    dtData = cProc.SetParamData(dtData, "CHECK", "",
                                    txtCONT_CD.Text.Trim(),
                                    "", // cboSeq.EditValue.ToString(),
                                    "", //cboPlant.EditValue.ToString(),
                                    "", //cboInOut.EditValue.ToString(),
                                    "",
                                    ""
                                    );
                    ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
                    if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable dtSource = rs.ResultDataSet.Tables[0];
                        if (dtSource.Rows.Count > 0 && dtSource != null)
                        {
                            if (dtSource.Rows[0][0].ToString() == "0")
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnSAVE_P_GMES00108_U_1(): " + ex.Message);
                return false;
            }
            return true;
        }

        private bool fnSAVE_CHECK_CONT(string strWorkType, string strCheck)
        {
            try
            {
                if (strWorkType.Equals("CHECK") == true)
                {
                    SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                    DataTable dtData = null;
                    dtData = cProc.SetParamData(dtData, "CHECK", "",
                                    strCheck,
                                    "", // cboSeq.EditValue.ToString(),
                                    "", //cboPlant.EditValue.ToString(),
                                    "", //cboInOut.EditValue.ToString(),
                                    "",
                                    ""
                                    );
                    ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
                    if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable dtSource = rs.ResultDataSet.Tables[0];
                        if (dtSource.Rows.Count > 0 && dtSource != null)
                        {
                            if (dtSource.Rows[0][0].ToString() == "0")
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnSAVE_P_GMES00108_U_1(): " + ex.Message);
                return false;
            }
            return true;
        }

        private bool fnSAVE_P_GMES00108_U_1(string strWorkType)
        {
            try
            {
                if (strWorkType.Equals("D") == true)
                {
                    SP_GMES0000_SAVE_Q dal = new SP_GMES0000_SAVE_Q();
                    DataTable dt = null; 

                    string[] strDatArray = new string[16];

                    for (int k = 0; k < strDatArray.Length; k++)
                    {
                        strDatArray[k] = "";
                    }
                    strDatArray[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                    strDatArray[1] = txtOBSNU.Text.ToString().Trim();
                    if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strDatArray[2] = cboSeq.EditValue.ToString();
                    if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strDatArray[3] = cboPlant.EditValue.ToString();
                    if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strDatArray[4] = cboInOut.EditValue.ToString();
                    strDatArray[5] = txtUserID.Text.ToString().Trim();
                    strDatArray[6] = txtUserPW.Text.ToString().Trim();

                    //  + "/" + GetIPAddress()
                    dt = dal.SetParamData(dt, strWorkType,
                            strDatArray,
                            SessionInfo.UserID
                    );
                    if (CommonProcessSave(ServiceInfo.LMESBizDB, dt, dal.ProcName, dal.GetParamInfo(), grdBase_Detail))
                    {
                        MessageBoxW("Delete ok.");
                    }
                }
                if (strWorkType.Equals("S") == true)
                {
                    if (cboInOut.EditValue.ToString().Equals("O"))
                    {
                        if (!fnSAVE_CHECK_CONT("CHECK", txtCONT_CD.Text.Trim()))
                        {
                            DialogResult dlr = MessageBox.Show("Container Code: " + txtCONT_CD.Text.Trim() +  " không có trên hệ thống bạn có muốn Save không?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dlr == System.Windows.Forms.DialogResult.Yes)
                            {

                            }
                            else
                            {
                                return false;
                            }
                        }

                    }
                    
                    SP_GMES0000_SAVE_Q dal = new SP_GMES0000_SAVE_Q();  
                    DataTable dt = null;

                    string[] strDatArray = new string[16];

                    for (int k = 0; k < strDatArray.Length; k++)
                    {
                        strDatArray[k] = "";
                    }
                    strDatArray[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                    strDatArray[1] = txtOBSNU.Text.ToString().Trim();
                    if (cboSeq.EditValue != null) strDatArray[2] = cboSeq.EditValue.ToString();
                    if (cboPlant.EditValue != null) strDatArray[3] = cboPlant.EditValue.ToString();
                    if (cboInOut.EditValue != null) strDatArray[4] = cboInOut.EditValue.ToString();
                    strDatArray[5] = txtUserID.Text.ToString().Trim();
                    strDatArray[6] = txtUserPW.Text.ToString().Trim();
                    strDatArray[7] = txt_TotCartQty.Text.ToString().Trim();
                    if (cboLocation.EditValue != null) strDatArray[10] = cboLocation.EditValue.ToString();
                    strDatArray[11] = txtCONT_CD.Text.ToString().Trim();
                    strDatArray[12] = cboCont_size.EditValue.ToString();
                    strDatArray[13] = cboPC.EditValue.ToString();
                    strDatArray[14] = cboForw.EditValue.ToString();
                    strDatArray[15] = cboForw2.EditValue.ToString();
                    //  + "/" + GetIPAddress()
                    dt = dal.SetParamData(dt, "S",
                            strDatArray,
                            SessionInfo.UserID
                    );
                    if (CommonProcessSave(ServiceInfo.LMESBizDB, dt, dal.ProcName, dal.GetParamInfo(), grdBase_Detail))
                    {
                        SP_GMES0000_SAVE_Q dal2 = new SP_GMES0000_SAVE_Q();
                        DataTable dtModified = (DataTable)grdBase_Detail.DataSource; // this.BindingData(grdBase, true, false);
                        DataTable dt2 = null;
                        if (dtModified != null)
                        {
                            if (dtModified.Rows.Count > 0)
                            {
                                for (int k = 0; k < strDatArray.Length; k++)
                                {
                                    strDatArray[k] = "";
                                }
                                strDatArray[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                                strDatArray[1] = txtOBSNU.Text.ToString().Trim();
                                if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strDatArray[2] = cboSeq.EditValue.ToString();
                                if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strDatArray[3] = cboPlant.EditValue.ToString();
                                if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strDatArray[4] = cboInOut.EditValue.ToString();
                                strDatArray[5] = txtUserID.Text.ToString().Trim();
                                strDatArray[6] = txtUserPW.Text.ToString().Trim();

                                for (int ii = 0; ii <= dtModified.Rows.Count - 1; ii++)
                                {
                                    for (int jj = 1; jj < gvwBase_Detail.Columns.Count - 1; jj++)
                                    {
                                        string V_P_SIZE = gvwBase_Detail.Bands[jj].Caption;
                                        string V_P_QTY = dtModified.Rows[ii][jj].ToString();

                                        if (V_P_SIZE.Equals("") == false && V_P_QTY.Equals("") == false)
                                        {
                                            strDatArray[7] = txt_TotCartQty.Text.ToString().Trim();
                                            strDatArray[8] = V_P_SIZE;
                                            strDatArray[9] = V_P_QTY;

                                            dt2 = dal2.SetParamData(dt2, "SS",
                                                    strDatArray,
                                                    SessionInfo.UserID
                                            );
                                        }
                                    }
                                }
                                if (CommonProcessSave(ServiceInfo.LMESBizDB, dt2, dal2.ProcName, dal2.GetParamInfo(), grdBase_Detail))
                                {
                                    MessageBoxW("Save ok.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnSAVE_P_GMES00108_U_1(): " + ex.Message);
                return false;
            }
            return true;
        }
        
        public class SP_GMES00108_1_Q : BaseProcClass
        {
            public SP_GMES00108_1_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SP_GMES00108_1_Q";
                ParamAdd();
            }
            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@V_P_TYPE", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_WORK_DATE", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_OBS_NU", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_OBS_SEQ_NU", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_LINE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_INOUT_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_USER_ID", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_USER_PW", "Varchar", 20, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable, System.String p_work_type, String V_P_WORK_DATE,  String V_P_OBS_NU, String V_P_OBS_SEQ_NU, String V_P_LINE_CD, String V_P_INOUT_CD
                , String V_P_USER_ID, String V_P_USER_PW)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                                    p_work_type, V_P_WORK_DATE, V_P_OBS_NU, V_P_OBS_SEQ_NU, V_P_LINE_CD, V_P_INOUT_CD, V_P_USER_ID, V_P_USER_PW
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        public class SP_GMES1111_SAVE_Q : BaseProcClass
        {
            public SP_GMES1111_SAVE_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SP_GMES00108_2_U_Q";
                ParamAdd();
            }
            private void ParamAdd()
            {
                _ParamInfo.Add(new ParamInfo("@P_WORK_TYPE", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_WORK_DATE", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_GRADE", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_IO_DIV", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_LINE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_STYLE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_OBS_ID", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_SIZE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_PRS_QTY", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_MLINE", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_UPD_USER", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable, String P_WORK_TYPE, String[] V_P_PARAM, string V_P_USER)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                                    P_WORK_TYPE, 
                                    V_P_PARAM[0],V_P_PARAM[1],V_P_PARAM[2],V_P_PARAM[3],V_P_PARAM[4],V_P_PARAM[5],V_P_PARAM[6],V_P_PARAM[7],V_P_PARAM[8],
                                    V_P_USER
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        public class SP_GMES0000_SAVE_Q : BaseProcClass
        {
            public SP_GMES0000_SAVE_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SP_GMES00108_1_U_4";
                ParamAdd();
            }
            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@P_WORK_TYPE", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_WORK_DATE", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_OBS_NU", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_OBS_SEQ_NU", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_LINE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_INOUT_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_USER_ID", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_USER_PW", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_TOTAL_CNT", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_SIZE_CD", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_EXAM_QTY", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_LOCATION", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_CONT_CD", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_CONT_SIZE", "Varchar", 50, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_CONT_PC", "Varchar", 50, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_CONT_FORW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_CONT_FORW2", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@V_P_UPD_USER", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable, String P_WORK_TYPE, String[] V_P_PARAM, string V_P_USER)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                                    P_WORK_TYPE, 
                                    V_P_PARAM[0],V_P_PARAM[1],V_P_PARAM[2],V_P_PARAM[3],V_P_PARAM[4],V_P_PARAM[5],V_P_PARAM[6],V_P_PARAM[7],V_P_PARAM[8],V_P_PARAM[9],V_P_PARAM[10],
                                    V_P_PARAM[11], V_P_PARAM[12], V_P_PARAM[13], V_P_PARAM[14],V_P_PARAM[15],
                                    V_P_USER
                };
                dataTable.Rows.Add(objData);  
                return dataTable;
            }
        }
      
        //private void gvwBase_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    try
        //    {
        //        DataTable dtb1 = (DataTable)grdBase.DataSource;
        //        decimal chgqty = 0;
        //        for (int i = 1; i <= dtb1.Columns.Count - 1; i++)
        //        {
        //            if (dtb1.Rows[dtb1.Rows.Count - 1][i].ToString() != string.Empty)
        //            {
        //                chgqty += Convert.ToInt32(dtb1.Rows[dtb1.Rows.Count - 1][i].ToString());
        //            }
        //        }
        //        dtb1.Rows[dtb1.Rows.Count - 1][0] = chgqty;
        //        SetData(grdBase, dtb1);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.MessageBoxW("gvwBase_CellValueChanged: " + ex.Message);
        //    }
        //}

        //private void cboWIP_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cbo_line.Contains(cboPlant.EditValue.ToString()))
        //        {

        //            SetLookUp(cboGrade, "", "L_COM0035_2","");
        //            SetLookUp(cboComponent, "", "L_COM0035_5", "wip_op_cd='" + cboPlant.EditValue.ToString() + "'");
        //            lb_process_line.Text = "Line";
        //            cboGrade.Visible = true;
        //            cboStyle_Cd.Visible = false;
        //            cboStyle_Cd.EditValue = "";
        //        }
        //        else 
        //        {
        //            SetLookUp(cboStyle_Cd, "", "L_COM0035_3", "");
        //            SetLookUp(cboComponent, "", "L_COM0035_4", "REMARKS1='Y'");
        //            lb_process_line.Text = "Process";
        //            cboGrade.Visible = false;
        //            cboGrade.EditValue = "";
        //            cboStyle_Cd.Visible = true;
        //        }
        //        fnQRY_P_GMES00108_Q("Q2");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.MessageBoxW("cboWIP_EditValueChanged: " + ex.Message);
        //    }
        //}

        //private void cboStyle_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cboStyle.EditValue.ToString() != "")
        //        {
        //          //  SetLookUp(cboOBS_TYPE, "", "L_COM0035_4", "STYLE_CD='" + cboStyle.Text.ToString().Replace("-", "") + "'");
        //            int v_num = cboStyle.EditValue.ToString().Length - 10;
        //            //MessageBox.Show("v_num " + v_num + "v_num2 " + cboStyle.EditValue.ToString());
        //            lbStyleName.Text = cboStyle.EditValue.ToString().Substring(10, v_num).ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.MessageBoxW("cboStyle_EditValueChanged: " + ex.Message);
        //    }
        //}

        //private void cboProcess_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!cbo_line.Contains(cboPlant.EditValue.ToString()))
        //        {
        //            SetLookUp(cboComponent, "", "L_COM0035_4", "wip_id='" + cboStyle_Cd.EditValue.ToString() + "'");
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        this.MessageBoxW("cboProcess_EditValueChanged: " + ex.Message);
        //    }
        //}

        private void gvwBase_Detail_RowClick(object sender, RowClickEventArgs e)
        {
            //try
            //{
            //    if (e.)
            //    string V_OBS_NO = gvwBase_Detail.GetRowCellDisplayText(e.RowHandle, "OBS#").Trim();

            //    if (radioButton2.Checked == true)
            //    {
            //        txtOBSNU.Text = V_OBS_NO;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    this.MessageBoxW("gvwBase_Detail_RowClick: " + ex.Message);
            //}
        }

        private void loadControl()
        {
            try
            {
                SetLookUp(cboPlant, "CODE", "L_COM0108_PLANT", "");
                //SetLookUp(cboStyle, "", "L_COM0035_6", "");
                SetLookUp(cboGradeS, "", "L_COM001108_2", "");
                //SetLookUp(cbo_area, "", "L_COM001108_2", "");

               // string strServerDate = fnSMW_Get_ServerDate();
               // DateTime dtNow = new DateTime(Convert.ToInt32(strServerDate.Substring(0, 4)), Convert.ToInt32(strServerDate.Substring(4, 2)), Convert.ToInt32(strServerDate.Substring(6, 2))); 

                //cboDate1.EditValue = DateTime.Now.ToString();
                //cboDate2.EditValue = DateTime.Now.ToString();
                //cboDate1.EditValue = dtNow.ToString();
               // cboDate2.EditValue = dtNow.ToString();
                Double.TryParse(cboDate1.DateTime.ToString("yyyyMMdd"), out  valid_date);

                
                loadControl_Set();

                //cboDate1.Enabled = false;
                //cboDate2.Enabled = false;
            }
            catch (Exception ex)
            {
                this.MessageBoxW("loadControl: " + ex.Message);
            }
        }
        private void loadControl_Set()
        {
            try
            {
                if (chkID_B_Overrun.Checked == true)
                {
                    cboDate2.Enabled = true;
                    cboGradeS.Enabled = true;
                    cboPO.Enabled = true;
                    cboStyle.Enabled = true;
                    lblMline.Enabled = false;
                    cboMline.Enabled = false;
                    

                    gridControlEx03.Enabled = true;
                    gridControlEx04.Enabled = true;
                    
                }
                else
                {
                    cboDate2.Enabled = false;
                    cboGradeS.Enabled = false;
                    cboPO.Enabled = false;
                    cboStyle.Enabled = false;
                    lblMline.Enabled = false;
                    cboMline.Enabled = false;

                    gridControlEx03.Enabled = false;
                    gridControlEx04.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                this.MessageBoxW("loadControl: " + ex.Message);
            }        
        }

        private DataTable getSize(string strWorkType, string line_cd, string area, string grade, string style_cd)
        {
            try
            {
                //SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                //DataTable dtData = null;
                //DataTable detail = null;
                //dtData = cProc.SetParamData(dtData,
                //                strWorkType,
                //                cboDate1.DateTime.ToString("yyyyMMdd"),
                //                cboDate2.DateTime.ToString("yyyyMMdd"),
                //                line_cd,
                //                area,
                //                grade,
                //                style_cd
                //                );
                ////dtData = cproc1.SetParamData(dtData, "2", dtinput.DateTime.ToString("yyyyMMdd"), line, "", grade, style_cd.Replace("-", "").Trim());
                //ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true);
                //if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
                //{
                //    detail = rs.ResultDataSet.Tables[0];
                //}
                //return detail;

                return null;
            }
            catch (Exception ex)
            {
                DataTable detail = null;
                this.MessageBoxW("getSize: " + ex.Message);
                return detail;
            }
        }

        private void SetValueCell(DataTable dtb1)
        {
            try
            {
                decimal total = 0;
                DataTable dtba = (DataTable)grdBase.DataSource;
                DataTable dtb = dtba.Clone();
                dtb.Rows.Add();
                int r = dtb.Rows.Count - 1;
                for (int i = 0; i <= dtb1.Rows.Count - 1; i++)
                {
                    for (int j = 0; j <= dtb.Columns.Count - 1; j++)
                    {
                        if (dtb1.Rows[i][0].ToString() == dtb.Columns[j].ColumnName)
                        {
                            dtb.Rows[0][j] = dtb1.Rows[i][1];
                        }
                    }
                }
                for (int i = 0; i <= dtb.Columns.Count - 1; i++)
                {
                    if (dtb.Rows[0][i] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(dtb.Rows[0][i]);
                    }
                }
                dtb.Rows[0][0] = total;
                dtb.AcceptChanges();
                SetData(grdBase, dtb);
            }
            catch (Exception ex)
            {
                this.MessageBoxW("SetValueCell: " + ex.Message);
            }
        }

        private void cboDate_EditValueChanged(object sender, EventArgs e)
        {
            Double.TryParse(cboDate1.DateTime.ToString("yyyyMMdd"), out valid_date);

            //if (cboDate1.DateTime > cboDate2.DateTime)
            //{
            //    cboDate2.EditValue = cboDate1.DateTime;
            //}
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioButton1.Checked == true)
            //{
            //    txtOBSNU.Text = "";
            //    txtLineCD.Text = "";
            //    txtOBSNU.ReadOnly = true;
            //    txtLineCD.ReadOnly = true;
            //    cboITEM.Reset();
            //    cboChangeTo.Reset();
            //}
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioButton2.Checked == true)
            //{
            //    txtOBSNU.ReadOnly = false;
            //    txtLineCD.ReadOnly = false;
            //}
        }

        private void gvwBase_Detail_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                //e.Column.OptionsColumn.AllowEdit = true;

                //if (e.Column.FieldName.Equals("OBS#") && radioButton2.Checked == true)
                //{
                //    string V_OBS_NO = gvwBase_Detail.GetRowCellDisplayText(e.RowHandle, "OBS#").Trim();

                //    txtOBSNU.Text = V_OBS_NO;

                //    string strSearch = string.Empty;

                //    strSearch = "OBS_NU = '" + txtOBSNU.Text + "'";

                //    if (cboITEM.EditValue != null)
                //    {
                //        SetLookUp(cboITEM, "", "L_SMG_LINE_S", strSearch);
                //    }
                //    else
                //    {
                //        cboITEM.SelectedIndex = 0;

                //        SetLookUp(cboITEM, "", "L_SMG_LINE_S", strSearch);
                //    }
                //}
                //if (e.Column.FieldName.Equals("ITEM") && radioButton2.Checked == true)
                //{
                //    string V_OBS_SEQ_NO = gvwBase_Detail.GetRowCellDisplayText(e.RowHandle, "ITEM").Trim();

                //    string strSearch = string.Empty;

                //    strSearch = "OBS_NU = '" + txtOBSNU.Text + "'";

                //    if (cboITEM.EditValue != null)
                //    {
                //        SetLookUp(cboITEM, "", "L_SMG_LINE_S", strSearch);
                //    }
                //    else
                //    {
                //        cboITEM.SelectedIndex = 0;

                //        SetLookUp(cboITEM, "", "L_SMG_LINE_S", strSearch);
                //    }
                //}
                //else
                //{
                //    cboITEM.Reset();
                //}
            }
            catch (Exception ex)
            {
                this.MessageBoxW("gvwBase_Detail_RowClick: " + ex.Message);
            }

        }

        private void cboITEM_EditValueChanged(object sender, EventArgs e)
        {
            //string str_obs_nu = txtOBSNU.Text;
            //string str_item = cboITEM.EditValue.ToString();

            //SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            //DataTable dtData = null;
            //dtData = cProc.SetParamData(dtData,
            //                "Q2",
            //                str_obs_nu,
            //                str_item
            //                );
            //ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            //if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            //{

            //    DataTable dtSource = rs.ResultDataSet.Tables[0];
            //    //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
            //    txtLineCD.Text = dtSource.Rows[0][0].ToString();
            //}
            //SetLookUp(cboChangeTo, "", "L_SMG_LINE_S2", "");
        }

        private void cboPlant_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtOBSNU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetLookUp(cboSeq, "", "L_COM001108_1", "OBS_NU='" + txtOBSNU.Text + "'");

                txtOA.Text = fnMSR_OBS_OA_CHECK();
                fnQRY_P_GMES00108_Q_HEAD("Q_H");
                fnQRY_P_GMES00108_Q_InputBefore();
                init_Set_Form_Auth();
            }
        }

        private void cboSeq_EditValueChanged(object sender, EventArgs e)
        {
            fnQRY_P_GMES00108_Q("Q");

            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false)
            {
                if (txtOBSNU.Text.Trim().Equals(""))
                {
                    init_Set_Form_Auth();
                    this.MessageBoxW("You have to input PO number");
                    return;
                }
            }
            txt_TotCartQty.Text = "";
            txtInputBefore.Text = "";

            txtOA.Text = fnMSR_OBS_OA_CHECK();
            fnQRY_P_GMES00108_Q_HEAD("Q_H");
            fnQRY_P_GMES00108_Q_InputBefore();

            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false)
            {
                fnQRY_P_GMES00108_Q_DETAIL_ORDER("Q_D");
                fnQRY_P_GMES00108_Q_DETAIL_TOT_IN("Q_T");
                fnQRY_P_GMES00108_Q_DETAIL_INPUT("Q_I");
            }

            int data_cnt = ((DataTable)gridControlEx01.DataSource).Rows.Count;

            if (data_cnt > 0)
            {
                SaveButton = true;
                cboSeq.Focus();

            }
            else
            {
                //SaveButton = false;
                DeleteButton = false;
            }
            init_Set_Form_Auth();
        }

        private bool fnQRY_P_GMES00108_Q_IDPW()
        {
            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_C", strTempParam[0],
                            txtOBSNU.Text,
                            strTempParam[1], //cboSeq.EditValue.ToString(),
                            strTempParam[2], //cboPlant.EditValue.ToString(),
                            strTempParam[3], //cboInOut.EditValue.ToString(),
                            txtUserID.Text,
                            txtUserPW.Text
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                string str_user_chk = dtSource.Rows[0][0].ToString();

                init_Set_Form_Auth();

                if (str_user_chk.Equals("0"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private bool fnQRY_P_GMES00108_Q_HEAD(string strWorkType)
        {
            try
            {
                while (gridViewEx01.Columns.Count > 0)
                {
                    gridViewEx01.Columns.RemoveAt(0);
                }
                string[] strTempParam = new string[10];
                for (int kk = 0; kk < strTempParam.Length; kk++)
                {
                    strTempParam[kk] = "";
                }
                strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
                if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
                if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, strWorkType, strTempParam[0],
                                txtOBSNU.Text,
                                strTempParam[1], //cboSeq.EditValue.ToString(),
                                strTempParam[2], //cboPlant.EditValue.ToString(),
                                strTempParam[3], //cboInOut.EditValue.ToString(),
                                txtUserID.Text,
                                txtUserPW.Text
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, gridControlEx01);
                //gridViewEx2.BestFitColumns();
                gridViewEx01.OptionsView.ColumnAutoWidth = false;

                int[] col_width = new int[] { 120, 120, 90, 280, 120, 120, 120 };
                for (int i = 0; i < gridViewEx01.Columns.Count; i++)
                {
                    gridViewEx01.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gridViewEx01.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
                    gridViewEx01.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx01.Columns[i].OptionsColumn.ReadOnly = true;
                    gridViewEx01.Columns[i].OptionsColumn.AllowEdit = false;
                    if (i < col_width.Length)
                    {
                        gridViewEx01.Columns[i].Width = col_width[i];
                    }
                    {
                        gridViewEx01.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }
                    if (i == 5 || i == 6)
                    {
                        gridViewEx01.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        gridViewEx01.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gridViewEx01.Columns[i].DisplayFormat.FormatString = "#,#.#";

                        //gridViewEx2.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                        //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gridViewEx2.Columns[i].FieldName, "{0:n0}")});
                    }
                    //if (i == 0)
                    //{
                    //    gridViewEx2.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                    //        new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, gridViewEx2.Columns[i].FieldName, "{0:n0} Rows", gridViewEx2.Columns[i].FieldName)});
                    //}
                    //if (i == 0 || i == 1 || i == 2)
                    //{
                    //    gridViewEx2.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                    //}
                    //else
                    //{
                    //    gridViewEx2.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    //}
                }
                gridControlEx01.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q_HEAD(): " + ex.Message);
                return false;
            }
            return true;
        }
        //private bool fnQRY_P_GMES00108_Q_DETAIL(string strWorkType)
        //{
        //    try
        //    {
        //        while (gvwBase_Detail.Columns.Count > 0)
        //        {
        //            gvwBase_Detail.Columns.RemoveAt(0);
        //        }
        //        string[] strTempParam = new string[10];
        //        for (int kk = 0; kk < strTempParam.Length; kk++)
        //        {
        //            strTempParam[kk] = "";
        //        }
        //        strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
        //        if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
        //        if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
        //        if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

        //        SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
        //        DataTable dtData = null;
        //        dtData = cProc.SetParamData(dtData, strWorkType, strTempParam[0],
        //                        txtOBSNU.Text,
        //                        strTempParam[1], //cboSeq.EditValue.ToString(),
        //                        strTempParam[2], //cboPlant.EditValue.ToString(),
        //                        strTempParam[3], //cboInOut.EditValue.ToString(),
        //                        txtUserID.Text,
        //                        txtUserPW.Text
        //                        );
        //        CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, grdBase_Detail);
        //        //gvwBase_Detail.BestFitColumns();
        //        gvwBase_Detail.OptionsView.ColumnAutoWidth = false;

        //        int[] col_width = new int[] { 120, 120, 90, 280, 120, 120, 120 };
        //        for (int i = 0; i < gvwBase_Detail.Columns.Count; i++)
        //        {
        //            //gvwBase_Detail.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gvwBase_Detail.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
        //            //gvwBase_Detail.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            //gvwBase_Detail.Columns[i].OptionsColumn.ReadOnly = true;
        //            if (i < col_width.Length)
        //            {
        //                gvwBase_Detail.Columns[i].Width = 60;
        //            }
        //            {
        //                gvwBase_Detail.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            }
        //            if (i > 0)
        //            {
        //                gvwBase_Detail.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        //                gvwBase_Detail.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        //                //gvwBase_Detail.Columns[i].DisplayFormat.FormatString = "#,#";

        //                //gvwBase_Detail.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
        //                //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gvwBase_Detail.Columns[i].FieldName, "{0:n0}")});
        //            }
        //        }
        //        grdBase_Detail.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.MessageBoxW("fnQRY_P_GMES00108_Q_DETAIL(): " + ex.Message);
        //        return false;
        //    }
        //    return true;
        //}
        private bool fnQRY_P_GMES00108_Q_Grid3(string strWorkType)
        {
            try
            {
                while (gridViewEx03.Columns.Count > 0)
                {
                    gridViewEx03.Columns.RemoveAt(0);
                }
                string[] strTempParam = new string[10];
                for (int kk = 0; kk < strTempParam.Length; kk++)
                {
                    strTempParam[kk] = "";
                }
                strTempParam[0] = cboDate1.DateTime.ToString("yyyyMMdd");
                if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[1] = cboSeq.EditValue.ToString();
                if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[2] = cboPlant.EditValue.ToString();
                if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[3] = cboInOut.EditValue.ToString();

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, strWorkType, strTempParam[0],
                                txtOBSNU.Text,
                                strTempParam[1], //cboSeq.EditValue.ToString(),
                                strTempParam[2], //cboPlant.EditValue.ToString(),
                                strTempParam[3], //cboInOut.EditValue.ToString(),
                                txtUserID.Text,
                                txtUserPW.Text
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, gridControlEx03);
                //gridViewEx5.BestFitColumns();
                gridViewEx03.OptionsView.ColumnAutoWidth = false;

                int[] col_width = new int[] { 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45, 45 };
                for (int i = 0; i < gridViewEx03.Columns.Count; i++)
                {
                    //gridViewEx5.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gridViewEx5.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
                    gridViewEx03.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //gridViewEx03.Columns[i].OptionsColumn.ReadOnly = true;
                    //gridViewEx03.Columns[i].OptionsColumn.AllowEdit = false;
                    if (i < col_width.Length)
                    {
                        gridViewEx03.Columns[i].Width = col_width[i];
                    }
                    //if (i == 2)
                    //{
                    //    gridViewEx5.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    //}
                    //else
                    //{
                    //    gridViewEx5.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //}
                    //if (i == 5 || i == 6)
                    //{
                        gridViewEx03.Columns[i].OptionsColumn.ReadOnly = false;
                        gridViewEx03.Columns[i].OptionsColumn.AllowEdit = true;
                        gridViewEx03.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        gridViewEx03.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gridViewEx03.Columns[i].DisplayFormat.FormatString = "#,#.#";


                        //gridViewEx5.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                        //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gridViewEx5.Columns[i].FieldName, "{0:n0}")});
                    //}
                    //if (i == 0)
                    //{
                    //    gridViewEx5.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                    //        new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, gridViewEx5.Columns[i].FieldName, "{0:n0} Rows", gridViewEx5.Columns[i].FieldName)});
                    //}
                    //if (i == 0 || i == 1 || i == 2)
                    //{
                    //    gridViewEx5.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                    //}
                    //else
                    //{
                    //    gridViewEx5.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    //}
                }
                //gridControlEx03.EndUpdate();
                gridControlEx03.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q_Grid3(): " + ex.Message);
                return false;
            }
            return true;
        }

        private bool fnQRY_P_GMES00108_Q_Grid4(string strWorkType)
        {
            try
            {
                while (gridViewEx04.Columns.Count > 0)
                {
                    gridViewEx04.Columns.RemoveAt(0);
                }

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, strWorkType, cboDate1.DateTime.ToString("yyyyMMdd"),
                                txtOBSNU.Text,
                                cboSeq.EditValue == null ? "" : cboSeq.EditValue.ToString(),
                                "", //cboPlant.EditValue.ToString(),
                                "", //cboInOut.EditValue.ToString(),
                                "", //txtUserID.Text,
                                "" //txtUserPW.Text,
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, gridControlEx04);
                //gridViewEx04.BestFitColumns();
                gridViewEx04.OptionsView.ColumnAutoWidth = false;

                int[] col_width = new int[] { 150, 150, 150 };
                for (int i = 0; i < gridViewEx04.Columns.Count; i++)
                {
                    gridViewEx04.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gridViewEx04.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
                    gridViewEx04.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx04.Columns[i].OptionsColumn.ReadOnly = true;
                    gridViewEx04.Columns[i].OptionsColumn.AllowEdit = false;

                    if (i < col_width.Length)
                    {
                        gridViewEx04.Columns[i].Width = col_width[i];
                    }
                    //if (i == 2)
                    //{
                    //    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    //}
                    //else
                    //{
                    //    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //}
                    if (i == 2)
                    {
                        gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        gridViewEx04.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gridViewEx04.Columns[i].DisplayFormat.FormatString = "#,#.#";

                        //gridViewEx04.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                        //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gridViewEx04.Columns[i].FieldName, "{0:n0}")});
                    }
                }

                gridControlEx04.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q_Grid4(): " + ex.Message);
                return false;
            }
            return true;
        }


        private bool fnQRY_P_GMES00108_Q_COMPONENT(string strWorkType)
        {
            try
            {
                while (gridViewEx04.Columns.Count > 0)
                {
                    gridViewEx04.Columns.RemoveAt(0);
                }
                string strGradeS = cboGradeS.EditValue.ToString();

                if (strGradeS.Equals("CFS"))
                    strGradeS = "112";
                else if (strGradeS.Equals("CUP"))
                    strGradeS = "113";
                else if (strGradeS.Equals("CSL"))
                    strGradeS = "114";
                else if (strGradeS.Equals("CQD"))
                    strGradeS = "115";

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, strWorkType, cboDate1.DateTime.ToString("yyyyMMdd"),
                                "",
                                "",
                                strGradeS, //cboPlant.EditValue.ToString(),
                                "", //cboInOut.EditValue.ToString(),
                                "", //txtUserID.Text,
                                "" //txtUserPW.Text,
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, gridControlEx04);
                //gridViewEx04.BestFitColumns();
                gridViewEx04.OptionsView.ColumnAutoWidth = false;

                int[] col_width = new int[] { 150, 150, 150 };
                for (int i = 0; i < gridViewEx04.Columns.Count; i++)
                {
                    gridViewEx04.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gridViewEx04.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
                    gridViewEx04.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx04.Columns[i].OptionsColumn.ReadOnly = true;
                    gridViewEx04.Columns[i].OptionsColumn.AllowEdit = false;

                    if (i < col_width.Length)
                    {
                        gridViewEx04.Columns[i].Width = col_width[i];
                    }
                    //if (i == 2)
                    //{
                    //    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    //}
                    //else
                    //{
                    //    gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //}
                    if (i == 2)
                    {
                        gridViewEx04.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        gridViewEx04.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gridViewEx04.Columns[i].DisplayFormat.FormatString = "#,#.#";

                        //gridViewEx04.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                        //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gridViewEx04.Columns[i].FieldName, "{0:n0}")});
                    }
                }

                gridControlEx04.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q_COMPONENT(): " + ex.Message);
                return false;
            }
            return true;
        }
        // 1 번째 그리드, 콤포넌트 관련
        private bool fnQRY_P_GMES00108_Q_Grid1()
        {
            try
            {
                while (gridViewEx01.Columns.Count > 0)
                {
                    gridViewEx01.Columns.RemoveAt(0);
                }

                SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData, "Q1", cboDate1.DateTime.ToString("yyyyMMdd"),
                                "", // txtOBSNU.Text,
                                "", // cboSeq.EditValue.ToString(),
                                "", // cboPlant.EditValue.ToString(),
                                "", // cboInOut.EditValue.ToString(),
                                "", // txtUserID.Text,
                                ""  // txtUserPW.Text,
                                );
                CommonProcessQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true, gridControlEx01);
                //gridViewEx2.BestFitColumns();
                gridViewEx01.OptionsView.ColumnAutoWidth = false;

                int[] col_width = new int[] { 120, 120, 90, 280, 120, 120, 120 };
                for (int i = 0; i < gridViewEx01.Columns.Count; i++)
                {
                    gridViewEx01.Columns[i].Caption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gridViewEx01.Columns[i].GetCaption().Replace("_", " ")); //.ToLower()
                    gridViewEx01.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridViewEx01.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                    gridViewEx01.Columns[i].OptionsColumn.ReadOnly = true;
                    gridViewEx01.Columns[i].OptionsColumn.AllowEdit = false;
                    
                    if (i < col_width.Length)
                    {
                        gridViewEx01.Columns[i].Width = col_width[i];
                    }
                    if (i == 5 || i == 6)
                    {
                        gridViewEx01.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        gridViewEx01.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gridViewEx01.Columns[i].DisplayFormat.FormatString = "#,#.#";

                        //gridViewEx2.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                        //    new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, gridViewEx2.Columns[i].FieldName, "{0:n0}")});
                    }
                    //if (i == 0)
                    //{
                    //    gridViewEx2.Columns[i].Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                    //        new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, gridViewEx2.Columns[i].FieldName, "{0:n0} Rows", gridViewEx2.Columns[i].FieldName)});
                    //}
                    //if (i == 0 || i == 1 || i == 2)
                    //{
                    //    gridViewEx2.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                    //}
                    //else
                    //{
                    //    gridViewEx2.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    //}
                }

                gridControlEx01.Refresh();
            }
            catch (Exception ex)
            {
                this.MessageBoxW("fnQRY_P_GMES00108_Q_Grid1(): " + ex.Message);
                return false;
            }
            return true;
        }
        private bool fnQRY_P_GMES00108_Q_InputBefore()
        {
            txtInputBefore.Text = "";
            if (txtOBSNU.Text.Equals("")) return false;

            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[0] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[1] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[2] = cboInOut.EditValue.ToString();

            if (strTempParam[0] == "" || strTempParam[1] == "" || strTempParam[2] == "") return false;

            // ####################################################################################
            int intPlant = Convert.ToInt32(cboPlant.EditValue.ToString());

            if (intPlant >= 300 && intPlant < 401)
            {
                strTempParam[2] = "B";
                if (strTempParam[1].Equals("300")) strTempParam[1] = "011";
                else if (strTempParam[1].Equals("301")) strTempParam[1] = "012";
                else if (strTempParam[1].Equals("302")) strTempParam[1] = "099";
                else if (strTempParam[1].Equals("303")) strTempParam[1] = "013";
                else if (strTempParam[1].Equals("304")) strTempParam[1] = "014";
                else if (strTempParam[1].Equals("305")) strTempParam[1] = "015";
                else if (strTempParam[1].Equals("306")) strTempParam[1] = "016";
                else if (strTempParam[1].Equals("307")) strTempParam[1] = "007";
                else if (strTempParam[1].Equals("308")) strTempParam[1] = "008";
                else if (strTempParam[1].Equals("309")) strTempParam[1] = "009";
                else if (strTempParam[1].Equals("310")) strTempParam[1] = "010";
                else if (strTempParam[1].Equals("311")) strTempParam[1] = "017";
                else if (strTempParam[1].Equals("400")) strTempParam[1] = "000";
            }

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q6", cboDate1.DateTime.ToString("yyyyMMdd"),
                            txtOBSNU.Text,
                            strTempParam[0], // cboSeq.EditValue.ToString(),
                            strTempParam[1], //cboPlant.EditValue.ToString(),
                            strTempParam[2], //cboInOut.EditValue.ToString(),
                            txtUserID.Text,
                            txtUserPW.Text
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                txtInputBefore.Text = dtSource.Rows[0][0].ToString();
            }
            return true;
        }

        private bool fnQRY_P_GMES00108_Q_DETAIL_ORDER(string strWorkType)
        {
            for (int kk = 0; kk < strGridOrderSize.Length; kk++)
            {
                strGridOrderSize[kk] = "";
            }
            for (int kk = 1; kk < (gvwBase_Detail.Columns.Count - 1); kk++)
            {
                gvwBase_Detail.Bands[kk].Caption = "";
                gvwBase_Detail.Bands[kk].Children[0].Caption = "";
            }
            string[] strHead = new string[] { "Total In", "Total Out", "Total" };
            if (cboInOut.EditValue.ToString().Equals("I"))
                gvwBase_Detail.Bands[0].Children[0].Children[0].Caption = strHead[0];
            else if (cboInOut.EditValue.ToString().Equals("O"))
                gvwBase_Detail.Bands[0].Children[0].Children[0].Caption = strHead[1];
            else
                gvwBase_Detail.Bands[0].Children[0].Children[0].Caption = strHead[2];
            gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Caption = "0";
            gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            if (cboPlant.EditValue == null) return false;
            if (cboSeq.EditValue == null) return false;
            if (cboInOut.EditValue == null) return false;
            if (txtOBSNU.Text.ToString().Equals("")) return false;

            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[0] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[1] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[2] = cboInOut.EditValue.ToString();

            if (strTempParam[0] == "" || strTempParam[1] == "" || strTempParam[2] == "") return false;

            // ####################################################################################

            int intPlant = Convert.ToInt32(cboPlant.EditValue.ToString());

            //if (intPlant >= 300 && intPlant < 401)
            //{
            //    strTempParam[2] = "B";
            //    if (strTempParam[1].Equals("300")) strTempParam[1] = "011";
            //    else if (strTempParam[1].Equals("301")) strTempParam[1] = "012";
            //    else if (strTempParam[1].Equals("302")) strTempParam[1] = "099";
            //    else if (strTempParam[1].Equals("303")) strTempParam[1] = "013";
            //    else if (strTempParam[1].Equals("304")) strTempParam[1] = "014";
            //    else if (strTempParam[1].Equals("305")) strTempParam[1] = "015";
            //    else if (strTempParam[1].Equals("306")) strTempParam[1] = "016";
            //    else if (strTempParam[1].Equals("307")) strTempParam[1] = "007";
            //    else if (strTempParam[1].Equals("308")) strTempParam[1] = "008";
            //    else if (strTempParam[1].Equals("309")) strTempParam[1] = "009";
            //    else if (strTempParam[1].Equals("310")) strTempParam[1] = "010";
            //    else if (strTempParam[1].Equals("311")) strTempParam[1] = "017";
            //    else if (strTempParam[1].Equals("400")) strTempParam[1] = "000";
            //}

            int totOrder = 0;
            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, strWorkType, cboDate1.DateTime.ToString("yyyyMMdd"),
                            txtOBSNU.Text,
                            strTempParam[0], // cboSeq.EditValue.ToString(),
                            strTempParam[1], //cboPlant.EditValue.ToString(),
                            strTempParam[2], //cboInOut.EditValue.ToString(),
                            txtUserID.Text,
                            txtUserPW.Text
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                // CS_SIZE, SIZE_NUM, ORD_QTY
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        string strSize = dtSource.Rows[i][0].ToString();
                        string strOrderCnt = dtSource.Rows[i][2].ToString();
                        totOrder += Convert.ToInt32(strOrderCnt);
                        //this.MessageBoxW(strSize + " _ " + strOrderCnt);
                        gvwBase_Detail.Bands[i + 1].Caption = strSize;
                        gvwBase_Detail.Bands[i + 1].Children[0].Caption = strOrderCnt;
                        strGridOrderSize[i + 1] = strSize;

                        gvwBase_Detail.Bands[i + 1].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);
                        gvwBase_Detail.Bands[i + 1].Children[0].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);
                        gvwBase_Detail.Bands[i + 1].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gvwBase_Detail.Bands[i + 1].Children[0].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }
                }
                gvwBase_Detail.Bands["gridBandTotal1"].Caption = "Total";
                gvwBase_Detail.Bands["gridBandTotal1"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Caption = totOrder.ToString("#,##0");
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                gvwBase_Detail.Bands["gridBandTotal1"].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);

            }
            grdBase_Detail.EndUpdate();

            // txtOA check
            DataTable dtOrderq = (DataTable)gridControlEx01.DataSource;
            if (dtOrderq != null)
            {
                if (dtOrderq.Rows.Count > 0)
                {
                    int val_CarQty = Convert.ToInt32(dtOrderq.Rows[0]["Prs_Q'ty"].ToString().Replace(",", ""));
                    if (val_CarQty != totOrder)
                    {
                        txtOA.Text = "OA";
                        //this.MessageBoxW(val_CarQty.ToString() + " : " + totOrder.ToString());
                    }
                }
            }

            return true;
        }

        private bool fnQRY_P_GMES00108_Q_DETAIL_TOT_IN(string strWorkType)
        {
            for (int kk = 1; kk < (gvwBase_Detail.Columns.Count - 1); kk++ )
            {
                gvwBase_Detail.Bands[kk].Children[0].Children[0].Caption = "";
            }
            gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Children["gridBandTotal3"].Caption = "0";
            gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Children["gridBandTotal3"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            if (cboPlant.EditValue == null) return false;
            if (cboSeq.EditValue == null) return false;
            if (cboInOut.EditValue == null) return false;
            if (txtOBSNU.Text.ToString().Equals("")) return false;

            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[0] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[1] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[2] = cboInOut.EditValue.ToString();

            if (strTempParam[0] == "" || strTempParam[1] == "" || strTempParam[2] == "") return false;

            // ####################################################################################

            int intPlant = Convert.ToInt32(cboPlant.EditValue.ToString());

            //if (intPlant >= 300 && intPlant < 401)
            //{
            //    strTempParam[2] = "B";
            //    if (strTempParam[1].Equals("300")) strTempParam[1] = "011";
            //    else if (strTempParam[1].Equals("301")) strTempParam[1] = "012";
            //    else if (strTempParam[1].Equals("302")) strTempParam[1] = "099";
            //    else if (strTempParam[1].Equals("303")) strTempParam[1] = "013";
            //    else if (strTempParam[1].Equals("304")) strTempParam[1] = "014";
            //    else if (strTempParam[1].Equals("305")) strTempParam[1] = "015";
            //    else if (strTempParam[1].Equals("306")) strTempParam[1] = "016";
            //    else if (strTempParam[1].Equals("307")) strTempParam[1] = "007";
            //    else if (strTempParam[1].Equals("308")) strTempParam[1] = "008";
            //    else if (strTempParam[1].Equals("309")) strTempParam[1] = "009";
            //    else if (strTempParam[1].Equals("310")) strTempParam[1] = "010";
            //    else if (strTempParam[1].Equals("311")) strTempParam[1] = "017";
            //    else if (strTempParam[1].Equals("400")) strTempParam[1] = "000";
            //}

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_T", cboDate1.DateTime.ToString("yyyyMMdd"),
                            txtOBSNU.Text,
                            strTempParam[0], // cboSeq.EditValue.ToString(),
                            strTempParam[1], //cboPlant.EditValue.ToString(),
                            strTempParam[2], //cboInOut.EditValue.ToString(),
                            txtUserID.Text,
                            txtUserPW.Text
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                // CS_SIZE, SIZE_NUM, ORD_QTY
                int totOrder = 0;
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        string strSize = dtSource.Rows[i][0].ToString();
                        string strOrderCnt = dtSource.Rows[i][2].ToString();
                        totOrder += Convert.ToInt32(strOrderCnt);

                        for (int kk = 1; kk < (gvwBase_Detail.Columns.Count - 1); kk++)
                        {
                            if (gvwBase_Detail.Bands[kk].Caption.ToString().Equals(strSize))
                            {
                                //this.MessageBoxW(strSize + " _ " + strOrderCnt);
                                gvwBase_Detail.Bands[kk].Children[0].Children[0].Caption = strOrderCnt;
                                gvwBase_Detail.Bands[kk].Children[0].Children[0].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);
                                gvwBase_Detail.Bands[kk].Children[0].Children[0].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            }
                        }
                    }
                }
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Children["gridBandTotal3"].Caption = totOrder.ToString("#,##0");
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Children["gridBandTotal3"].AppearanceHeader.Font = new Font("Calibri", 12F, FontStyle.Regular);
                gvwBase_Detail.Bands["gridBandTotal1"].Children["gridBandTotal2"].Children["gridBandTotal3"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            return true;
        }

        private bool fnQRY_P_GMES00108_Q_DETAIL_INPUT(string strWorkType)
        {
            if (cboPlant.EditValue == null) return false;
            if (cboSeq.EditValue == null) return false;
            if (cboInOut.EditValue == null) return false;
            if (txtOBSNU.Text.ToString().Equals("")) return false;

            string[] strTempParam = new string[10];
            for (int kk = 0; kk < strTempParam.Length; kk++)
            {
                strTempParam[kk] = "";
            }
            if (cboSeq.EditValue != null && cboSeq.EditValue.ToString().Equals("") == false) strTempParam[0] = cboSeq.EditValue.ToString();
            if (cboPlant.EditValue != null && cboPlant.EditValue.ToString().Equals("") == false) strTempParam[1] = cboPlant.EditValue.ToString();
            if (cboInOut.EditValue != null && cboInOut.EditValue.ToString().Equals("") == false) strTempParam[2] = cboInOut.EditValue.ToString();

            if (strTempParam[0] == "" || strTempParam[1] == "" || strTempParam[2] == "") return false;

            // ####################################################################################

            int intPlant = Convert.ToInt32(cboPlant.EditValue.ToString());

            SP_GMES00108_1_Q cProc = new SP_GMES00108_1_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "Q_I", cboDate1.DateTime.ToString("yyyyMMdd"),
                            txtOBSNU.Text,
                            strTempParam[0], // cboSeq.EditValue.ToString(),
                            strTempParam[1], //cboPlant.EditValue.ToString(),
                            strTempParam[2], //cboInOut.EditValue.ToString(),
                            txtUserID.Text,
                            txtUserPW.Text
                            );
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                // #############################
                DataTable dtDataDisp = new DataTable();
                dtDataDisp.Columns.Add("SIZE_01", typeof(string));
                for (int i = 2; i <= 36; i++)
                {
                    dtDataDisp.Columns.Add(string.Format("SIZE_{0:00}", i), typeof(string));
                }

                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                // CS_SIZE, SIZE_NUM, ORD_QTY
                int totOrder = 0;
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    DataRow dr = dtDataDisp.NewRow();
                    dr["SIZE_01"] = "Input";

                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        string strSize = dtSource.Rows[i][0].ToString();
                        string strOrderCnt = dtSource.Rows[i][2].ToString();
                        totOrder += Convert.ToInt32(strOrderCnt);

                        for (int kk = 1; kk < (strGridOrderSize.Length - 2); kk++)
                        {
                           
                               
                            
                            //pHUOC MODIFIED;
                            //if (strGridOrderSize[kk].ToString().Equals(strSize))
                            //{
                            //   // dr[string.Format("SIZE_{0:00}", kk + 1)] = Convert.ToDouble(strOrderCnt) ;
                            //    dr[string.Format("SIZE_{0:00}", kk + 1)] = 0 ;
                            //}
                            //else
                            //{
                            if (kk < 36)
                            {
                                dr[string.Format("SIZE_{0:00}", kk + 1)] = "";

                                //string col_val2 = gvwBase_Detail.Bands[kk].Children[0].Caption;
                                //string col_val3 = gvwBase_Detail.Bands[kk].Children[0].Children[0].Caption;
                                //if (col_val3.Equals(col_val2))
                                //    gvwBase_Detail.Columns[kk].OptionsColumn.AllowEdit = false;
                            }
                            //}
                        }
                    }
                   // dr["SIZE_36"] = null;//totOrder;
                    dtDataDisp.Rows.Add(dr);

                  
                }
                dtDataDisp.AcceptChanges();
                //grdBase_Detail.DataSource = dtDataDisp;
                SetData(grdBase_Detail, dtDataDisp);
            }

            return true;
        }

        private void cboGradeS_EditValueChanged(object sender, EventArgs e)
        {
            if (cboGradeS.EditValue == null) return;
            if (cboGradeS.EditValue.ToString().Equals("")) return;

            string strGradeS = "";
            string strLineS = "";

            if (cboGradeS.EditValue != null) strGradeS = cboGradeS.EditValue.ToString();

            if (strGradeS.Equals("CFS") || strGradeS.Equals("CUP") || strGradeS.Equals("CSL") || strGradeS.Equals("CQD"))
            {
                label13.Visible = true;
                cboPO.Visible = true;
                lblStyleName.Visible = true;
                cboStyle.Visible = true;
                lblMline.Visible = true;
                cboMline.Visible = true;
                lblMline.Enabled = true;
                cboMline.Enabled = true;
                txtStyleCD.Visible = false;
                cboDate2.EditValue = DateTime.Now;
                cboDate2.Enabled = false;
            }
            else
            {
                label13.Visible = false;
                cboPO.Visible = false;
                lblMline.Visible = false;
                cboMline.Visible = false;
                lblMline.Enabled = false;
                cboMline.Enabled = false;
                lblStyleName.Visible = true;
                txtStyleCD.Visible = true;
                cboStyle.Visible = false;
                cboDate2.Enabled = true;
            }
            //lblStyleName.Text = "";
            if (strGradeS.Equals("CFS")) 
                strLineS = "112";
            else if (strGradeS.Equals("CUP")) 
                strLineS = "113";
            else if (strGradeS.Equals("CSL")) 
                strLineS = "114";
            else if (strGradeS.Equals("CQD"))
                strLineS = "115";

            //SetLookUp(cboPO, "", "L_COM001108_3", string.Format("LINE_CD IN ('{0}')", strLineS));
            SetLookUp(cboPO, "", "L_COM0108_1", "");
        }

        private void chkID_B_Overrun_CheckedChanged(object sender, EventArgs e)
        {
            loadControl_Set();
        }

        private void cboPO_EditValueChanged(object sender, EventArgs e)
        {
            if (cboGradeS.EditValue == null) return;
            if (cboGradeS.EditValue.ToString().Equals("")) return;
            if (cboPO.EditValue == null) return;
            if (cboPO.EditValue.ToString().Equals("")) return;

            //if (cboGradeS.EditValue.ToString().Equals("Component-B"))
            //{
            //    SetLookUp(cboStyle, "", "L_COM001108_4", "OBS_ID = '" + cboPO.EditValue.ToString() + "'");
            //} else if (cboGradeS.EditValue.ToString().Equals("Component-U")) {
            //    SetLookUp(cboStyle, "", "L_COM001108_5", "OBS_ID = '" + cboPO.EditValue.ToString() + "'");
            //}

            string strGradeS = "";
            string strLineS = "";
            string strPO = "";

            if (cboGradeS.EditValue != null) strGradeS = cboGradeS.EditValue.ToString();
            if (cboPO.EditValue != null) strPO = cboPO.EditValue.ToString();

            if (strGradeS.Equals("CFS"))
                strLineS = "112";
            else if (strGradeS.Equals("CUP"))
                strLineS = "113";
            else if (strGradeS.Equals("CSL"))
                strLineS = "114";
            else if (strGradeS.Equals("CQD"))
                strLineS = "115";

            SetLookUp(cboStyle, "", "L_COM0108_2", string.Format("SUBSTR(PO_NO,3,6) = '{0}'", strPO));
        }

        private void cboPlant_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            int intPlant = 0;

            if (cboPlant.EditValue.ToString().Equals("") == false)
            {
                intPlant = Convert.ToInt32(cboPlant.EditValue.ToString());

                if (intPlant >= 300 && intPlant < 401)
                {
                    SetLookUp(cboInOut, "", "L_COM_MAIN_1", "");
                    //SetLookUp(cboLocation, "", "L_COM_MAIN_3", "LINE_CD='"+ cboPlant.EditValue.ToString() + "'");
                    SetLookUp(cboLocation, "", "L_COM_MAIN_3", "LINE_CD='401'");
                }
                else
                {
                    SetLookUp(cboInOut, "", "L_COM_MAIN_2", "");
                    SetLookUp(cboLocation, "", "L_COM_MAIN_3", "LINE_CD='" + cboPlant.EditValue.ToString() + "'");
                }
            }
        }

        private void gvwBase_Detail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Value.ToString() == "0")
            //{
            //    gvwBase_Detail.SetRowCellValue(e.RowHandle, gvwBase_Detail.Columns[e.Column.ColumnHandle], null);
            //}
            
            int strVal = Convert.ToInt32(e.Value.ToString());

            if (strVal.ToString().Length > 5)
            {
                this.MessageBoxW("Input value is longer 5 number !!");
                gvwBase_Detail.CancelUpdateCurrentRow();

            } else if (strVal < 1)
            {
                int colPos = Convert.ToInt32(e.Column.Name.Substring(5, 2)) - 1;
                string col_val3 = gvwBase_Detail.Bands[colPos].Children[0].Children[0].Caption;
                if (col_val3.Equals("")) col_val3 = "0";

                int strTotal = Convert.ToInt32(col_val3);
                if ((strTotal + strVal) < 0)
                {
                    this.MessageBoxW("Can't input over order q'ty");
                    gvwBase_Detail.CancelUpdateCurrentRow();
                }
                //this.MessageBoxW("Input value is wrong !!");
                //gvwBase_Detail.CancelUpdateCurrentRow();
            }
            else
            {
                int colPos = Convert.ToInt32(e.Column.Name.Substring(5, 2)) - 1;
                //this.MessageBoxW(gvwBase_Detail.Bands[colPos].Children[0].Caption);
                //this.MessageBoxW(gvwBase_Detail.Bands[i + 1].Children[0].Children[0].Caption);

                string col_val2 = gvwBase_Detail.Bands[colPos].Children[0].Caption;
                string col_val3 = gvwBase_Detail.Bands[colPos].Children[0].Children[0].Caption;
                if (col_val3.Equals("")) col_val3 = "0";
                if (col_val2.Equals(""))
                {
                    this.MessageBoxW("Input value is wrong !!");
                    gvwBase_Detail.CancelUpdateCurrentRow();
                    return;
                }
                int strOrder = Convert.ToInt32(col_val2);
                int strTotal = Convert.ToInt32(col_val3);
                if (strOrder < (strTotal + strVal))
                {
                    this.MessageBoxW("Can't input over order q'ty");
                    gvwBase_Detail.CancelUpdateCurrentRow();
                }
            }
            //e.Column.OptionsColumn.AllowEdit = false;

            int data_cnt = 0;
            DataTable dtModified = (DataTable)grdBase_Detail.DataSource;
            if (dtModified != null)
            {
                if (dtModified.Rows.Count > 0)
                {
                    for (int i = 0; i <= dtModified.Rows.Count - 1; i++)
                    {
                        for (int j = 1; j < gvwBase_Detail.Columns.Count - 1; j++)
                        {
                            if (dtModified.Rows[i][j].ToString().Equals("0") == false && dtModified.Rows[i][j].ToString() != "")
                            {
                                data_cnt += Convert.ToInt32(dtModified.Rows[i][j].ToString());
                            }
                        }
                        dtModified.Rows[i][35] = data_cnt;
                    }
                    dtModified.AcceptChanges();
                    SetData(grdBase_Detail, dtModified);
                }
            }
            SaveButton = true;
        }

        private void txtStyleCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fnSMW_Get_StyleName();
            }
        }

        private void cboStyle_EditValueChanged(object sender, EventArgs e)
        {
            if (cboStyle.EditValue == null) return;
             
            fnSMW_Get_StyleName();
            init_Set_Form_Auth(); 
        }

        private void gvwBase_Detail_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
        }

        private void init_Set_Form_Auth()
        {
            string strFormFileName = "CSI.MES.P.GMES00108"; 

            if (SessionInfo.UserID.Equals("admin"))
            {
                AddButton = true;
                FormAccessInfo.AllowSave = SaveButton = true;
                DeleteRowButton = true;
                FormAccessInfo.AllowDelete = DeleteButton = true; 
                return;
            }

            P_SYS1200_ID_Q cProc = new P_SYS1200_ID_Q();
            DataTable dtData = null;
            dtData = cProc.SetParamData(dtData, "AUTHORITY", strFormFileName, SessionInfo.UserID);
            ResultSet rs = CommonCallQuery(ServiceInfo.LMESBizDB, dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", false);
            if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dtSource = rs.ResultDataSet.Tables[0];
                //this.MessageBoxW("" + dtSource.Rows[0][0].LToInt());
                string chkSAVE = dtSource.Rows[0]["FORM_SAVE_YN"].ToString();
                string chkDELETE = dtSource.Rows[0]["FORM_DELETE_YN"].ToString();
                //this.MessageBoxW("" + chkSAVE + "," + chkDELETE);
                if (chkSAVE.Equals("Y"))
                {
                    AddButton = false;
                    FormAccessInfo.AllowSave = SaveButton = true;
                }
                else
                {
                    AddButton = false;
                    FormAccessInfo.AllowSave = SaveButton = false;
                }
                if (chkDELETE.Equals("Y"))
                {
                    DeleteRowButton = false;
                    FormAccessInfo.AllowDelete = DeleteButton = true;
                }
                else
                {
                    DeleteRowButton = false;
                    FormAccessInfo.AllowDelete = DeleteButton = false;
                }
            }
        }

        private void gvwBase_Detail_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void gvwBase_Detail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value.ToString() == "")
            {
                gvwBase_Detail.SetRowCellValue(e.RowHandle, gvwBase_Detail.Columns[e.Column.ColumnHandle], 0);
            }
        }

        private void cboInOut_EditValueChanged(object sender, EventArgs e)
        {

            if (cboInOut.EditValue.ToString().Equals("O"))
            {
                //cboDate1.Enabled = true;
                //txtCONT_CD.Visible = true;
                //lblContainer.Visible = true;
                panel1.Visible = true;
            }
            else
            {
                string strServerDate = fnSMW_Get_ServerDate();
                DateTime dtNow = new DateTime(Convert.ToInt32(strServerDate.Substring(0, 4)), Convert.ToInt32(strServerDate.Substring(4, 2)), Convert.ToInt32(strServerDate.Substring(6, 2)));
                cboDate1.EditValue = dtNow.ToString();
                cboDate1.Enabled = false;
                //txtCONT_CD.Visible = false;
                //lblContainer.Visible = false;
                panel1.Visible = false;
            }
        }

        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void gridViewEx03_ShownEditor(object sender, EventArgs e)
        {
           
        }

        private void gridViewEx03_ShowingEditor(object sender, CancelEventArgs e)
        {
            //if (!IsNumber(gridViewEx03.GetRowCellValue(gridViewEx03.FocusedRowHandle, gridViewEx03.Columns[gridViewEx03.FocusedColumn.ColumnHandle].FieldName).ToString()))
            //{
            //    gridViewEx03.SetRowCellValue(gridViewEx03.FocusedRowHandle, gridViewEx03.Columns[gridViewEx03.FocusedColumn.ColumnHandle].FieldName, "");
            //    e.Cancel = true;
            //}
        }

        private void gridViewEx03_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Value.ToString() == "")
            //{
            //    gridViewEx03.SetRowCellValue(e.RowHandle, gridViewEx03.Columns[e.Column.ColumnHandle], 0);
            //}
        }

        private void gridViewEx03_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value.ToString() == "") return;
            int strVal = Convert.ToInt32(e.Value.ToString());
        }

    }
}
