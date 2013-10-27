using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.DataAccess;
using Smarterdam.Filters;
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class IntelligenceManager : IIntelligenceManager
    {
        private FilterParameters parameters;
        private readonly IForecastResultRepository resultsRepository;
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

        public IntelligenceManager(IForecastResultRepository repository, ITestStartDateProvider testStartDateProvider)
        {
            this.resultsRepository = repository;
            this.testStartDateProvider = testStartDateProvider;
        }

        public virtual PipelinePack ComposePipeline(Commands command, string id)
        {
            List<List<float>> DbRealValue = new List<List<float>>();
            List<DateTime> DbDateTime = new List<DateTime>();

            var _id = Int32.Parse(id);

            var onlinePipeline = ComposeOnPipeline(_id);

            var offlinePipeline = ComposeOffPipeline(_id);

            var databasePipeline = ComposeDbPipeline();

            return new PipelinePack { OnlinePipeline = onlinePipeline, OfflinePipeline = offlinePipeline, DatabasePipeline = databasePipeline };
        }

        public StreamPipeline UpdatePipeline(StreamPipeline sourcePipeline, Command command)
        {
            return sourcePipeline;
        }

        protected virtual StreamPipeline ComposeOnPipeline(int id)
        {
            var onlinePipeline = new StreamPipeline();

            parameters = new FilterParameters();

            var trainUntil = testStartDateProvider.GetTimestampOfTestStart(id);

            onlinePipeline.Register(new onNeuralPredictionFilter(parameters, trainUntil));
            onlinePipeline.Register(new onErrorCalculationFilter(parameters));
            
            onlinePipeline.Register(new ResultOutputFilter(resultsRepository) { MeasurementId = id});

            return onlinePipeline;
        }

        protected virtual StreamPipeline ComposeOffPipeline(int id)
        {
            var offlinePipeline = new StreamPipeline();

            Dictionary<string, int> time = new Dictionary<string, int>();

            var trainUntil = testStartDateProvider.GetTimestampOfTestStart(id);

            offlinePipeline.Register(new offPredictionFittingFilter(parameters, trainUntil, time));

            return offlinePipeline;
        }

        protected virtual StreamPipeline ComposeDbPipeline()
        {
            return new StreamPipeline();
        }
    }
}
