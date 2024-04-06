namespace DirectProblem.Core.GridComponents;

public struct Material
{
    public double Mu { get; set; }
    public double Sigma { get; set; }

    public Material(double mu, double sigma)
    {
        Mu = mu;
        Sigma = sigma;
    }
}