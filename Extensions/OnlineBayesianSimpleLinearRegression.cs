using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Xml.Serialization;
using System.Globalization;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class OnlineBayesianSimpleLinearRegression
{
    public double priorPrecision { get; set; }

    public double likePrecision { get; set; }

    [TypeConverter(typeof(UnidimensionalArrayConverter))]
    public double[] m0 { get; set; }

    [XmlIgnore]
    [TypeConverter(typeof(MultidimensionalArrayConverter))]
    public double[,] S0 { get; set; }

    [Browsable(false)]
    [XmlElement("S0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string KernelXml
    {
        get { return ArrayConvert.ToString(S0, CultureInfo.InvariantCulture); }
	set { S0 = (double[,])ArrayConvert.ToArray(value, 2, typeof(double), CultureInfo.InvariantCulture); }
    }

    public IObservable<PosteriorDataItem> Process(IObservable<RegressionObservation> source)
    {
        Console.WriteLine("PosteriorCalculator Process called");
        return source.Scan(
            new PosteriorDataItem
            {
                mean = Vector<double>.Build.DenseOfArray(m0),
                cov = Matrix<double>.Build.DenseOfArray(S0)
            },
            (prior, observation) =>
            {
                double[] aux = new[] { 1, observation.x };
                Vector<double> phi = Vector<double>.Build.DenseOfArray(aux);
                PosteriorDataItem pdi = BayesianLinearRegression.OnlineUpdate(prior, phi, observation.t, priorPrecision, likePrecision);
                return pdi;
            });
    }
}
