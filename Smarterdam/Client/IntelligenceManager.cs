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
        private int trainingSize;

        private DateTime trainUntil;

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

        public IntelligenceManager()
        {
        }

        public virtual PipelinePack ComposePipeline(Commands command, string id)
        {
            trainUntil = DateTime.Now.AddDays(-3);

            List<List<float>> DbRealValue = new List<List<float>>();
            List<DateTime> DbDateTime = new List<DateTime>();

            var onlinePipeline = ComposeOnPipeline(id);

            var offlinePipeline = ComposeOffPipeline();

            var databasePipeline = ComposeDbPipeline();

            return new PipelinePack { OnlinePipeline = onlinePipeline, OfflinePipeline = offlinePipeline, DatabasePipeline = databasePipeline };
        }

        public StreamPipeline UpdatePipeline(StreamPipeline sourcePipeline, Command command)
        {
            return sourcePipeline;
        }

        protected virtual StreamPipeline ComposeOnPipeline(string id)
        {
            var onlinePipeline = new StreamPipeline();

            parameters = new FilterParameters();

            trainingSize = 1056;

            onlinePipeline.Register(new onNeuralPredictionFilter(parameters, trainUntil));
            onlinePipeline.Register(new onErrorCalculationFilter(parameters));

            IForecastResultRepository repo = new MongoDbForecastResultRepository();
            onlinePipeline.Register(new ResultOutputFilter(repo) { MeasurementId = Int32.Parse(id)});

            return onlinePipeline;
        }

        protected virtual StreamPipeline ComposeOffPipeline()
        {
            var offlinePipeline = new StreamPipeline();

            Dictionary<string, int> time = new Dictionary<string, int>();

            offlinePipeline.Register(new offPredictionFittingFilter(parameters, trainUntil, time));

            return offlinePipeline;
        }

        protected virtual StreamPipeline ComposeDbPipeline()
        {
            return new StreamPipeline();
        }
    }
}
