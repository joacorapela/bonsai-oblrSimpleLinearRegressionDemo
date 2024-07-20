using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Drawing;
using Bonsai.Design;
using System.Windows.Forms;
using Bonsai.Design.Visualizers;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using ScottPlot;
using ScottPlot.Plottable;

[assembly: TypeVisualizer(typeof(BatchRegressionObsAndPredictionsVisualizer), Target=typeof(Tuple<IList<RegressionObservation>, PosteriorDataItem>))] 

public class BatchRegressionObsAndPredictionsVisualizer : DialogTypeVisualizer
{
    private static ScottPlot.FormsPlot _formsPlot1;
    public List<Func<double, double>> basisFunctions;
    public double beta;

    public override void Load(IServiceProvider provider)
    {
        _formsPlot1 = new ScottPlot.FormsPlot() { Dock = DockStyle.Fill };
    }

    public override void Show(object aValue)
    {
        Console.WriteLine("BatchRegressionObsAndPredictionsVis::Show called");

        // Step 1: Cast `aValue` to the tuple type
        var tuple = (Tuple<IList<RegressionObservation>, PosteriorDataItem>)aValue;

        // Step 2: Deconstruct the tuple
        var batchRObs = tuple.Item1;
        var pdi = tuple.Item2;

        //var (batchRObs, pdi) = ((IList<RegressionObservation>, PosteriorDataItem)) aValue;
	// var (batchRObs, pdi) = (IList<RegressionObservation>, PosteriorDataItem) aValue;
    	// var batchRObs = (IList<RegressionObservation>, PosteriorDataItem) aValue;
        // List<RegressionObservation> batchRObs = (System.Collections.Generic.List<RegressionObservation>) batchROandPos.batchRObs;
        // PosteriorDataItem pdi = batchROandPos.pdi;

        // computer predictions
        double[] x = new double[batchRObs.Count];
        double[] t = new double[batchRObs.Count];
        for (int i=0; i<batchRObs.Count; i++)
        {
            x[i] = batchRObs[i].phi[2];
            t[i] = batchRObs[i].t;
        }

            // find xMin and xMax
        double xMin = x[0];
        double xMax = x[0];
        for(int i=1; i<x.Length; i++)
        {
            if (x[i] > xMax)
            {
                xMax = x[i];
            }
            if (x[i] < xMin)
            {
                xMin = x[i];
            }
        }

        int nDense = 100;
        double step = (xMax - xMin) / nDense;
        var xDense = Enumerable.Range(0, (int)Math.Ceiling((xMax - xMin) / step))
            .Select(i => xMin + i * step).ToArray();
        double[] mean = new double[xDense.Length];
        double[] variance  = new double[xDense.Length];
        for (int i=0; i<xDense.Length; i++)
        {
            var subsetBasisFunctions = new List<Func<double, double>>(this.basisFunctions.Take(pdi.mn.Count));
            Vector<double> phiRow = RegressionUtils.BuildDesignMatrixRow(xDense[i], subsetBasisFunctions);
            // (mean[i], variance[i]) = BayesianLinearRegression.Predict(phiRow, pdi.mn, pdi.Sn, this.beta);
            // var (aMean, aVar) = BayesianLinearRegression.Predict(phiRow, pdi.mn, pdi.Sn, this.beta);

            // var tuple2 = BayesianLinearRegression.Predict(phiRow, pdi.mn, pdi.Sn, this.beta);
            // var aMean = tuple2.Item1;
            // var aVar = tuple2.Item2;

            // ValueTuple<double, double> tuple2 = BayesianLinearRegression.Predict(phiRow, pdi.mn, pdi.Sn, this.beta);
            ValueTuple<double, double> tuple2 = BayesianLinearRegression.Predict(phiRow, pdi.mn, pdi.Sn, this.beta);

	    mean[i] = tuple2.Item1;
	    variance[i] = tuple2.Item2;
        }

        // plot means and 95% ci for xDense
        var ci95Width = variance.Select(aVar=>1.96*Math.Sqrt(aVar)).ToArray();

        _formsPlot1.Plot.Clear();

        _formsPlot1.Plot.AddScatter(xDense, mean, Color.Blue, label: "Predictions");
        _formsPlot1.Plot.AddFillError(xDense, mean, ci95Width, Color.FromArgb(50, Color.Blue));

        // plot data
        _formsPlot1.Plot.AddScatter(x, t, Color.Red, lineWidth: 0, label: "Observations");
        _formsPlot1.Plot.YLabel("f(x)");
        _formsPlot1.Plot.XLabel("x");
        var legend = _formsPlot1.Plot.Legend();
        legend.Location = Alignment.UpperLeft;


        _formsPlot1.Refresh();
    }

    public override void Unload()
    {
    }
}
