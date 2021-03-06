﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Smarterdam.Api;
using Smarterdam.Api.Helpers;
using Smarterdam.DataSource.EcoSCADAService;
using Smarterdam.Log;

namespace Smarterdam.DataSource
{
    public class EcoScadaDataSource : IDataSource
    {
        //private int measurementId;
        private string ACCESS_GUID = PasswordHelper.EcoScadaServicePassword;
        private string SERVICE_URI = "http://flexible.ecoscada.com/service_test/EcoSCADAService.svc";

        private EcoSCADADataClient client;

        private DateTime lastReceivedDateTime = DateTime.Now;

        public EcoScadaDataSource()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            
            var endpointAddress = new EndpointAddress(SERVICE_URI);

            client = new EcoSCADADataClient(binding, endpointAddress);
            
        }

        public bool HasNewData(int measurementId)
        {
            return HasNewData(lastReceivedDateTime, measurementId);
        }

        public bool HasNewData(DateTime sinceWhen, int measurementId)
        {
            var response = GetMeasurementData(measurementId, sinceWhen, DateTime.Now);

            Logging.Info("response length: " + response.Length);

            var newData = response.Where(x => x.TimeStamp > ConvertTimeStampToRemote(sinceWhen));

            return newData.Any();
        }

        public IEnumerable<DataStreamUnit> GetNewData(int measurementId)
        {
            var newData = GetNewData(lastReceivedDateTime, measurementId);

            if (newData.Any())
            {
                lastReceivedDateTime = ConvertTimeStampToLocal(newData.Last().TimeStamp);
            }

            return newData;
        }

        public void SetDate(DateTime newDate)
        {
            lastReceivedDateTime = newDate;
        }

        /// <summary>
        /// Подкорректировать время из российского в бельгийское
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private DateTime ConvertTimeStampToRemote(DateTime source)
        {
            return source.AddHours(2);
        }

        /// <summary>
        /// Подкорректировать время из бельгийского в российское
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private DateTime ConvertTimeStampToLocal(DateTime source)
        {
            return source.AddHours(-2);
        }

        private DateTime ConvertDateForService(DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second);
        }

        public IEnumerable<DataStreamUnit> GetNewData(DateTime sinceWhen, int measurementId)
        {
            var response = GetMeasurementData(measurementId, sinceWhen, DateTime.Now);

            var newData = response.Where(x => x.TimeStamp > ConvertTimeStampToRemote(sinceWhen));

            return newData.Select(x =>
            {
                var values = new ConcurrentDictionary<string, object>();
                values["Value"] = x.Value;
                values["TimeStamp"] = x.TimeStamp;

                return new DataStreamUnit() { Values = values, TimeStamp = x.TimeStamp };
            });
        }

        public DateTime GetLastTimestamp(int measurementId)
        {
            return ConvertDateForService(DateTime.Now);
        }

        private CompositeType[] GetMeasurementData(int measurementId, DateTime from, DateTime to)
        {
            try
            {
                return client.GetMeasurementsPerPeriod(measurementId, ConvertDateForService(from),
                                                       ConvertDateForService(to), null, ACCESS_GUID);
            }
            catch (System.ServiceModel.FaultException)
            {
                return new CompositeType[0];
            }
        }
    }
}
