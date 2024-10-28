using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название категории.")]
    [StringLength(100, ErrorMessage = "Название категории не должно превышать 100 символов.")]
    public string Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
