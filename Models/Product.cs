
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название товара.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Название товара должно содержать от 3 до 100 символов.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Введите стоимость товара.")]
    [Range(50, double.MaxValue, ErrorMessage = "Стоимость товара должна быть не менее 50.")]
    public double Price { get; set; }

    public string? ImagePath { get; set; }

    [Required(ErrorMessage = "Выберите категорию.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Выберите бренд.")]
    public int BrandId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Навигационные свойства
    public virtual Category? Category { get; set; }
    public virtual Brand? Brand { get; set; }
}
