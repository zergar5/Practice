namespace Domain.Materials;

public interface IMaterial
{
    int Id { get; set; }
}

public abstract class Material : IMaterial
{
    public int Id { get; set; }
}

public class MaterialWithSigmaMu : Material
{
    public double Mu { get; set; }
    public double Sigma { get; set; }
}