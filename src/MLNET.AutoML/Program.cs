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
            // Set up the MLContext, which is a catalog of components in ML.NET.
            var mlContext = new MLContext();

            var trainDataPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "RawData", "SMSSpamCollection");
            // Specify the schema for spam data and read it into DataView.
            var data = mlContext.Data.LoadFromTextFile<SpamInput>(path: trainDataPath);

            // Create the estimator which converts the text label to boolean, featurizes the text, and adds a linear trainer.
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline =
                mlContext.Transforms.Conversion.MapValue("BoolLabel", 
                        new List<KeyValuePair<string, bool>>() { new KeyValuePair<string, bool>("ham", false), new KeyValuePair<string, bool>("spam", true)},
                        "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", "Message"))
                .Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText"))
                .Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features"))
                .AppendCacheCheckpoint(mlContext);

            var transformerChain = dataProcessPipeline.Fit(data);
            var trainData = transformerChain.Transform(data);

            var experiment = mlContext
                .Auto()
                .CreateBinaryClassificationExperiment(20)
                .Execute(trainData, labelColumnName: "BoolLabel");
            var bestRunModel = experiment.BestRun.Model;

            Helpers.OutputBinaryClassMetrics(bestRunModel, trainData, mlContext);
        }
    }
}
