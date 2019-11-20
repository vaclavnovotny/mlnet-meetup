using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using MLNET.Basics;

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
            var trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName: "Label", numberOfIterations: 20, featureColumnName: "FeaturesText"), labelColumnName: "Label")
                                      .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeLine = dataProcessPipeline.Append(trainer);


            Console.WriteLine("Training model...");

            // Now let's train a model on the full data set!
            TransformerChain<TransformerChain<KeyToValueMappingTransformer>> model;
            using (new PerformanceTimer("Training of model")) {
                model = trainingPipeLine.Fit(data);
            }

            // Calculate some metrics
            OutputMetrics(model, data, mlContext);

            // Create a PredictionFunction from our model 
            var predictor = mlContext.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(model);

            Console.WriteLine("\nType message to be checked or type empty message to exit");
            string readLine;
            while (!string.IsNullOrEmpty(readLine = Console.ReadLine()))
            {
                if (readLine.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                    break;
                ClassifyMessage(predictor, readLine);
            }
        }

        private static void OutputMetrics(TransformerChain<TransformerChain<KeyToValueMappingTransformer>> model, IDataView data, MLContext mlContext) {
            
            var dataView = model.Transform(data);
            var metrics = mlContext.MulticlassClassification.Evaluate(dataView);
            var confusionTable = metrics.ConfusionMatrix.GetFormattedConfusionTable();
            Console.WriteLine($"\nMicro accuracy: {metrics.MicroAccuracy}");
            Console.WriteLine($"Macro accuracy: {metrics.MacroAccuracy}");
            Console.WriteLine($"Log Loss: {metrics.LogLoss}");
            Console.WriteLine($"Log Loss reduction: {metrics.LogLossReduction}");
            Console.Write(confusionTable);
        }

        private static void ClassifyMessage(PredictionEngine<SpamInput, SpamPrediction> predictor, string message)
        {
            var input = new SpamInput { Message = message };

            SpamPrediction prediction;
            using (new PerformanceTimer("Prediction", true)) {
                prediction = predictor.Predict(input);
            }
            Console.BackgroundColor = prediction.IsSpam ? ConsoleColor.DarkRed : ConsoleColor.Black;
            Console.WriteLine($"The message '{input.Message}' is {(prediction.IsSpam ? "spam" : "not spam")}. {prediction.Scores[0]:0.000}, {prediction.Scores[1]:0.000}\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
