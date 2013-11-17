using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarterdam.Web.ViewModels
{
    public class ChartViewModel
    {
        public string Id { get; set; }

        public string ChartData { get; set; }

        public double? Error { get; set; }

        public string ChartName { get; set; }

        public ChartViewModel()
        {
            this.Id = "";
            this.ChartData = "";
            this.ChartName = "";
        }
    }
}