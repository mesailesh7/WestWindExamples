using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestWindLibrary.DAL;
using WestWindLibrary.Entities;

namespace WestWindLibrary.BLL
{
    public class SupplierServices
    {
        private WestWindContext _context;

        public SupplierServices(WestWindContext context)
        {
            _context = context;
        }

        #region Queries
        public List<Supplier> GetAllSuppliers()
        {
            //Order By just select a field to order
            //Can chain then by with as many fields as we want
            return _context.Suppliers.OrderBy(x => x.CompanyName).ThenBy(x => x.SupplierID).ToList();
        }
        #endregion
    }
}
