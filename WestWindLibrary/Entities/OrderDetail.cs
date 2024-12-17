﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WestWindLibrary.Entities;

[Index("OrderID", Name = "OrderID")]
[Index("OrderID", Name = "OrdersOrder_Details")]
[Index("ProductID", Name = "ProductID")]
[Index("ProductID", Name = "ProductsOrder_Details")]
public partial class OrderDetail
{
    [Key]
    public int OrderDetailID { get; set; }

    public int OrderID { get; set; }

    public int ProductID { get; set; }

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    public short Quantity { get; set; }

    public float Discount { get; set; }

    [ForeignKey("OrderID")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("OrderDetails")]
    public virtual Product Product { get; set; }
}