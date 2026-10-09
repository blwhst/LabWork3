using System;
using System.Collections.Generic;

namespace ShoeStoreData.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
