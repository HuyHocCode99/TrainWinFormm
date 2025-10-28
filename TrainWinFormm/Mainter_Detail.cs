using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainWinFormm
{
    public partial class frmMainter_Detail : Form
    {
        string connection = "Data Source=Huy\\SQLEXPRESS;Initial Catalog=QUANLYBANHANG;Integrated Security=True;Trust Server Certificate=True";
        DataSet DataSet = new DataSet();
        public frmMainter_Detail()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string truyvanDanhMuc = "SELECT COTID FROM CATAGORIES WHERE ";
                SqlDataAdapter categories_Adapter = new SqlDataAdapter(truyvanDanhMuc,conn);


            }
        }
    }
}
