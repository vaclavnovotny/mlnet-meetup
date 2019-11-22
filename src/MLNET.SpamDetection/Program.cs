using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using MLNET.Core;

namespace MLNET.SpamDetection
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
            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", "Message"))
                .Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText"))
                .Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features"))
                .AppendCacheCheckpoint(mlContext);

            // Set the training algorithm 
            var trainer = mlContext.MulticlassClassification.Trainers.PairwiseCoupling(
                    mlContext.BinaryClassification.Trainers.AveragedPerceptron("Label", numberOfIterations: 20, featureColumnName: "FeaturesText"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeLine = dataProcessPipeline.Append(trainer);
            
            Console.WriteLine("Training model...");

            // Now let's train a model on the full data set!
            TransformerChain<TransformerChain<KeyToValueMappingTransformer>> model;
            using (new PerformanceTimer("Training of model")) {
                model = trainingPipeLine.Fit(data);
            }

            mlContext.Model.Save(model, data.Schema, Path.Combine($"{Environment.CurrentDirectory}", "SpamDetectorModel.zip"));

            // Calculate some metrics
            Helpers.OutputMultiClassMetrics(model, data, mlContext);
            
            TestMessages(mlContext, model);
        }

        private static void TestMessages(MLContext mlContext, TransformerChain<TransformerChain<KeyToValueMappingTransformer>> model) {
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
