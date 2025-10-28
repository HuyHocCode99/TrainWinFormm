using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TrainWinFormm
{
    public partial class frmMainter_Detail : Form
    {
        string connection = "Data Source=Huy\\SQLEXPRESS;Initial Catalog=QUANLYBANHANG;Integrated Security=True";
        // Khai báo DataSet ở cấp độ class để có thể truy cập ở các hàm khác nếu cần
        DataSet dataSet = new DataSet();

        public frmMainter_Detail()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu cũ trong DataSet trước mỗi lần load để tránh trùng lặp
            dataSet.Clear();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    // LỖI SỬA 1: Cần lấy tất cả các cột hoặc ít nhất là cột tên để hiển thị
                    string truyvanDanhMuc = "SELECT COTID, CATNAME FROM CATEGORIES"; // Hoặc "SELECT * FROM Categories"
                    SqlDataAdapter categories_Adapter = new SqlDataAdapter(truyvanDanhMuc, conn);

                    string truyvanSanPham = "SELECT PRODID, PRODNAME, PRICE, COTID FROM PRODUCTS"; // Hoặc "SELECT * FROM Products"
                    SqlDataAdapter products_Adapter = new SqlDataAdapter(truyvanSanPham, conn);

                    // SqlDataAdapter sẽ tự động mở và đóng kết nối khi Fill
                    categories_Adapter.Fill(dataSet, "Categories");
                    products_Adapter.Fill(dataSet, "Products");

                    // LỖI SỬA 2: Đảm bảo tên cột trong code khớp chính xác với tên cột trong CSDL
                    DataColumn parentCol = dataSet.Tables["Categories"].Columns["COTID"];
                    DataColumn childCol = dataSet.Tables["Products"].Columns["COTID"];

                    DataRelation relation = new DataRelation("Categories_Products", parentCol, childCol);
                    dataSet.Relations.Add(relation);

                    // LỖI SỬA 3: Logic hiển thị dữ liệu lên DataGridView
                    // dgv cha sẽ hiển thị bảng cha (Categories)
                    dgvCategories.DataSource = dataSet.Tables["Categories"];

                    // dgv con sẽ dùng chung nguồn dữ liệu với dgv cha và dùng DataMember là tên của mối quan hệ
                    dgvSanPham.DataSource = dgvCategories.DataSource;
                    dgvSanPham.DataMember = "Categories_Products";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                }
            }
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];// lấy phần tử mà đang chọn trong dgv
                // lấy mã sản phẩm
                txtMaSP.Text = row.Cells["PRODID"].Value.ToString();
                txtTenSP.Text = row.Cells["PRODNAME"].Value.ToString();
                txtGiaSP.Text = row.Cells["PRICE"].Value.ToString();
            }
        }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCategories.Rows[e.RowIndex];// lấy phần tử mà đang chọn trong dgv
                txtMaSP.Text = row.Cells["COTID"].Value.ToString();
                lblTenSP.Text = row.Cells["CATNAME"].Value.ToString();
            }
        }

            private void btnThem_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(txtTenSP.Text) || string.IsNullOrWhiteSpace(txtGiaSP.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm.");
                    return;
                }
                try
                {
                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        conn.Open();
                        string insertQuery = "INSERT INTO PRODUCTS ( PRODNAME, PRICE, COTID) VALUES ( @PRODNAME, @PRICE, @COTID)";
                        SqlCommand cmdThem = new SqlCommand(insertQuery, conn);

                    int selectedID = Convert.ToInt32(dgvCategories.CurrentRow.Cells["COTID"].Value);
                        // tạo thêm những parameter để tránh lỗi SQL Injection
                        cmdThem.Parameters.AddWithValue("@PRODNAME", txtTenSP.Text);
                        cmdThem.Parameters.AddWithValue("@PRICE", txtGiaSP.Text);
                        cmdThem.Parameters.AddWithValue("@COTID", selectedID); // Assuming COTID is also required

                    // Correct the method call to ExecuteNonQuery instead of EndExecuteNonQuery
                    int result = cmdThem.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Thêm sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 💡 CẢI TIẾN: Tải lại dữ liệu để cập nhật DataGridView
                        btnLoad_Click(sender, e);
                        // 💡 CẢI TIẾN: Xóa trắng các ô nhập liệu sau khi thêm
                        txtMaSP.Clear();
                        txtTenSP.Clear();
                        txtGiaSP.Clear();
                    }
                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                }
            }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtTenSP.Text) || string.IsNullOrWhiteSpace(txtGiaSP.Text))
            {
                MessageBox.Show("Bạn Vui Lòng Nhập Đầy Đủ Thông Tin Vào Đây!","Thông Báo!",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            try
            {
                using(SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmdSua = new SqlCommand("UPDATE PRODUCTS SET PRODNAME=@PRODNAME, PRICE=@PRICE WHERE PRODID=@PRODID", conn);
                    int selectedID = Convert.ToInt32(dgvSanPham.CurrentRow.Cells["PRODID"].Value);
                    cmdSua.Parameters.AddWithValue("@PRODID", selectedID);
                    cmdSua.Parameters.AddWithValue("@PRODNAME", txtTenSP.Text);
                    cmdSua.Parameters.AddWithValue("@PRICE", txtGiaSP.Text);
                    int result = cmdSua.ExecuteNonQuery();
                    if(result > 0)
                    {
                        MessageBox.Show("Sửa Thành Công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnLoad_Click(sender, e);
                        txtGiaSP.Clear();
                        txtTenSP.Clear();
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    int selectedID = Convert.ToInt32(dgvSanPham.CurrentRow.Cells["PRODID"].Value);
                    SqlCommand cmdXoa = new SqlCommand("DELETE FROM PRODUCTS WHERE PRODID=@PRODID", conn);
                    cmdXoa.Parameters.AddWithValue("@PRODID", selectedID);
                    int result = cmdXoa.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Xóa Thành Công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnLoad_Click(sender, e);
                        txtGiaSP.Clear();
                        txtTenSP.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }

}