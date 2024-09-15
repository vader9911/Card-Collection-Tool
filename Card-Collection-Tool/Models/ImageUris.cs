public class ImageUris
{
    public int Id { get; set; }
    public string? Small { get; set; }
    public string? Normal { get; set; }
    public string? Large { get; set; }
    public string? Png { get; set; }
    public string? ArtCrop { get; set; }
    public string? BorderCrop { get; set; }

    public string ScryfallCardId { get; set; }
    public ScryfallCard ScryfallCard { get; set; }
}