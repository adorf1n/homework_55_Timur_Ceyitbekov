using System.ComponentModel.DataAnnotations;

public class Brand
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название бренда.")]
    [StringLength(100, ErrorMessage = "Название бренда не должно превышать 100 символов.")]
    public string Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
