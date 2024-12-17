using Microsoft.AspNetCore.Components;
using WestWindLibrary.BLL;
using WestWindLibrary.Entities;
using WWE = WestWindLibrary.Entities;

namespace WestWindWeb.Components.Pages
{
    public partial class ProductList
    {
        private List<Product> products = [];
        private List<string> errorMsgs = [];
        private List<Category> categories = [];
        private List<Supplier> suppliers = [];
        private List<WWE.Program> programs = new List<WWE.Program>();
        private int categoryId;
        private bool noProducts;
        private string productNameSearch = string.Empty;

        [Inject]
        NavigationManager _navigationManager { get; set; }
        [Inject]
        ProductServices _productServices { get; set; }
        [Inject]
        CategoryServices _categoryServices { get; set; }
        [Inject]
        SupplierServices _supplierServices { get; set; }

        protected override void OnInitialized()
        {
            //Everytime you connect or use one your services you need a try/catch
            try
            {
                categories = _categoryServices.GetCategories();
                suppliers = _supplierServices.GetAllSuppliers();
            }
            catch (Exception ex)
            {
                errorMsgs.Add($"Data Loading Error: {GetInnerException(ex).Message}");
            }
            base.OnInitialized();
        }
        private void LoadProductsByCategory()
        {
            errorMsgs.Clear();
            noProducts = false;

            if (categoryId == 0)
            {
                errorMsgs.Add("Please select a category to search by.");
            }
            else
            {
                try
                {
                    // This coe below gets all the products
                    //products = _productServices.GetAllProducts();
                    products = _productServices.GetProducts_ByCategory(categoryId);
                    if (products.Count == 0)
                    {
                        noProducts = true;
                    }
                }
                catch (Exception ex)
                {
                    errorMsgs.Add($"Data Loading Error: {GetInnerException(ex).Message}");
                }
            }
        }

        private void SearchProductsByName()
        {
            errorMsgs.Clear();
            noProducts = false;

            if (string.IsNullOrWhiteSpace(productNameSearch))
            {
                errorMsgs.Add("Please enter a full or partial name to search.");
            }
            else
            {
                try
                {
                    products = _productServices.GetProducts_ByName(productNameSearch);
                    if (products.Count == 0)
                    {
                        noProducts = true;
                    }
                }
                catch (Exception ex)
                {
                    errorMsgs.Add($"Data Loading Error: {GetInnerException(ex).Message}");
                }
            }
        }

        private void EditProduct(int productId)
        {
            _navigationManager.NavigateTo($"/product/{productId}");
        }

        private void AddProduct()
        {
            _navigationManager.NavigateTo("/product");
        }
        private Exception GetInnerException(Exception ex)
        {
            //drill down into your Exception until there are no more inner exceptions
            //at this point you have the "real" error
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}

