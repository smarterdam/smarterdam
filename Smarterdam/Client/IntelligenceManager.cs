using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoRepository;
using Smarterdam.Entities;
using Smarterdam.Filters;
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class IntelligenceManager : IIntelligenceManager
    {
        private FilterParameters parameters;
        private readonly MongoRepository<Measurement> repository = new MongoRepository<Measurement>("mongodb://localhost/smarterdam", "measurements");
        private readonly ITestStartDateProvider testStartDateProvider;

        public class Entity
        {
            //public ObjectId _id { get; set; }
            public string IDD { get; set; }
            public string Date { get; set; }
            public string RealValue { get; set; }
            public string PredictedValue { get; set; }
            public string Theta { get; set; }
            public string Validity { get; set; }
        }

        public IntelligenceManager(ITestStartDateProvider testStartDateProvider)
        {
            this.testStartDateProvider = testStartDateProvider;
        }

        public virtual PipelinePack ComposePipeline(string id, string name)
        {
            List<List<float>> DbRealValue = new List<List<float>>();
            List<DateTime> DbDateTime = new List<DateTime>();

            var exchange = new DataExchange();

            var onlinePipeline = ComposeOnPipeline(id, exchange, name);

            var offlinePipeline = ComposeOffPipeline(id, exchange);

            var databasePipeline = ComposeDbPipeline();

            return new PipelinePack { OnlinePipeline = onlinePipeline, OfflinePipeline = offlinePipeline, DatabasePipeline = databasePipeline };
        }

        public StreamPipeline UpdatePipeline(StreamPipeline sourcePipeline, Command command)
        {
            return sourcePipeline;
        }

        protected virtual StreamPipeline ComposeOnPipeline(string id, DataExchange exchange, string name)
        {
            var onlinePipeline = new StreamPipeline();

            parameters = new FilterParameters();

            var trainUntil = testStartDateProvider.GetTimestampOfTestStart(id);

            onlinePipeline.Register(new onNeuralPredictionFilter(parameters, trainUntil, exchange));
            onlinePipeline.Register(new onErrorCalculationFilter(parameters));
            
            onlinePipeline.Register(new ResultOutputFilter(repository) { MeasurementId = id, ForecastModelId = name});

            return onlinePipeline;
        }

		protected virtual StreamPipeline ComposeOffPipeline(string id, DataExchange exchange)
        {
            var offlinePipeline = new StreamPipeline();

            Dictionary<string, int> time = new Dictionary<string, int>();

            var trainUntil = testStartDateProvider.GetTimestampOfTestStart(id);

            offlinePipeline.Register(new offPredictionFittingFilter(parameters, trainUntil, time, exchange));

            return offlinePipeline;
        }

        protected virtual StreamPipeline ComposeDbPipeline()
        {
            return new StreamPipeline();
        }
    }
}
