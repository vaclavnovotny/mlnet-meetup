using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
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

            //Set AutoML experiment settings
            Console.WriteLine("Creating experiment settings");
            var settings = new MulticlassExperimentSettings()
            {
                OptimizingMetric = MulticlassClassificationMetric.MicroAccuracy,
                MaxExperimentTimeInSeconds = 20
            };
            settings.Trainers.Remove(MulticlassClassificationTrainer.FastForestOva);

            #endregion

            #region Experiment!

            // Start Experiment
            Console.WriteLine("Starting the experiment");
            var experiment = mlContext
                .Auto()
                .CreateMulticlassClassificationExperiment(20)
                .Execute(data, progressHandler: Progress);

            Console.WriteLine($"Winner: {experiment.BestRun.TrainerName}");

            #endregion

            Helpers.OutputMultiClassMetrics(experiment.BestRun.Model, data, mlContext);
        }

        private static Progress<RunDetail<MulticlassClassificationMetrics>> Progress 
            => new Progress<RunDetail<MulticlassClassificationMetrics>>(
                detail => Console.WriteLine($"Model: {detail.TrainerName}, took {detail.RuntimeInSeconds:###0.000}s, metric: {detail.ValidationMetrics.MicroAccuracy:0.000}."));
    }
}