using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Vision;

namespace MLNET.ImageClassification
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
            var workspaceRelativePath = Path.Combine(projectDirectory, "workspace");
            var assetsRelativePath = Path.Combine(projectDirectory, "Data");
            IEnumerable<ImageData> images = LoadImagesFromDirectory(assetsRelativePath);

            var mlContext = new MLContext();
            var imageData = mlContext.Data.LoadFromEnumerable(images);
            var shuffledData = mlContext.Data.ShuffleRows(imageData);
            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Label", outputColumnName: "LabelAsKey")
                .Append(mlContext.Transforms.LoadRawImageBytes(inputColumnName: "ImagePath", outputColumnName: "Image", imageFolder:assetsRelativePath));

            var preprocessedData = preprocessingPipeline.Fit(shuffledData).Transform(shuffledData);
            DataOperationsCatalog.TrainTestData trainSplit = mlContext.Data.TrainTestSplit(data: preprocessedData, testFraction: 0.15);
            DataOperationsCatalog.TrainTestData validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet);

            IDataView trainSet = trainSplit.TrainSet;
            IDataView validationSet = validationTestSplit.TrainSet;
            IDataView testSet = validationTestSplit.TestSet;

            var classifierOptions = new ImageClassificationTrainer.Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelAsKey",
                ValidationSet = validationSet,
                Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                MetricsCallback = Console.WriteLine,
                //BatchSize = 100,
                TestOnTrainSet = false,
                ReuseTrainSetBottleneckCachedValues = true,
                ReuseValidationSetBottleneckCachedValues = true,
                WorkspacePath = workspaceRelativePath
            };

            var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            ITransformer trainedModel = trainingPipeline.Fit(trainSet);

            ClassifyImages(mlContext, testSet, trainedModel);

            mlContext.Model.Save(trainedModel, trainSet.Schema, @"C:\Users\novotnyv\Documents\YSoft\mlnet-meetup\src\MLNET.ImageClassification\Model\model.zip");
        }

        public static void ClassifyImages(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            IDataView predictionData = trainedModel.Transform(data);
            IEnumerable<ModelOutput> predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionData, reuseRowObject: true);
            Console.WriteLine("Classifying multiple images");
            foreach (var prediction in predictions)
            {
                OutputPrediction(prediction);
            }
        }

        private static void OutputPrediction(ModelOutput prediction)
        {
            string imageName = Path.GetFileName(prediction.ImagePath);
            Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}");
        }

        private static IEnumerable<ImageData> LoadImagesFromDirectory(string folder) {
            return new DirectoryInfo(folder).EnumerateFiles("*.jpeg", SearchOption.AllDirectories).Select(x => new ImageData() {
                ImagePath = x.FullName,
                Label = x.Directory.Name
            });
        }
    }

    internal class ModelInput
    {
        public byte[] Image { get; set; }

        public UInt32 LabelAsKey { get; set; }

        public string ImagePath { get; set; }

        public string Label { get; set; }
    }

    internal class ModelOutput
    {
        public string ImagePath { get; set; }

        public string Label { get; set; }

        public string PredictedLabel { get; set; }
    }

    internal class ImageData
    {
        public string ImagePath { get; set; }
        public string Label { get; set; }

    }
}
