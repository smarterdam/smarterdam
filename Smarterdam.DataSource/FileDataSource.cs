using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.DataSource
{
    public class FileDataSource : IDataSource
    {
        StreamReader sr;
        string[] headers;

        public FileDataSource(string filePath)
        {
            sr = new StreamReader(filePath);

            headers = sr.ReadLine().Split(';');
        }

        public bool HasNewData(DateTime sinceWhen, int measurementId)
        {
            return HasNewData(measurementId);
        }

        public bool HasNewData(int measurementId)
        {
            var eof = sr.Peek() == -1;
            if (eof)
            {
                sr.Close();
            }
            return !eof;
        }

        public IEnumerable<DataStreamUnit> GetNewData(int measurementId)
        {
            var result = new List<DataStreamUnit>();

            if (HasNewData(DateTime.MinValue, measurementId))
            {
                var line = sr.ReadLine();

                var resultUnit = new DataStreamUnit();

                var values = line.Split(';').Where(x => !String.IsNullOrWhiteSpace(x));
                var paramIndex = 0;
                resultUnit.Values = new System.Collections.Concurrent.ConcurrentDictionary<string, object>(values.ToDictionary(x => headers[paramIndex++].Trim(), x => (object)x));

                resultUnit.TimeStamp = DateTime.Now; ;// sinceWhen;

                result.Add(resultUnit);
            }

            return result;
        }

        public IEnumerable<DataStreamUnit> GetNewData(DateTime sinceWhen, int measurementId)
        {
            return GetNewData(measurementId);
        }

        public void SetDate(DateTime newDate)
        {
            
        }
    }
}
