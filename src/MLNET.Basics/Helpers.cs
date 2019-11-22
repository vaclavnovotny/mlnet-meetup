using System;
using Microsoft.ML;

namespace MLNET.Core
{
    public static class Helpers
    {
        public static void OutputMultiClassMetrics(ITransformer model, IDataView data, MLContext mlContext)
        {

            var dataView = model.Transform(data);
            var metrics = mlContext.MulticlassClassification.Evaluate(dataView);
            var confusionTable = metrics.ConfusionMatrix.GetFormattedConfusionTable();
            Console.WriteLine($"\nMicro accuracy: {metrics.MicroAccuracy}");
            Console.WriteLine($"Macro accuracy: {metrics.MacroAccuracy}");
            Console.WriteLine($"Log Loss: {metrics.LogLoss}");
            Console.WriteLine($"Log Loss reduction: {metrics.LogLossReduction}");
            Console.Write(confusionTable);
        }

        public static void OutputBinaryClassMetrics(ITransformer model, IDataView data, MLContext mlContext)
        {
            var dataView = model.Transform(data);
            var metrics = mlContext.BinaryClassification.EvaluateNonCalibrated(dataView, "BoolLabel");
            var confusionTable = metrics.ConfusionMatrix.GetFormattedConfusionTable();
            Console.WriteLine($"\nAccuracy: {metrics.Accuracy}");
            Console.Write(confusionTable);
        }

        public static void ClassifyMessage(PredictionEngine<SpamInput, SpamPrediction> predictor, string message)
        {
            var input = new SpamInput { Message = message };

            SpamPrediction prediction;
            using (new PerformanceTimer("Prediction", true))
            {
                prediction = predictor.Predict(input);
            }
            Console.BackgroundColor = prediction.IsSpam ? ConsoleColor.DarkRed : ConsoleColor.Black;
            Console.WriteLine($"The message '{input.Message}' is {(prediction.IsSpam ? "spam" : "not spam")}. {prediction.Scores[0]:0.000}, {prediction.Scores[1]:0.000}\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
