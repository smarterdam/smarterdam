using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Smarterdam.Client;
using Smarterdam.DataAccess;
using Smarterdam.DataSource;
using Smarterdam.Entities;
using Smarterdam.Log;
using Smarterdam.Server;
using Smarterdam.UI;
using Smarterdam.Web.ViewModels;

namespace Smarterdam.Web.Controllers
{
    public class HomeController : Controller
    {
        private IForecastResultRepository repository = null;

        public HomeController()
        {
            //repository = new FileForecastResultRepository();
            repository = new MongoDbForecastResultRepository();
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

            model.CurrentTasks = repository.GetTasks().Select(x => x.ToString()).ToList();
            return View(model);
        }

        [Authorize]
        public void Go(string id)
        {
            var _id = Int32.Parse(id);

            Logging.Info("entered Go");
            
            var query = "db.ownerMax.streamA.SAVE";
            
            Logging.Info("manager created");

            var server = SmarterdamFactory.CreateServer();
            server.Start(_id);

            var client = SmarterdamFactory.CreateClient();
            
            client.Start(query, id);
        }

        [Authorize]
        public void BatchGo(int quantity = 0)
        {
            //var buildings = quantity > 0 ? GetBuildingIdList().Take(quantity) : GetBuildingIdList();
            var buildings = GetBuildingIdList();
            foreach (var buildingId in buildings)
            {
                Go(buildingId);
            }
        }

        [Authorize]
        public ActionResult BatchResults(int cat = 50)
        {
            var model = new StatusViewModel();
            model.ChartData = "";

            try
            {
                var tasks = repository.GetTasks();

                var values = new List<ForecastResult>();

                foreach (var task in tasks)
                {
                    var forecastResult = repository.GetLast(task);
                    if (forecastResult.Error.HasValue && !double.IsNaN(forecastResult.Error.Value))
                    {
                        values.Add(forecastResult);
                    }
                }

                var errors = values.Select(x => x.Error.Value);
                var minError = errors.Min();
                if (double.IsNaN(minError)) minError = 0;
                var maxError = errors.Max();
                var gap = (maxError - minError) / cat;

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
                foreach (var q in buckets)
                {
                    i++;
                    dynamic value = new ExpandoObject();

                    upperBound = minError + i * gap;
                    value.Error = String.Format("{0} - {1}", (lowerBound / 100).ToString("00.0000"), (upperBound / 100).ToString("00.0000"));
                    lowerBound = upperBound;

                    value.Quantity = q;
                    result.Add(value);
                }

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

        [Authorize]
        public ActionResult Status(string id)
        {
            var model = new StatusViewModel();
            model.ChartData = "";

            try
            {
                var results = repository.GetAll(Int32.Parse(id));
                var values = new List<dynamic>();

                foreach (var result in results.OrderBy(x => x.TimeStamp))
                {
                    dynamic value = new ExpandoObject();
                    value.TimeStamp = result.TimeStamp.ToString("MM.dd.yy HH:mm:ss");
                    value.ValueReal = result.RealValue.ToString().Replace(',', '.');

                    values.Add(value);
                }

                model.ChartData = JsonConvert.SerializeObject(values);

                return View(model);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
