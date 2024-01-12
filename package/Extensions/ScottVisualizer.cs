using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class ScottVisualizer
{
    public IObservable<PosteriorDataItem> Process(IObservable<PosteriorDataItem> source)
    {
        return source;
    }
}
