
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
using MathNet.Numerics.Statistics;
using ScottPlot;
using ScottPlot.Plottable;

[assembly: TypeVisualizer(typeof(PredictionsVsResponsesVis), Target=typeof(Tuple<System.ValueTuple<double, double>, double>))] 

public class PredictionsVsResponsesVis : DialogTypeVisualizer
{
    public static int numPointsToSimDisplay{ get; set; }
    private static ScottPlot.FormsPlot _formsPlot1;
    private static double[] _observations;
    private static double[] _predictions;
    private static double[] _axisLimits = new double[] { 0.0, 15.0, 0.0, 3.0 };
    private static ScottPlot.Plottable.ScatterPlot _scatterPlot;

    public override void Load(IServiceProvider provider)
    {
        numPointsToSimDisplay = 500;
        _formsPlot1 = new ScottPlot.FormsPlot() { Dock = DockStyle.Fill };
        _observations = new double[numPointsToSimDisplay];
        _predictions = new double[numPointsToSimDisplay];
        _scatterPlot = _formsPlot1.Plot.AddScatter(new double[] { _axisLimits[0], _axisLimits[1] }, new double[] { _axisLimits[2], _axisLimits[3] }, lineWidth: 0, color: Color.Red);
        _scatterPlot = _formsPlot1.Plot.AddScatter(_observations, _predictions, lineWidth: 0, color: Color.Blue);
        _formsPlot1.Plot.XLabel("Responses of Neuron (spikes/bin)");
        _formsPlot1.Plot.YLabel("Predictions");
        _formsPlot1.Plot.SetAxisLimits(_axisLimits[0], _axisLimits[1], _axisLimits[2], _axisLimits[3]);
        _formsPlot1.Refresh();

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        if (visualizerService != null)
        {
            visualizerService.AddControl(_formsPlot1);
        }
    }

    public override void Show(object value)
    {
        Tuple<System.ValueTuple<double, double>, double> pair = (Tuple<System.ValueTuple<double, double>, double>) value;
        Array.Copy(_observations, 1, _observations, 0, _observations.Length - 1);
        Array.Copy(_predictions, 1, _predictions, 0, _predictions.Length - 1);
        _predictions[_predictions.Length-1] = pair.Item1.Item1;
        _observations[_observations.Length-1] = pair.Item2;
        _scatterPlot.Update(_observations, _predictions);
        double corCoef = Correlation.Pearson(_observations, _predictions);
        _formsPlot1.Plot.Title(String.Format("Correlation Coefficient: {0:F2}", corCoef));
        _formsPlot1.Refresh();
    }

    public override void Unload()
    {
    }
}
