using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using WestWindLibrary.BLL;
using WestWindLibrary.Entities;

namespace WestWindWeb.Components.Pages
{
    public partial class ProductCRUD
    {
        //Our form model, what holds data for our form
        private Product product = new();
        private string feedback = string.Empty;
        private List<string> errorMsgs = [];
        private List<Supplier> suppliers = [];
        private List<Category> categories = [];
        private bool isNew;
        private EditContext editContext;

        //This parameter matches the parameter in our page directive
        //Must have the same datatype and name
        [Parameter]
        public int? productId { get; set; }

        [Inject]
        SupplierServices supplierServices { get; set; }
        [Inject]
        CategoryServices categoryServices { get; set; }
        [Inject]
        ProductServices productServices { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager _navigationManager { get; set; }

        protected override void OnInitialized()
        {
            //Fill in the initial data for the drop downs (selects)
            try
            {
                suppliers = supplierServices.GetAllSuppliers();
                categories = categoryServices.GetCategories();
                //Check if the parameter productId has a value
                if (productId.HasValue)
                {
                    //If the productId is provided we need to be able to get the Product from the database.
                    product = productServices.GetProduct_ByProductId(productId.Value);
                    if (product == null)
                    {
                        errorMsgs.Add($"Supplied product ID {productId} does not match any products in the database.");
                        product = new();
                        isNew = true;
                    }
                }
                else
                {
                    isNew = true;
                }

                //Must pass the EditContext the model being used -- product
                editContext = new EditContext(product);
            }
            catch (Exception ex)
            {
                errorMsgs.Add(GetInnerException(ex).Message);
            }
            base.OnInitialized();
        }
        private void OnValidSubmit()
        {
            //Always clear the messaging first. Always, Always, Always!
            errorMsgs.Clear();
            feedback = string.Empty;

            if (product.SupplierID == 0)
            {
                //Add to errorMsg
                //If using EditContext you can add an error to the ValidationSummary for the related field
                errorMsgs.Add("You must select a supplier.");
            }
            if (product.CategoryID == 0)
            {
                errorMsgs.Add("You must select a category.");
            }
            if (errorMsgs.Count == 0)
            {
                if (isNew)
                {
                    AddProduct();
                }
                else
                {
                    UpdateProduct();
                }

            }
        }
        private void UpdateProduct()
        {
            try
            {
                int rowAffected = productServices.Product_UpdateProduct(product);
                if (rowAffected == 0)
                {
                    errorMsgs.Add($"Product {product.ProductName} (ID: {product.ProductID}) has not been updated. Please check to see if the product still exists in the database.");
                }
                else
                {
                    feedback = $"Product {product.ProductName} (ID: {product.ProductID}) has been successfully updated.";
                }
            }
            catch (Exception ex)
            {
                errorMsgs.Add(GetInnerException(ex).Message);
            }
        }
        private void AddProduct()
        {
            try
            {
                int newproductid = productServices.Product_AddProduct(product);
                feedback = $"Product {product.ProductName} (ID: {newproductid}) has be added to the database.";
                isNew = false;
            }
            catch (Exception ex)
            {
                errorMsgs.Add(GetInnerException(ex).Message);
            }
        }
        private void OnInvalidSubmit()
        {
            feedback = "This be borked!";
        }
        private async Task DeleteProduct()
        {
            bool isDeleted = false;
            object[] messageLine = new object[] { $"Are you sure you want to delete product {product.ProductName} (ID:{product.ProductID})?" };
            if (await JSRuntime.InvokeAsync<bool>("confirm", messageLine))
            {
                try
                {
                    feedback = string.Empty;
                    errorMsgs.Clear();
                    int rowAffected = productServices.Product_PhysicalDelete(product);
                    if (rowAffected == 0)
                    {
                        errorMsgs.Add($"Product {product.ProductName} (ID: {product.ProductID}) has not been deleted. Please check to see if the product still exists in the database.");
                    }
                    else
                    {
                        await JSRuntime.InvokeAsync<object>("alert", $"Product {product.ProductName} (ID: {product.ProductID}) has been successfully deleted.");
                        isDeleted = true;
                        //Option to stays on the same page and clear the product.
                        _navigationManager.NavigateTo("/product", true);
                    }
                }
                catch (Exception ex)
                {
                    errorMsgs.Add(GetInnerException(ex).Message);
                }
                if (!isDeleted)
                {
                    errorMsgs.Clear();
                    messageLine = new object[] { $"{product.ProductName} (ID:{product.ProductID}) could not be delete. Would you like to discontinue it?" };
                    if (await JSRuntime.InvokeAsync<bool>("confirm", messageLine))
                    {
                        try
                        {
                            int rowsAfffected = productServices.Product_LogicalDelete(product);
                            if (rowsAfffected == 0)
                            {
                                errorMsgs.Add($"Product {product.ProductName} (ID: {product.ProductID}) has not been discontinued. Please check to see if the product still exists in the database.");
                            }
                            else
                            {
                                feedback = $"Product {product.ProductName} (ID: {product.ProductID}) was successfully discontinued.";
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMsgs.Add(GetInnerException(ex).Message);
                        }
                    }
                }
            }
        }
        private void ReactivateProduct()
        {
            feedback = string.Empty;
            errorMsgs.Clear();

            if (editContext.Validate())
            {
                try
                {
                    product.Discontinued = false;
                    int rowAffected = productServices.Product_UpdateProduct(product);
                    if (rowAffected == 0)
                    {
                        errorMsgs.Add($"Product {product.ProductName} (ID: {product.ProductID}) has not been reactivated. Please check to see if the product still exists in the database.");
                    }
                    else
                    {
                        feedback = $"Product {product.ProductName} (ID: {product.ProductID}) has been successfully reactivated.";
                        StateHasChanged();
                    }
                }
                catch (Exception ex)
                {
                    errorMsgs.Add(GetInnerException(ex).Message);
                }
            }
        }
        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
