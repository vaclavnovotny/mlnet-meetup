using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using MLNET.Core;

namespace MLNET.AutoML
{
    class Program
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext();
            var trainDataPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "RawData", "SMSSpamCollection");

            var data = mlContext.Data.LoadFromTextFile<SpamInput>(path: trainDataPath);

            var experiment = mlContext
                .Auto()
                .CreateMulticlassClassificationExperiment(20)
                .Execute(data);

            Helpers.OutputMultiClassMetrics(experiment.BestRun.Model, data, mlContext);
        }
    }
}
