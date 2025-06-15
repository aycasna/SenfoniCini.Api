public class Color
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HexCode { get; set; } 


    public ICollection<ProductColor> ProductColors { get; set; }
}
