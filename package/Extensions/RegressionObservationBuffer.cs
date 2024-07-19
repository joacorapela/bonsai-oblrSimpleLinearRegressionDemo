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
public class RegressionObservationBuffer
{
    public int count { get; set; }

    public int skip { get; set; }

    public IObservable<IList<RegressionObservation>> Process(IObservable<RegressionObservation> source)
    {
        Console.WriteLine("RegressionObservationBuffer Process called");
        return source.Buffer(count);
    }
}
