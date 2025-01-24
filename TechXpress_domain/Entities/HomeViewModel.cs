using TechXpress_domain.Entities;

public class HomeViewModel
{
    public string HeroText { get; set; }
    public string HeroDescription { get; set; }
    public IEnumerable<Product> FeaturedProducts { get; set; }
}
