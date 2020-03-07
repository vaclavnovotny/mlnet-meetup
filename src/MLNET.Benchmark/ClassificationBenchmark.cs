using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.ML;
using MLNET.Core;

namespace MLNET.Benchmark
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class ClassificationBenchmark
    {
        private PredictionEngine<SpamInput, SpamPrediction> _predictionEngine;


        [GlobalSetup]
        public void Setup()
        {
            var filePath = $"{Environment.CurrentDirectory}\\SpamDetectorModel.zip";
            var mlContext = new MLContext();
            var transformer = mlContext.Model.Load(filePath, out var schema);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(transformer, schema);
        }

        [Params(
            "small message",
            "Congratulations!! You have won a brand new car! Contact our customer support for more information!",
            //100
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc scelerisque lectus eu risus sollicitudin, in laoreet purus vulputate. Etiam eu risus a dolor elementum bibendum. Mauris porta sed eros ac tristique. Suspendisse fringilla tempor tellus, ut venenatis ex sodales in. Pellentesque at felis accumsan enim dignissim volutpat. Aenean vitae dapibus.",
            //200
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc in feugiat quam. Curabitur gravida diam turpis. Sed tincidunt ex nec porta convallis. Integer suscipit quam ac cursus fringilla. Donec vitae quam id elit feugiat placerat. Maecenas nec nulla eros. Ut ornare mi sapien, viverra aliquet arcu pulvinar nec. Vestibulum non nulla id libero blandit euismod. Etiam fringilla pharetra velit, sed molestie enim fermentum at. Praesent molestie viverra risus. Praesent interdum nec neque vel dignissim. Maecenas pellentesque tincidunt interdum. Duis pellentesque libero non neque varius, sit amet aliquam ipsum mattis. Vivamus vestibulum consequat quam non condimentum. Nulla congue lorem ut nulla consectetur, quis malesuada enim convallis.\r\n\r\nInteger lobortis elit eu dignissim pretium. Nulla tristique nunc et ex iaculis, at viverra ex tristique. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer at consequat libero, quis ullamcorper mi. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nulla elementum risus quis orci pellentesque semper. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Cras porttitor massa eu augue euismod, vitae tempor lorem blandit. Aliquam venenatis, dolor quis tempor scelerisque, elit nisl venenatis quam, sit amet venenatis nulla mauris non metus. Nullam tincidunt lectus non rhoncus fringilla. Donec."
        )]
        public string Message;

        [Benchmark]
        public void Predict() => _predictionEngine.Predict(new SpamInput() {Message = Message});
    }
}