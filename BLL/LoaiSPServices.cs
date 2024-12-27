using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LoaiSPServices
    {

        public List<LoaiSP> GetAll()
        {
            ProductDBContext context = new ProductDBContext();
            return context.LoaiSPs.ToList();
        }
    }
}
