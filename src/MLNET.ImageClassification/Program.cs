using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Vision;
using MLNET.Core;

namespace MLNET.ImageClassification
{
    class Program
    {
        static void Main(string[] args)
        {

            var projectDirectory = args.Length == 1 ? args[0] : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
            var workspaceRelativePath = Path.Combine(projectDirectory, "workspaceHuge");
            var assetsRelativePath = Path.Combine(projectDirectory, "Data");
            var modelDestinationPath = Path.Combine(projectDirectory, "Model", "modelHuge.zip");

            var mlContext = new MLContext();
            LoadData(assetsRelativePath, mlContext, out var trainSet, out var validationSet, out var testSet);

            var trainedModel = File.Exists(modelDestinationPath)
                                   ? LoadModel(modelDestinationPath, mlContext)
                                   : TrainModel(trainSet, validationSet, workspaceRelativePath, mlContext);

            //ClassifyImages(mlContext, testSet, trainedModel);
            ClassifyImagesWithEngine(mlContext, testSet, trainedModel);

            mlContext.Model.Save(trainedModel, trainSet.Schema, modelDestinationPath);
        }

        private static void LoadData(string assetsRelativePath, MLContext mlContext, out IDataView trainSet, out IDataView validationSet, out IDataView testSet) {
            var images = LoadImagesFromDirectory(assetsRelativePath);
            var imageData = mlContext.Data.LoadFromEnumerable(images);
            var shuffledData = mlContext.Data.ShuffleRows(imageData);
            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Label", outputColumnName: "LabelAsKey")
                .Append(mlContext.Transforms.LoadRawImageBytes(inputColumnName: "ImagePath", outputColumnName: "Image", imageFolder: assetsRelativePath));

            var preprocessedData = preprocessingPipeline.Fit(shuffledData).Transform(shuffledData);
            var trainSplit = mlContext.Data.TrainTestSplit(data: preprocessedData, testFraction: 0.2);
            var validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet);

            trainSet = trainSplit.TrainSet;
            validationSet = validationTestSplit.TrainSet;
            testSet = validationTestSplit.TestSet;
        }

        private static IEnumerable<ImageData> LoadImagesFromDirectory(string folder) {
            return new DirectoryInfo(folder).EnumerateFiles("*.jpeg", SearchOption.AllDirectories).Select(x => new ImageData() {
                ImagePath = x.FullName,
                Label = x.Directory.Name
            });
        }

        private static ITransformer LoadModel(string path, MLContext mlContext) {
            return mlContext.Model.Load(path, out _);
        }

        private static ITransformer TrainModel(IDataView trainSet, IDataView validationSet, string workspaceRelativePath, MLContext mlContext) {
            var classifierOptions = new ImageClassificationTrainer.Options() {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelAsKey",
                ValidationSet = validationSet,
                Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                MetricsCallback = Console.WriteLine,
                TestOnTrainSet = false,
                ReuseTrainSetBottleneckCachedValues = true,
                ReuseValidationSetBottleneckCachedValues = true,
                WorkspacePath = workspaceRelativePath
            };

            var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // takes approx. ~ 31min for 10 classes, 18300 images
            ITransformer trainedModel;
            using (new PerformanceTimer("DNN Image classifier training"))
                trainedModel = trainingPipeline.Fit(trainSet);

            return trainedModel;
        }

        public static void ClassifyImages(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            var predictionData = trainedModel.Transform(data);
            var predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionData, reuseRowObject: true);
            Console.WriteLine("Classifying multiple images");
            foreach (var prediction in predictions)
            {
                OutputPrediction(prediction);
            }
        }

        public static void ClassifyImagesWithEngine(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);
            var inputs = mlContext.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: true);
            Console.WriteLine("Classifying multiple images using prediction engine.");
            foreach (var input in inputs) {
                // Single prediction takes approx. ~ 90ms
                ModelOutput prediction;
                using (new PerformanceTimer("Single prediction"))
                    prediction = predictionEngine.Predict(input);
                OutputPrediction(prediction);
            }
        }

        private static void OutputPrediction(ModelOutput prediction)
        {
            var imageName = Path.GetFileName(prediction.ImagePath);
            Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}{(prediction.Label != prediction.PredictedLabel ? " WRONG" : string.Empty)}");
        }
    }
}
