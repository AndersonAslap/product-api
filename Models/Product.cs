using System.ComponentModel.DataAnnotations;

namespace FirstApi.Models;
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public int QtdStock { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string? Description { get; set; }
}