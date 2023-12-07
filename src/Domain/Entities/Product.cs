using Domain.Common;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public int Price { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}