using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Smarterdam.DataAccess.DbEntities;
using Smarterdam.Entities;

namespace Smarterdam.DataAccess
{
    public class MongoDbForecastResultRepository : IForecastResultRepository
    {
        private const string ConnectionString = "mongodb://localhost/?safe=true";
        private readonly MongoServer server;
        private MongoCollection<DbForecastResult> resultCollection;
        
        public MongoDbForecastResultRepository()
        {
            var client = new MongoClient(ConnectionString);
            server = client.GetServer();
            var smarterdamDb = server.GetDatabase("smarterdam");
            resultCollection = smarterdamDb.GetCollection<DbForecastResult>("forecast_results");
        }

        public void Purge(int id)
        {
            var query = Query.EQ("MeasurementID", id);
            resultCollection.Remove(query);
        }

        public void Add(ForecastResult result)
        {
            resultCollection.Save(new DbForecastResult(result));
        }

        public IEnumerable<ForecastResult> GetAll(int measurementId)
        {
            var query = Query.EQ("MeasurementID", measurementId);
            foreach (var result in resultCollection.Find(query))
            {
                yield return result.ConvertBack();
            }
        }

        public ForecastResult GetLast(int measurementId)
        {
            var query = Query.EQ("MeasurementID", measurementId);

            var lastEntry = resultCollection.Find(query).OrderBy(x => x.TimeStamp).LastOrDefault();

            return lastEntry.ConvertBack();
        }

        public IEnumerable<int> GetTasks()
        {
            var taskIds = resultCollection.FindAll().Select(x => x.MeasurementID).Distinct();
            return taskIds;
        }
    }
}
