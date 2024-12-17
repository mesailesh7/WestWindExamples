using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using WestWindLibrary.DAL;
using WestWindLibrary.Entities;

namespace WestWindLibrary.BLL
{
    public class ProductServices
    {
        private readonly WestWindContext _context;

        //Create a constructor that take in the context (map to the database)
        public ProductServices(WestWindContext context)
        {
            _context = context;
        }

        #region Queries
        public List<Product> GetAllProducts()
        {
            //Address is only Included as an example.
            //Can only include a full entity!! DO NOT Try and include a field, this will break.
            return _context.Products.Include(p=>p.Category).Include(p=>p.Supplier).ThenInclude(s=>s.Address).ToList();
        }
        public List<Product> GetProducts_ByCategory(int categoryId)
        {
            //Includes removed as an example, the supplier and category lists on the ProductList page supply the Product table values. Example for reference.
            return _context.Products.Where(x=>x.CategoryID.Equals(categoryId)).ToList();
        }
        public List<Product> GetProducts_ByName(string searchString)
        {
            //Business Rule Example: Match full OR partial string
            return _context.Products.Where(x => x.ProductName.ToLower().Contains(searchString.ToLower())).Include(p => p.Category).Include(p => p.Supplier).ToList();

            //For exact match we can use .Equals() ==, consider if you want them in the same case in this 'case'
        }
        public Product GetProduct_ByProductId(int productId)
        {
            return _context.Products.Where(x => x.ProductID == productId).Include(p => p.Category).Include(p => p.Supplier).FirstOrDefault();
        }
        #endregion

        #region CRUD
        //For an insert we always return an int for identifier as the PK, this is the new PK
        public int Product_AddProduct(Product product)
        {
            //Check if you got data
            if (product == null)
            {
                throw new ArgumentNullException("You must supply the new product information.");
            }

            //Business Rule Example
            //Does the Product already exist?
            bool exists = _context.Products.Any(x => x.SupplierID == product.SupplierID
                                                && x.ProductName == product.ProductName);

            if (exists)
            {
                throw new ArgumentException("Product already exists, cannot add.");
            }

            //You can check multiple Business rules
            //You can reuse the exists bool if needed

            //Staging
            //IMPORTANT - Remember this is only local, meaning it is only local in memory not in the database
            //This product will not have an ID
            _context.Products.Add(product);

            //Commit
            //This sends the data to the database
            //The info for the changes or inserted records is updated in our DBSet (Products)
            _context.SaveChanges();

            //Now the new product will have an ID
            return product.ProductID;
        }

        //For an update we always return an int. This is not an indentifier, this is how many rows were affected by our update.
        public int Product_UpdateProduct(Product product)
        {
            //Check if you got data
            if (product == null)
            {
                throw new ArgumentNullException("You must supply the new product information.");
            }

            //Need to check that the data exists in the database
            bool exists = _context.Products.Any(p => p.ProductID == product.ProductID);

            if(!exists)
            {
                throw new ArgumentException($"Product {product.ProductName} (ID: {product.ProductID}) is no longer in the database.");
            }

            //Business Rule Example
            //Make sure the product wasn't updated to match another product.
            //Make sure to we include a check that we aren't faulsly matching the product we are working on (Updating) - Producing a false match
            exists = _context.Products.Any(p => p.SupplierID == product.SupplierID
                                            && p.ProductName == product.ProductName
                                            && p.ProductID != product.ProductID);

            if (exists)
            {
                throw new ArgumentException($"Product {product.ProductName} (ID: {product.ProductID}) already exists for the indicated supplier.");
            }

            //Can add more business rules, etc.

            //Staging
            EntityEntry<Product> updating = _context.Entry(product);
            updating.State = EntityState.Modified;

            //Commit
            //This return will return how many records were updated in the database.
            return _context.SaveChanges();
        }

        //For a delete we always again return an int. This is not an indentifier, this is how many rows were affected by the delete.
        public int Product_PhysicalDelete(Product product)
        {
            //Physical Delete
            //Remove the record from the data
            //If there are child records to prevent the record removal
            //You must remove the child first
            //Cascade the Delete
            //Database can have Cascade deletes set up in the database, this is not always the case.
            //If there are child records AND the child record are required
            //You cannot delete the record!
            //Potential Business rules

            if (product == null)
            {
                throw new ArgumentNullException("You must supply the new product information.");
            }

            //check if the product exists in the database
            //Return the product if it exists, or null if it does not.
            //Product? exists = _context.Products.Where(p => p.ProductID == product.ProductID).FirstOrDefault();
            Product? exists = _context.Products.Include(p=>p.OrderDetails).Include(p=>p.ManifestItems).FirstOrDefault(p => p.ProductID == product.ProductID);

            if (exists == null)
            {
                throw new ArgumentException($"Product {product.ProductName} (ID: {product.ProductID}) is no longer in the database.");
            }

            //Potential Check Needed
            //Check if Child Records exists, this is generally good practice.
            int existingChildren = exists.ManifestItems.Count;
            existingChildren += exists.OrderDetails.Count;

            //Example Business Rule
            //If a child record exists you cannot delete the product
            //If an order contained this product, it cannot be deleted.
            if(existingChildren > 0)
            {
                throw new ArgumentException($"Product {product.ProductName} (ID: {product.ProductID}) has related information in the database. Unable to delete.");
            }

            //Staging
            EntityEntry<Product> deleting = _context.Entry(product);
            deleting.State = EntityState.Deleted;

            //Commit
            //The returned value from the database for a physical delete is the number of rows affected
            return _context.SaveChanges();
        }

        //Logical Delete - Discontinue the product
        public int Product_LogicalDelete(Product product)
        {
            //Check if you got data
            if (product == null)
            {
                throw new ArgumentNullException("You must supply the new product information.");
            }

            //Need to check that the data exists in the database
            bool exists = _context.Products.Any(p => p.ProductID == product.ProductID);

            if (!exists)
            {
                throw new ArgumentException($"Product {product.ProductName} (ID: {product.ProductID}) is no longer in the database.");
            }

            //Can have business rule logic for a logical delete
                //Ex: Cannot discontinue where there is an active order, etc.
            
            //We need to change the record to make the logical delete true (or false depending on the record)
                //Ex: Discontinued = true OR Active = false
            product.Discontinued = true;

            //Staging
            EntityEntry<Product> updating = _context.Entry(product);
            updating.State = EntityState.Modified;

            //Commit
            //This return will return how many records were updated in the database.
            return _context.SaveChanges();
        }
        #endregion
    }
}
