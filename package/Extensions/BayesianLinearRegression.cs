using MathNet.Numerics.LinearAlgebra;

public class BayesianLinearRegression
{
    static public PosteriorDataItem OnlineUpdate(PosteriorDataItem prior, Vector<double> phi, double y, double alpha, double beta)
    {
         Vector<double> mn = prior.mn;
         Matrix<double> Sn = prior.Sn;

         Vector<double> aux1 = Sn.Multiply(phi);
         double aux2 = 1.0/(1.0 / beta + phi.ToRowMatrix().Multiply(Sn).Multiply(phi)[0]);

         Matrix<double> Snp1 = Sn - aux2 * aux1.OuterProduct(aux1);
         Vector<double> mnp1 = beta * y * Snp1.Multiply(phi) + mn - aux2 * phi.DotProduct(mn) * Sn.Multiply(phi);

	 System.Console.WriteLine("mnp1");
	 System.Console.WriteLine(mnp1);
	 System.Console.WriteLine("Snp1");
	 System.Console.WriteLine(Snp1);

         PosteriorDataItem posterior = new PosteriorDataItem
         {
             mn = mnp1,
             Sn = Snp1
         };

	 return posterior;
    }

}
