﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WestWindLibrary.Entities;

public partial class Shipment
{
    [Key]
    public int ShipmentID { get; set; }

    public int OrderID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ShippedDate { get; set; }

    public int ShipVia { get; set; }

    [Column(TypeName = "money")]
    public decimal FreightCharge { get; set; }

    [StringLength(128)]
    [Unicode(false)]
    public string TrackingCode { get; set; }

    [InverseProperty("Shipment")]
    public virtual ICollection<ManifestItem> ManifestItems { get; set; } = new List<ManifestItem>();

    [ForeignKey("OrderID")]
    [InverseProperty("Shipments")]
    public virtual Order Order { get; set; }

    [ForeignKey("ShipVia")]
    [InverseProperty("Shipments")]
    public virtual Shipper ShipViaNavigation { get; set; }
}