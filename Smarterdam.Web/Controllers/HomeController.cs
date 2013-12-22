using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Ninject;
using Smarterdam.Client;
using Smarterdam.DataSource;
using Smarterdam.Entities;
using Smarterdam.Log;
using Smarterdam.Server;
using Smarterdam.UI;
using Smarterdam.Web.ViewModels;
using Smarterdam.DataAccess;

namespace Smarterdam.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Func<ISmarterdamClient> clientFunc;
        private readonly ISmarterdamServer server;
        private readonly IRepository<Measurement> repository;
        private readonly ITestStartDateProvider testStartDateProvider;

        private static object gate = new object();
        
        [Inject]
        public HomeController(Func<ISmarterdamClient> clientFunc, ISmarterdamServer server, IRepository<Measurement> repository, ITestStartDateProvider testStartDateProvider)
        {
            this.clientFunc = clientFunc;
            this.server = server;
            this.repository = repository;
            this.testStartDateProvider = testStartDateProvider;
        }
        //
        // GET: /Home/

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var model = new IndexViewModel();
            var lines = GetBuildingIdList();

            model = new IndexViewModel();
            model.Measurements = lines.Select(x => new SelectListItem() { Text = x, Value = x });

            model.CurrentTasks = repository.Select(x => x.MeasurementId).ToList().OrderBy(x => Int32.Parse(x)).ToList();
            return View(model);
        }

        [Authorize]
        public void Go(string id)
        {
            var _id = Int32.Parse(id);

            var query = "db.ownerMax.streamA.SAVE";


            server.Start(_id);

            var client = clientFunc();
            client.Start(query, id);
        }

        [Authorize]
        public void BatchGo(int quantity = 0)
        {
            lock (gate)
            {
                Logging.Debug("Started BatchGo");
                var buildings = GetBuildingIdList();
                foreach (var buildingId in buildings)
                {
                    Logging.Debug("Starting Go for {0}", buildingId);
                    Go(buildingId);
                    Logging.Debug("Finished Go for {0}", buildingId);
                }
            }
        }

        [Authorize]
        public ActionResult BatchResults(int cat = 50)
        {
            Logging.Debug("Entered BatchResults");
            var model = new ChartViewModel();
            model.ChartData = "";

            try
            {
                var tasks = repository.ToList();

                Logging.Debug("Tasks length:{0}", tasks.Count());

                var values = new List<Forecast>();

                foreach (var task in tasks)
                {
                    Logging.Debug("Start task {0}", task);
                    var forecast = task.Forecasts.FirstOrDefault();
                    Logging.Debug("Forecast result error: {0}", forecast.Error);
                    if (forecast.Error.HasValue && !double.IsNaN(forecast.Error.Value))
                    {
                        values.Add(forecast);
                    }
                }

                Logging.Debug("Foreach finished");
                var errors = values.Select(x => x.Error.Value);
                var minError = errors.Min();
                if (double.IsNaN(minError)) minError = 0;
                var maxError = errors.Max();
                var gap = (maxError - minError) / cat;

                Logging.Debug("Start bucketing");

                var buckets = new int[cat];
                foreach (var value in values)
                {
                    var bucket = gap > 0 ? (int)Math.Floor((value.Error.Value - minError) / gap) : 0;
                    if (bucket == cat) bucket--;
                    buckets[bucket]++;
                }

                var result = new List<dynamic>();
                int i = 0;
                double lowerBound = 0;
                double upperBound = 0;
                Logging.Debug("starting foreach");
                foreach (var q in buckets)
                {
                    Logging.Debug("Bucket {0}", q);
                    i++;
                    dynamic value = new ExpandoObject();

                    upperBound = minError + i * gap;
                    value.Error = String.Format("{0} - {1}", (lowerBound / 100).ToString("00.0000"), (upperBound / 100).ToString("00.0000"));
                    lowerBound = upperBound;

                    value.Quantity = q;
                    result.Add(value);
                }

                Logging.Debug("Finished bucketing");

                model.ChartData = JsonConvert.SerializeObject(result);

                return View(model);
            }

            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        IEnumerable<string> GetBuildingIdList()
        {
            return System.IO.File.ReadAllLines(Server.MapPath("~") + "/App_Data/buildingData.csv")
                      .Select(a => a.Split(';')[0]);
        }

        private ChartViewModel GetChart(Forecast forecast)
        {
            var model = new ChartViewModel();
            model.ChartName = forecast.ForecastModelId;

            var results = forecast.Results;
            var values = GetValuesForChart(results);

            model.ChartData = JsonConvert.SerializeObject(values);
            var lastDate = forecast.Results.LastOrDefault().TimeStamp;
            //lastDate = lastDate.AddHours(12).Date; //округляем до ближайшего дня
            model.Error = CalculateMAPE(forecast.Results.SkipWhile(x => x.TimeStamp < testStartDateProvider.GetTimestampOfTestStart(lastDate)));

            return model;
        }

        private List<dynamic> GetValuesForChart(IEnumerable<ForecastResult> forecastResults)
        {
            var values = new List<dynamic>();

            foreach (var result in forecastResults.OrderBy(x => x.TimeStamp))
            {
                dynamic value = new ExpandoObject();
				value.TimeStamp = result.TimeStamp.ToString("yyyy-MM-ddTHH:mm:ss");
                value.ValueReal = result.RealValue.ToString().Replace(',', '.');
                value.ValuePredicted = result.PredictedValue.HasValue ? result.PredictedValue.ToString().Replace(',', '.') : null;

                values.Add(value);
            }
            
            return values;
        }

        private ChartViewModel GetWinnerModelsChart(List<Tuple<DateTime, int>> winnerModels)
        {
            var model = new ChartViewModel();
            model.ChartName = "Winner models chart";

            var values = new List<dynamic>();

            foreach (var winner in winnerModels)
            {
                dynamic value = new ExpandoObject();
                value.TimeStamp = winner.Item1;
                value.ValueReal = winner.Item2;

                values.Add(value);
            }

            model.ChartData = JsonConvert.SerializeObject(values);

            return model;
        }

        private ChartViewModel GetTrustIndexChart(string id, out List<Tuple<DateTime, int>> winnerModels)
        {
            winnerModels = new List<Tuple<DateTime, int>>();
            var model = new ChartViewModel();
            model.ChartName = "Trust Index model";

            try
            {
                var forecasts = repository.FirstOrDefault(x => x.MeasurementId == id).Forecasts;
                var startForecastingDate =
                    forecasts.Select(x => x.Results.FirstOrDefault(r => r.PredictedValue != null).TimeStamp).Max();
                    //дата, начиная с которой будем использовать метод trust index

                var lastDate = forecasts[0].Results.LastOrDefault().TimeStamp;
                var startTestDate = testStartDateProvider.GetTimestampOfTestStart(lastDate);
                startForecastingDate = startForecastingDate.AddHours(12).Date; //округляем до ближайшего дня

                var values = new List<ForecastResult>();
                values.AddRange(forecasts[0].Results.TakeWhile(x => x.TimeStamp < startForecastingDate).Select(x =>
                    {
                        x.PredictedValue = null;
                        return x;
                    }));

                var forecastResults = forecasts.Select(x => x.Results.SkipWhile(y => y.TimeStamp < startForecastingDate).ToList()).ToList();

                var trustIndexDict = new Dictionary<int, int>();
                for (int i = 0; i < forecastResults.Count(); i++)
                {
                    trustIndexDict.Add(i, 0);
                }

                var currentDay = startForecastingDate.AddDays(1);

                var errorSum = 0.0;
                var counter = 0;

                while (currentDay.Date <= lastDate.AddDays(1))
                {
                    var innerCurrentDay = currentDay;
                    var nextDay = forecastResults.Select(x => x.TakeWhile(r => r.TimeStamp < innerCurrentDay).ToList()).ToList();
                    forecastResults = forecastResults.Select(x => x.SkipWhile(r => r.TimeStamp < innerCurrentDay).ToList()).ToList();
                    var errors = nextDay.Select(x => CalculateMAPE(x)).ToList();
                    var minError = errors.Min();
                    var winningModelIndex = errors.FindIndex(x => x == minError);
                    trustIndexDict[winningModelIndex]++;

                    var mostTrustedModelIndex = currentDay.Day % 2; // trustIndexDict.FirstOrDefault(x => x.Value == trustIndexDict.Max(y => y.Value)).Key;
                    winnerModels.Add(new Tuple<DateTime, int>(innerCurrentDay.AddDays(-1), mostTrustedModelIndex));

                    values.AddRange(nextDay[mostTrustedModelIndex]);

                    if (currentDay >= startTestDate)
                    {
                        counter++;
                        errorSum += minError;
                    }

                    currentDay = currentDay.AddDays(1);
                }

                model.ChartData = JsonConvert.SerializeObject(GetValuesForChart(values));
                model.Error = CalculateMAPE(values.SkipWhile(x => x.TimeStamp < startTestDate));

                return model;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private double CalculateMAPE(IEnumerable<ForecastResult> source)
        {
            var sum = 0.0;
            var counter = 0;

            foreach (var forecastResult in source)
            {
                if (forecastResult.RealValue > 0 && forecastResult.PredictedValue.HasValue)
                {
                    counter++;
                    sum += Math.Abs(forecastResult.RealValue.Value - forecastResult.PredictedValue.Value)/forecastResult.RealValue.Value;
                }
            }

            return sum/counter;
        }

        [Authorize]
        public ActionResult Status(string id)
        {
            var model = new StatusViewModel();
            model.BuildingId = id;
            
            try
            {
				var forecasts = repository.FirstOrDefault(x => x.MeasurementId == id).Forecasts;
				foreach (var forecast in forecasts)
				{
					model.Charts.Add(GetChart(forecast));
				}

                List<Tuple<DateTime, int>> winnerModels;
                model.Charts.Add(GetTrustIndexChart(id, out winnerModels));
                model.Charts.Add(GetWinnerModelsChart(winnerModels));

                int i = 0;
                foreach (var chart in model.Charts)
                {
                    chart.Id = (i++).ToString();
                }

                return View(model);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
