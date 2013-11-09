using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Entities;
using System.IO;

namespace Smarterdam.DataAccess
{
    public class FileForecastResultRepository : IForecastResultRepository
    {
        private string dirPath = @"C:/Output_files/";
        private string filePathFormat;

        public FileForecastResultRepository()
        {
            filePathFormat = dirPath + "{0}.txt";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public void Purge(int id)
        {
            var filePath = String.Format(filePathFormat, id);
            if(File.Exists(filePath)) File.Delete(filePath);
        }

        public void Add(ForecastResult result)
        {
            var fileName = result.MeasurementID.ToString();
            var filePath = String.Format(filePathFormat, fileName);

            StreamWriter sw;
            if (!File.Exists(filePath)) sw = new StreamWriter(File.Create(filePath));
            else sw = File.AppendText(filePath);

            var line = CreateLine(result);

            sw.WriteLine(line);
            sw.Close();
        }

        public IEnumerable<int> GetTasks()
        {
            var di = new DirectoryInfo(dirPath);
            return di.GetFiles().Select(x => Int32.Parse(x.Name.Replace(x.Extension, "")));
        }

        public IEnumerable<ForecastResult> GetAll(int measurementId)
        {
            var filePath = String.Format(filePathFormat, measurementId);

            if (!File.Exists(filePath)) yield return null;

            var lines = System.IO.File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var res = ParseLine(line);
                res.MeasurementID = measurementId;
                yield return res;
            }
        }

        public ForecastResult GetLast(int measurementId)
        {
            var filePath = String.Format(filePathFormat, measurementId);

            if (!File.Exists(filePath)) return null;

            var line = System.IO.File.ReadAllLines(filePath).Last();
            var res = ParseLine(line);

            res.MeasurementID = measurementId;

            return res;
        }

        private string CreateLine(ForecastResult result)
        {
            try
            {
                var errStr = result.Error;
                var error = errStr.HasValue ? errStr.Value.ToString("0.00") : "N/A";

                var message = String.Format("{0};{1};{2};{3};",
                                            result.TimeStamp,
                                            result.RealValue,
                                            result.PredictedValue.Value.ToString("0.00"),
                                            error);

                return message;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ForecastResult ParseLine(string line)
        {
            try
            {
                var parts = line.Split(';');
                var result = new ForecastResult();

                result.TimeStamp = DateTime.ParseExact(parts[0], "dd.MM.yyyy H:mm:ss", null);
                result.RealValue = Double.Parse(parts[1]);
                result.PredictedValue = Double.Parse((parts[2]));

                double error;
                result.Error = Double.TryParse(parts[3], out error) && !Double.IsNaN(error) ? 
                    error : (double?)null;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void Add(int measurementId, ForecastResult result)
        {
            throw new NotImplementedException();
        }

        public Forecast Get(int measurementId)
        {
            throw new NotImplementedException();
        }


        public Forecast Create(int id)
        {
            throw new NotImplementedException();
        }
    }
}
