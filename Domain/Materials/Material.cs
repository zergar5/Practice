namespace Domain.Materials;

public class Material
{
    public int Id { get; set; }
}

public class MaterialWithSigmaMu : Material
{
    public double Mu { get; set; }
    public double Sigma { get; set; }
}