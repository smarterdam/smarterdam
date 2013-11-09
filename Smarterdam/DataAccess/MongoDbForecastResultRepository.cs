using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using Smarterdam.DataAccess.DbEntities;
using Smarterdam.Entities;
using Smarterdam.Log;

namespace Smarterdam.DataAccess
{
    public class MongoDbForecastResultRepository : IForecastResultRepository
    {
        private const string ConnectionString = "mongodb://localhost/?safe=true";
        private readonly MongoServer server;
        private MongoCollection<DbForecast> forecastCollection;
        
        public MongoDbForecastResultRepository()
        {
            var client = new MongoClient(ConnectionString);
            server = client.GetServer();
            var smarterdamDb = server.GetDatabase("smarterdam");
            forecastCollection = smarterdamDb.GetCollection<DbForecast>("forecasts");
        }

        public void Purge(int id)
        {
            var query = Query.EQ("MeasurementId", id);
            forecastCollection.Remove(query);
        }

        public Forecast Create(int id)
        {
            var forecast = new Forecast() { MeasurementId = id };
            var dbForecast = new DbForecast(forecast);
            forecastCollection.Save(dbForecast);
            return forecast;
        }

        public void Add(int measurementId, ForecastResult result)
        {
            var forecast = forecastCollection.AsQueryable().FirstOrDefault(x => x.MeasurementId == measurementId);
            forecast.Results.Add(new DbForecastResult(result));
            forecast.Error = result.Error;
            forecastCollection.Save(forecast);
        }

        public Forecast Get(int measurementId)
        {
            var forecast = forecastCollection.AsQueryable().FirstOrDefault(x => x.MeasurementId == measurementId);
            return forecast.ConvertBack();
        }

        public ForecastResult GetLast(int measurementId)
        {
            Logging.Debug("Start 'GetLast' query");

            var forecast = forecastCollection.AsQueryable().FirstOrDefault(x => x.MeasurementId == measurementId);
            var lastEntry = forecast.Results.LastOrDefault();
            Logging.Debug("Finished 'GetLast' query");

            return lastEntry.ConvertBack();
        }

        public IEnumerable<int> GetTasks()
        {
            var taskIds = forecastCollection.FindAll().Select(x => x.MeasurementId).Distinct();
            return taskIds;
        }
    }
}
