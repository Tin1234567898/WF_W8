using BLL;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        private readonly SanphamServices sanphamServices = new SanphamServices();
        private readonly LoaiSPServices loaiSPServices = new LoaiSPServices();

        private List<Sanpham> originalData;
        private List<Sanpham> deletedData;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu ban đầu
                originalData = sanphamServices.GetAll();
                deletedData = new List<Sanpham>();

                // Điền vào combobox loại sản phẩm
                FillLoaiSPCombobox(loaiSPServices.GetAll());
                // Bind dữ liệu lên GridView
                BindGrid(originalData);

                // Disable các nút "Lưu" và "Không lưu" ban đầu
                SetButtonState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillLoaiSPCombobox(List<LoaiSP> listLoaiSP)
        {
            listLoaiSP.Insert(0, new LoaiSP { TenLoai = "-- Chọn loại --" });
            cboLoaiSP.DataSource = listLoaiSP;
            cboLoaiSP.DisplayMember = "TenLoai";
            cboLoaiSP.ValueMember = "MaLoai";
        }

        private void BindGrid(List<Sanpham> listSanpham)
        {
            dgvSanpham.Rows.Clear();
            foreach (var item in listSanpham)
            {
                int index = dgvSanpham.Rows.Add();
                dgvSanpham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanpham.Rows[index].Cells[1].Value = item.TenSP;
                dgvSanpham.Rows[index].Cells[2].Value = item.Ngaynhap.ToString("dd/MM/yyyy");
                dgvSanpham.Rows[index].Cells[3].Value = item.LoaiSP?.TenLoai;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string maSP = txtMaSP.Text.Trim();
                var existingSanpham = sanphamServices.GetById(maSP);
                if (existingSanpham != null)
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập mã sản phẩm khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var sanPham = new Sanpham
                {
                    MaSP = txtMaSP.Text,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgayNhap.Value,
                    MaLoai = cboLoaiSP.SelectedValue?.ToString()
                };

                sanphamServices.Add(sanPham);
                MessageBox.Show("Thêm sản phẩm thành công!");

                RefreshData();
                SetButtonState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm sản phẩm: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = dgvSanpham.SelectedRows[0];
                string maSP = selectedRow.Cells[0].Value.ToString();

                var sanPham = new Sanpham
                {
                    MaSP = maSP,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgayNhap.Value,
                    MaLoai = cboLoaiSP.SelectedValue?.ToString()
                };

                sanphamServices.Update(sanPham);
                MessageBox.Show("Cập nhật sản phẩm thành công!");

                RefreshData();
                SetButtonState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa sản phẩm: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = dgvSanpham.SelectedRows[0];
                string maSP = selectedRow.Cells[0].Value.ToString();

                var sanphamToDelete = originalData.FirstOrDefault(sp => sp.MaSP == maSP);
                if (sanphamToDelete != null)
                {
                    deletedData.Add(sanphamToDelete);
                    originalData.Remove(sanphamToDelete);

                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                BindGrid(originalData);
                SetButtonState(deletedData.Any());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa sản phẩm: " + ex.Message);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string maSP = txtTim.Text.Trim();
                var listSanPham = string.IsNullOrEmpty(maSP)
                    ? sanphamServices.GetAll()
                    : new List<Sanpham> { sanphamServices.GetById(maSP) }.Where(sp => sp != null).ToList();

                if (!listSanPham.Any())
                {
                    MessageBox.Show("Sản phẩm không tồn tại.");
                    return;
                }

                BindGrid(listSanPham);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (var sanpham in deletedData)
                {
                    sanphamServices.Delete(sanpham.MaSP);
                }


                deletedData.Clear();

                BindGrid(originalData);
                SetButtonState(false);
                MessageBox.Show("Dữ liệu đã được lưu!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var sanpham in deletedData)
                {
                    originalData.Add(sanpham);
                }

                deletedData.Clear();

                BindGrid(originalData);
                SetButtonState(false);
                MessageBox.Show("Dữ liệu đã được khôi phục!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khôi phục: " + ex.Message);
            }
        }

        private void RefreshData()
        {
            originalData = sanphamServices.GetAll();
            BindGrid(originalData);
        }

        private void SetButtonState(bool enabled)
        {
            btnLuu.Enabled = enabled;
            btnKLuu.Enabled = enabled;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đóng form?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void dgvSanpham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvSanpham.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    var row = dgvSanpham.Rows[e.RowIndex];

                    txtMaSP.Text = row.Cells[0].Value?.ToString() ?? string.Empty;
                    txtTenSP.Text = row.Cells[1].Value?.ToString() ?? string.Empty;

                    if (DateTime.TryParse(row.Cells[2].Value?.ToString(), out DateTime ngayNhap))
                    {
                        dtNgayNhap.Value = ngayNhap;
                    }
                    else
                    {
                        dtNgayNhap.Value = DateTime.Now;
                    }

                    cboLoaiSP.Text = row.Cells[3].Value?.ToString() ?? "-- Chọn loại --";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
