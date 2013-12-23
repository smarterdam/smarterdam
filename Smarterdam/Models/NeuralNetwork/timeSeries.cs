using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smarterdam.Models.NeuralNetwork
{
    /// <summary>
    /// class time series represents the array as a time series;
    /// </summary>
    public class TimeSeries
    {
        public List<double> timeseries = new List<double>();
        public List<int> status = new List<int>();
        public List<DateTime> timestamps = new List<DateTime>();
        public List<double> temperature = new List<double>();
        public List<int> cluster = new List<int>();




        //public void correctStatus()
        //{
        //    double maxValue = this.timeseries.Max();
        //    double threshold = maxValue - (80 * maxValue / 100);
        //    double currentValue = 0.0;
        //    int counter = 0;

        //    for (int i = 0; i < this.timeseries.Count(); i++)
        //    {
        //        currentValue = this.timeseries[i];
        //        if ((currentValue > threshold) && (this.status[i] == 0))
        //        {
        //            this.status[i] = 1;
        //            counter++;
        //        }
        //    }

        //    if (counter > 0)
        //        Console.WriteLine("Corrected " + counter.ToString() + "\r\n");
        //}

        /// <summary>
        /// TRUE = рабочий день
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //public bool getStatus(DateTime _date)
        //{
        //    return !(_date.DayOfWeek == DayOfWeek.Saturday || _date.DayOfWeek == DayOfWeek.Sunday);
        //}

        //public int getStatus(double _value)
        //{
        //    //по хорошему нужно реализовать расписание
        //    int result = 0;
        //    if (_value > 25.0)
        //        result = 1;
        //    return result;
        //}


        //public String getTimeAsAString(DateTime _dt)
        //{
        //    return String.Format("{0:HH:mm:ss}", _dt);
        //}

        //List<double> forecast = new List<double>();

        //Array timeSeries2 = Array.CreateInstance(typeof(Double), 2);


        //public bool checkForConsistency(int minutes)
        //{
        //    bool res = false;

        //    DateTime curDate = this.timestamps[0];
        //    DateTime expectedDate = curDate.AddMinutes(minutes);

        //    for (int i = 1; i < this.timeseries.Count(); i++)
        //    {
        //        curDate = this.timestamps[i];
        //        if (curDate != expectedDate)
        //        {
        //            Console.WriteLine("Fail in checking for " + curDate.ToString() + "; expected" + expectedDate.ToString());
        //            res = true;

        //        }
        //        expectedDate = curDate.AddMinutes(minutes);
        //    }
        //    return res;

        //}




        //private double getMedian()
        //{
        //    //timeseries.Sort()
        //    return 0;
        //}

        //private double getMean()
        //{
        //    double mean = 0.0;

        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        mean += timeseries[i];
        //    }
        //    mean = mean / timeseries.Count;
        //    return mean;
        //}

        /// <summary>
        /// 0 initialisation
        /// </summary>
        /// <param name="_raws"></param>
        public TimeSeries(int _raws)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            for (int i = 0; i < _raws; i++)
            {
                timestamps.Add(dt.AddHours(i));
                status.Add(0);
                timeseries.Add(0.0);
                temperature.Add(0.0);
                cluster.Add(0);
            }
        }


        /// <summary>
        /// Create time series from the file;
        /// </summary>
        /// <param name="_fileName"></param>
        /// <param name="_flagHeader"></param>
        /// <param name="_separator"></param>
        //public TimeSeries(String _fileName, bool _flagHeader, char _separator, int _competitionId)
        //{
        //    string[] strArr;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
        //    if (_competitionId == 1111)
        //    {//for GEFC
        //        try
        //        {
        //            FileStream fs = new FileStream(_fileName, FileMode.Open);
        //            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

        //            //read header;
        //            if (_flagHeader)
        //                strArr = sr.ReadLine().Split(_separator);
        //            int k = 0;
        //            while (sr.EndOfStream != true) // framework 2.0
        //            {
        //                strArr = sr.ReadLine().Split(_separator);
        //                //timestamps.Add(DateTime.Parse(strArr[0] + " " + strArr[1]));
        //                //Date & Time;
        //                timestamps.Add(DateTime.Parse(strArr[0].ToString() + " " + strArr[1].ToString()));


        //                //status.Add(Convert.ToBoolean(strArr[1]));
        //                //status.Add(getStatus(DateTime.Parse(strArr[0])));
        //                timeseries.Add(Convert.ToDouble(strArr[3]));
        //                //status.Add(Convert.ToInt32(strArr[2]));                    
        //                //status.Add(getStatus(Convert.ToDouble(strArr[2])));
        //                //Console.WriteLine("get record" + k.ToString());

        //                //we init all points are correct in case of absence the data.
        //                outlierLabel.Add(0);
        //                cluster.Add(0);
        //                k++;
        //            }


        //            sr.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }

        //        // last date
        //        //try
        //        //{
        //        //    lastDate = timestamps[timestamps.Count - 1];
        //        //    frequency = lastDate.Subtract(timestamps[timestamps.Count - 2]);
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    Console.WriteLine(ex.Message);
        //        //}

        //    }
        //    else
        //    {//for adriaan phd

        //        try
        //        {
        //            FileStream fs = new FileStream(_fileName, FileMode.Open);
        //            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

        //            //read header;
        //            if (_flagHeader)
        //                strArr = sr.ReadLine().Split(_separator);

        //            while (sr.EndOfStream != true) // framework 2.0
        //            {
        //                strArr = sr.ReadLine().Split(_separator);
        //                //timestamps.Add(DateTime.Parse(strArr[0] + " " + strArr[1]));
        //                timestamps.Add(DateTime.Parse(strArr[0]));
        //                //status.Add(Convert.ToBoolean(strArr[1]));

        //                timeseries.Add(Convert.ToDouble(strArr[2]));
        //                //status.Add(Convert.ToInt32(strArr[2]));                    

        //                status.Add(Convert.ToInt32(Convert.ToBoolean(strArr[1])));

        //                //status.Add(getStatus(DateTime.Parse(strArr[0])));


        //                //temperature.Add(Convert.ToDouble(strArr[3]));

        //                temperature.Add(0.0);
        //                //we init all points are correct in case of absence the data.
        //                outlierLabel.Add(0);
        //                cluster.Add(0);
        //            }


        //            sr.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }



        //    }
        //}

        /// <summary>
        /// do aggregation timeseries according to parameter p;
        /// 
        /// </summary>
        /// <param name="_p">integration parameter </param>
        /// <returns></returns>
        //public TimeSeries aggregate(int _p)
        //{

        //    if (_p > 0)
        //    {


        //        int length = this.timeseries.Count();
        //        TimeSeries ts = new TimeSeries(length / _p);
        //        int cx = 0;
        //        int k = 0; // counter of the new timeseries;

        //        double curValue = 0;
        //        DateTime curTS = this.timestamps[0];
        //        int curStatus = this.status[0];
        //        double curTemp = 0;

        //        for (int i = 0; i < length; i++)
        //        {
        //            if (cx == 0)
        //            {
        //                curTS = this.timestamps[i];
        //                curStatus = this.status[i];
        //            }
        //            curValue += this.timeseries[i];
        //            curTemp += this.temperature[i];

        //            cx++;
        //            if (cx == _p)
        //            {

        //                ts.timestamps[k] = curTS;
        //                ts.timeseries[k] = curValue;
        //                ts.status[k] = curStatus;
        //                ts.temperature[k] = curTemp / _p;

        //                cx = 0;
        //                curValue = 0.0;
        //                k++;
        //            }
        //        }
        //        return ts;
        //    }
        //    else
        //    {
        //        return null;
        //    }


        //}

        /// <summary>
        /// формирование выборки данных.Точка старта,Точка финиша
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_length"></param>
        /// <returns></returns>
        //public TimeSeries getSample(int _start, int _length)
        //{
        //    TimeSeries ts = new TimeSeries(_length);

        //    if ((_start + _length) > timeseries.Count)
        //    {
        //        _length = timeseries.Count - 1 - _length;
        //    }

        //    for (int i = 0; i < _length; i++)
        //    {
        //        ts.timeseries[i] = timeseries[_start + i];
        //        ts.timestamps[i] = timestamps[_start + i];
        //        ts.status[i] = status[_start + i];
        //        ts.temperature[i] = temperature[_start + i];
        //        ts.outlierLabel[i] = outlierLabel[_start + i];
        //        ts.cluster[i] = cluster[_start + i];
        //    }
        //    return ts;
        //}

        /// <summary>
        /// формирование выборки данных
        /// </summary>
        /// <param name="_start">Data time start point</param>
        /// <param name="_end">Data time end point</param>
        /// <returns></returns>
        //public TimeSeries getSample(DateTime _start, DateTime _end)
        //{
        //    // do calculation of time;
        //    int startPoint = 0;
        //    int endPoint = 0;
        //    int length = 0;
        //    //не самый быстрый метод, но тем не менее
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        if (timestamps[i] == _start)
        //            startPoint = i;
        //        if (timestamps[i] == _end)
        //        {
        //            endPoint = i;
        //        }
        //    }
        //    length = (endPoint - startPoint) + 1;

        //    TimeSeries ts = new TimeSeries(length);
        //    ts = getSample(startPoint, length);
        //    return ts;
        //}


        //public TimeSeries getSample(int _hour, int _minute, bool _status = true)
        //{
        //    int _length = timeseries.Count / 96;
        //    TimeSeries ts = new TimeSeries(_length);
        //    int j = 0;
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        int cur_hour = Convert.ToInt32(timestamps[i].Hour.ToString());
        //        int cur_minutes = Convert.ToInt32(timestamps[i].Minute.ToString());


        //        if ((cur_hour == _hour) && (cur_minutes == _minute))
        //        {
        //            ts.timeseries[j] = timeseries[i];
        //            ts.timestamps[j] = timestamps[i];
        //            ts.status[j] = status[i];
        //            ts.temperature[j] = temperature[i];
        //            ts.outlierLabel[j] = outlierLabel[i];
        //            ts.cluster[j] = cluster[i];
        //            j++;
        //        }
        //    }
        //    return ts;
        //}


        /// <summary>
        /// Формирование временного ряда с одинковым временем _timestamp
        /// </summary>
        /// <param name="_timestamp"></param>
        /// <param name="_lengthOfShortInterval"></param>
        /// <returns></returns>
        //public TimeSeries getSample(String _timestamp, int _lengthOfShortInterval)
        //{
        //    int _length = timeseries.Count / _lengthOfShortInterval;
        //    TimeSeries ts = new TimeSeries(_length);

        //    //_length

        //    int j = 0;
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        if (timestamps[i].ToString().IndexOf(_timestamp) > 0)
        //        {
        //            ts.timeseries[j] = timeseries[i];
        //            ts.timestamps[j] = timestamps[i];
        //            ts.status[j] = status[i];
        //            ts.temperature[j] = temperature[i];
        //            ts.outlierLabel[j] = outlierLabel[i];
        //            ts.cluster[j] = cluster[i];
        //            j++;

        //            //Console.WriteLine(timestamps[i]);

        //        }
        //    }
        //    return ts;
        //}
        /// <summary>
        /// concatination of the two time series
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //public static TimeSeries operator +(TimeSeries a, TimeSeries b)
        //{
        //    TimeSeries ts = new TimeSeries(a.timeseries.Count + b.timeseries.Count);
        //    int k = 0;
        //    for (int i = 0; i < a.timeseries.Count; i++)
        //    {
        //        ts.timeseries[k] = a.timeseries[i];
        //        ts.timestamps[k] = a.timestamps[i];
        //        ts.status[k] = a.status[i];
        //        ts.temperature[k] = a.temperature[i];
        //        ts.outlierLabel[k] = a.outlierLabel[i];
        //        ts.cluster[k] = a.cluster[i];
        //        k++;

        //    }
        //    for (int i = 0; i < b.timeseries.Count; i++)
        //    {
        //        ts.timeseries[k] = b.timeseries[i];
        //        ts.timestamps[k] = b.timestamps[i];
        //        ts.status[k] = b.status[i];
        //        ts.temperature[k] = b.temperature[i];
        //        ts.outlierLabel[k] = b.outlierLabel[i];
        //        ts.cluster[k] = b.cluster[i];
        //        k++;
        //    }
        //    return ts;

        //}

        //public DateTime getLastDate()
        //{
        //    DateTime ld = timestamps[timestamps.Count - 1];
        //    return ld;

        //}

        //public TimeSpan getFrequency()
        //{
        //    TimeSpan tms;
        //    tms = getLastDate().Subtract(timestamps[timestamps.Count - 2]);
        //    return tms;
        //}

        /// <summary>
        /// return time series with exactly day of week
        /// </summary>
        /// <param name="_dayOfWeek">number of day of week</param>
        /// <returns></returns>
        //public TimeSeries getTimeSeriesDayOfWeek(int _dayOfWeek)
        //{
        //    int numberOfDayOfWeeks = 7;



        //    int _length = 0; //timeseries.Count / numberOfDayOfWeeks;
        //    //считаем длину ряда
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {

        //        if (Convert.ToInt32(timestamps[i].DayOfWeek) == _dayOfWeek)
        //        {
        //            _length++;
        //        }
        //    }
        //    TimeSeries ts = new TimeSeries(_length);

        //    int j = 0;
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {

        //        if (Convert.ToInt32(timestamps[i].DayOfWeek) == _dayOfWeek)
        //        {
        //            ts.timeseries[j] = timeseries[i];
        //            ts.timestamps[j] = timestamps[i];
        //            ts.status[j] = status[i];
        //            ts.temperature[j] = temperature[i];
        //            ts.outlierLabel[j] = outlierLabel[i];
        //            ts.cluster[j] = cluster[i];
        //            j++;
        //            //Console.WriteLine(timestamps[i]);
        //        }
        //    }
        //    return ts;
        //}


        //public int getNumberOfDayOfWeek(int _dayOfWeek)
        //{
        //    int nmb = 0;
        //    for (int i = 0; i < timeseries.Count; i++)
        //    {

        //        if (Convert.ToInt32(timestamps[i].DayOfWeek) == _dayOfWeek)
        //        {
        //            nmb++;
        //            //Console.WriteLine(timestamps[i]);
        //        }
        //    }
        //    return nmb;

        //}

        /// <summary>
        /// Display the time series into Console;        
        /// </summary>
        //public void display(String _caption)
        //{
        //    //double mean = 0.0, median = 0.0 ;

        //    Console.WriteLine("\n" + _caption);
        //    Console.WriteLine("TS with " + timeseries.Count.ToString() + " elements");


        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        //mean += timeseries[i];
        //        Console.WriteLine(timestamps[i] + "; " + timeseries[i]);
        //    }
        //    //Console.WriteLine("min=" + timeseries.Min());
        //    //Console.WriteLine("max=" + timeseries.Max());

        //    //Console.WriteLine("mean=" + getMean());

        //    //Console.WriteLine("median=" + getMedian());

        //}

        //public void info(String _caption)
        //{
        //    Console.WriteLine("====================================");
        //    Console.WriteLine("\n" + _caption);


        //    Console.WriteLine("length=" + timeseries.Count);

        //    Console.WriteLine("start:" + timestamps[0].ToString());
        //    Console.WriteLine("end:" + timestamps[timeseries.Count - 1].ToString());

        //    Console.WriteLine("frq:" + timestamps[timeseries.Count - 1].Subtract(timestamps[timestamps.Count - 2]));


        //    // getLastDate().Subtract(timestamps[timestamps.Count - 2]);
        //    Console.WriteLine("min=" + timeseries.Min());
        //    Console.WriteLine("max=" + timeseries.Max());
        //    Console.WriteLine("mean=" + getMean());
        //    Console.WriteLine("median=" + getMedian());

        //    Console.WriteLine("====================================");
        //}

        //public String info()
        //{
        //    String result = "";
        //    result += "length=" + timeseries.Count + "\r\n";
        //    result += "start:" + timestamps[0].ToString() + "\r\n";
        //    result += "end:" + timestamps[timeseries.Count - 1].ToString() + "\r\n";
        //    result += "frq:" + timestamps[timeseries.Count - 1].Subtract(timestamps[timestamps.Count - 2]) + "\r\n";

        //    result += "min=" + timeseries.Min() + "\r\n";
        //    result += "max=" + timeseries.Max() + "\r\n";

        //    result += "mean=" + getMean().ToString("0.00") + " \r\n";
        //    //result += "median=" + getMedian() + "\r\n";

        //    return result;
        //}

        //public void saveToFile(String _fileName)
        //{
        //    String aHeader = "";

        //    //create a header

        //    aHeader += "TimeStamp; Value \r\n";

        //    File.WriteAllText(_fileName, aHeader);
        //    aHeader = "";

        //    for (int i = 0; i < timeseries.Count; i++)
        //    {
        //        aHeader += timestamps[i].ToString() + ";" + timeseries[i].ToString() + " \r\n";

        //    }
        //    File.AppendAllText(_fileName, aHeader);
        //}


        //public TimeSeries rearrangeTimeStamp(DateTime _dts, DateTime _dtf)
        //{

        //    int length = 0;

        //    //calculate length;




        //    bool terminator = false;
        //    bool curdate_terminator = false;
        //    int j = 0;
        //    long min;// = int.MaxValue;
        //    DateTime curdt = _dts;
        //    int closestDateIndex;



        //    while (!terminator)
        //    {
        //        length++;
        //        curdt = curdt.AddMinutes(15);
        //        if (curdt > _dtf)
        //            terminator = true;
        //    }

        //    TimeSeries ts = new TimeSeries(length);

        //    //ones again
        //    terminator = false;
        //    curdt = _dts;


        //    min = Math.Abs(this.timestamps[0].Ticks - curdt.Ticks);
        //    closestDateIndex = 0;
        //    while (!terminator)
        //    {

        //        Console.WriteLine("perform for " + curdt.ToString());
        //        //найти ближайший                               
        //        for (int i = 0; i < this.timestamps.Count(); i++)
        //        {
        //            if (Math.Abs(this.timestamps[i].Ticks - curdt.Ticks) < min)
        //            {
        //                min = Math.Abs(this.timestamps[i].Ticks - curdt.Ticks);
        //                closestDateIndex = i;
        //            }
        //        }

        //        //заполняем
        //        ts.timeseries[j] = timeseries[closestDateIndex];
        //        ts.timestamps[j] = curdt;//timestamps[closestDateIndex];
        //        ts.status[j] = status[closestDateIndex];
        //        ts.temperature[j] = temperature[closestDateIndex];
        //        ts.outlierLabel[j] = outlierLabel[closestDateIndex];
        //        ts.cluster[j] = cluster[closestDateIndex];


        //        j++;
        //        curdt = curdt.AddMinutes(15);
        //        min = Math.Abs(this.timestamps[0].Ticks - curdt.Ticks);
        //        closestDateIndex = 0;

        //        if (curdt > _dtf)
        //            terminator = true;

        //    }

        //    return ts;
        //}
    }
}
