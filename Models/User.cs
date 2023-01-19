// using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FreelanceApi2.Models;

public class User
{
    // [Key]
    public Guid UserId { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? Name { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? LastName { get; set; }

    // [Required]
    // [EmailAddress]
    public string? Email { get; set; }

    // public string? Password { get; set; }
    [JsonIgnore]
    public virtual ICollection<Receipt>? Receipts { get; set; }

}