using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestWindLibrary.DAL;
using WestWindLibrary.Entities;

namespace WestWindLibrary.BLL
{
    public class CategoryServices
    {
        private WestWindContext _context;

        public CategoryServices(WestWindContext context)
        {
            _context = context;
        }

        #region Queries
        public List<Category> GetCategories()
        {
            return _context.Categories.OrderBy(x => x.CategoryName).ToList();
        }
        #endregion
    }
}
