public class ReviewViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; }
    [System.ComponentModel.DataAnnotations.Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
}