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
using MathNet.Numerics.LinearAlgebra;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Sink)]
[TypeVisualizer(typeof(SimpleLinearRegressionPostCoefsVisualizer))]
public class PosteriorDataItem
{
    public Vector<double> mn;
    public Matrix<double> Sn;
}

