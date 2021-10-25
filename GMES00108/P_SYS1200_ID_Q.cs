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
    public class P_SYS1200_ID_Q : BaseProcClass
    {
        public P_SYS1200_ID_Q()
        {
            // Modify Code : Procedure Name
            _ProcName = "P_SYS1200_ID_Q";
            ParamAdd();
        }
        public void ParamAdd()
        {
            // Modify Code : Procedure Parameter 
            _ParamInfo.Add(new ParamInfo("@V_WORK_TYPE", "Varchar", 20, "Input", typeof(System.String)));
            _ParamInfo.Add(new ParamInfo("@v_p_file_name", "Varchar", 30, "Input", typeof(System.String)));
            _ParamInfo.Add(new ParamInfo("@v_p_user_id", "Varchar", 20, "Input", typeof(System.String)));
        }
        public DataTable SetParamData(DataTable dataTable, String V_WORK_TYPE, String V_P_FILE_NAME, String V_P_USER_ID)
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
                                    V_WORK_TYPE, V_P_FILE_NAME, V_P_USER_ID
                };
            dataTable.Rows.Add(objData);
            return dataTable;
        }
    }
}
