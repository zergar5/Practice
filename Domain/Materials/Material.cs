namespace Domain.Materials;

public class Material
{
    public int Id { get; set; }
}

public class MaterialWithSigmaMu : Material
{
    public static double Mu => 4 * Math.PI * 1e-7;
    public double Sigma { get; set; }
}