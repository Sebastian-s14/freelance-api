namespace FreelanceApi2.Models;

public class Receipt
{
    // [Key]
    public Guid ReceiptId { get; set; }

    // [ForeignKey("UserId")]
    // public Guid UserId { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? Name { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? LastName { get; set; }

    // [Required]
    public string? Logo { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? Address { get; set; }

    // [Required]
    // [MaxLength(150)]
    public string? Title { get; set; }

    // // [Required]
    // // [MaxLength(300)]
    public string? Description { get; set; }

    // // [Required]
    public string? TypeDocument { get; set; }

    // // [Required]
    public string? NumberDocument { get; set; }

    // // [Required]
    public string? currency { get; set; }

    // // [Required]
    public int payment { get; set; }

    public DateTime CreateAt { get; set; }

    // public virtual User? User { get; set; }
}

// public enum TypeDoc
// {
//     DNI,
//     RUC
// }