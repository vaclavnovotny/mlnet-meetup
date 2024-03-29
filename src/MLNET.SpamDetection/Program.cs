﻿using System;
using System.IO;
using Microsoft.ML;
using MLNET.Core;

namespace MLNET.SpamDetection
{

    class Program
    {
        static void Main(string[] args)
        {
            // Set up the MLContext, which is a catalog of components in ML.NET.
            var mlContext = new MLContext();

            #region LoadData

            var trainDataPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "RawData", "SMSSpamCollection");
            // Specify the schema for spam data and read it into DataView.
            IDataView data = mlContext.Data.LoadFromTextFile<SpamInput>(path: trainDataPath);

            #endregion

            #region PreprocessData

            // Create the estimator which converts the text label to boolean, featurizes the text, and adds a linear trainer.
            // Data process configuration with pipeline data transformations
            var dataProcessPipeline = 
                mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", "Message"))
                .Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText"))
                .Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features"));

            #endregion

            #region SelectModel

            // Set the training algorithm 
            var trainer = mlContext.MulticlassClassification.Trainers.PairwiseCoupling(
                    mlContext.BinaryClassification.Trainers.AveragedPerceptron(numberOfIterations: 20))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeLine = dataProcessPipeline.Append(trainer);

            #endregion

            #region StartTraining

            Console.WriteLine("Training model...");
            // Now let's train a model on the full data set!
            ITransformer model;
            using (new PerformanceTimer("Training of model")) {
                model = trainingPipeLine.Fit(data);
            }

            #endregion

            #region SaveTrainedModel

            mlContext.Model.Save(model, data.Schema, Path.Combine($"{Environment.CurrentDirectory}", "..", "..", "..", "SpamDetectorModel.zip"));

            #endregion

            #region OutputMetrics

            // Calculate some metrics
            Helpers.OutputMultiClassMetrics(model, data, mlContext);

            #endregion

            #region TestOnCustomData

            // Lets test some of our own messages!
            TestMessages(mlContext, model);

            #endregion
        }

        private static void TestMessages(MLContext mlContext, ITransformer model) {
            // Create a PredictionFunction from our model 
            var predictor = mlContext.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(model);

            Console.WriteLine("\nType message to be checked or type exit");
            string message;
            while (!(message = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? true) {
                if (string.IsNullOrWhiteSpace(message))
                    continue;
                Helpers.ClassifyMessage(predictor, message);
            }
        }
    }
}
