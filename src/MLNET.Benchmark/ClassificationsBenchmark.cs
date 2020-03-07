using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.ML;
using MLNET.Core;

namespace MLNET.Benchmark
{
    [Orderer(SummaryOrderPolicy.Default)]
    [MemoryDiagnoser]
    public class ClassificationsBenchmark
    {
        private PredictionEngine<SpamInput, SpamPrediction> _predictionEngine;
        private ITransformer _transformer;
        private MLContext _mlContext;


        private List<SpamInput> _spamInputs;
        public string Message = "Message to predict";

        [Params(10, 100, 1000, 10000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var filePath = $"{Environment.CurrentDirectory}\\SpamDetectorModel.zip";
            _mlContext = new MLContext();
            _transformer = _mlContext.Model.Load(filePath, out var schema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(_transformer, schema);

            var spamInputs = new List<SpamInput>();
            for (int i = 0; i < N; i++)
            {
                spamInputs.Add(new SpamInput(){Message = Message});
            }

            _spamInputs = spamInputs;
        }

        [Benchmark(Baseline = true)]
        public void PredictionEngine()
        {
            foreach (var spamInput in _spamInputs)
            {
                _predictionEngine.Predict(spamInput);
            }
        }

        [Benchmark]
        public void Transfomer()
        {
            _transformer.Transform(_mlContext.Data.LoadFromEnumerable(_spamInputs));
        }
    }
}