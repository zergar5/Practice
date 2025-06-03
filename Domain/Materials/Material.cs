namespace Domain.Materials;

public interface IMaterial
{
    int Id { get; set; }
}

public class MaterialWithSigmaMu : IMaterial
{
    public int Id { get; set; }
    public double Mu { get; set; }
    public double Sigma { get; set; }
}