using System;
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

            // Load data from text file
            var data = mlContext.Data.LoadFromTextFile<SpamInput>(path: trainDataPath);

            #region ExperimentSettings

            // Set AutoML experiment settings
            var settings = new MulticlassExperimentSettings() {
                OptimizingMetric = MulticlassClassificationMetric.MicroAccuracy, 
                MaxExperimentTimeInSeconds = 20
            };
            settings.Trainers.Remove(MulticlassClassificationTrainer.FastForestOva);

            #endregion

            #region Experiment!

            // Start Experiment
            var experiment = mlContext
                .Auto()
                .CreateMulticlassClassificationExperiment(settings)
                .Execute(data);

            #endregion

            Helpers.OutputMultiClassMetrics(experiment.BestRun.Model, data, mlContext);
        }
    }
}