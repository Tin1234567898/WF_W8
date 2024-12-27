using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SanphamServices
    {
        // Lấy tất cả sản phẩm
        public List<Sanpham> GetAll()
        {
            ProductDBContext context = new ProductDBContext();
            return context.Sanphams.Include("LoaiSP").ToList();
        }

        // Thêm sản phẩm mới
        public void Add(Sanpham sanpham)
        {
            ProductDBContext context = new ProductDBContext();
            context.Sanphams.Add(sanpham);
            context.SaveChanges();
        }

        // Cập nhật sản phẩm
        public void Update(Sanpham sanpham)
        {
            ProductDBContext context = new ProductDBContext();

            // Tìm sản phẩm cũ theo MaSP
            var existingSanpham = context.Sanphams.Find(sanpham.MaSP);
            if (existingSanpham != null)
            {
                // Cập nhật các trường cần thiết
                existingSanpham.TenSP = sanpham.TenSP;
                existingSanpham.Ngaynhap = sanpham.Ngaynhap;
                existingSanpham.MaLoai = sanpham.MaLoai;

                // Lưu thay đổi
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }
        }

        // Xóa sản phẩm theo mã sản phẩm
        public void Delete(string maSP)
        {
            ProductDBContext context = new ProductDBContext();
            var sanpham = context.Sanphams.Find(maSP);
            if (sanpham != null)
            {
                context.Sanphams.Remove(sanpham);
                context.SaveChanges();
            }
        }

        // Tìm sản phẩm theo mã sản phẩm
        public Sanpham GetById(string maSP)
        {
            ProductDBContext context = new ProductDBContext();
            return context.Sanphams.Find(maSP);
        }
    }
}
