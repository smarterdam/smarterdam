using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarterdam.Web.ViewModels
{
    public class StatusViewModel
    {
        public string BuildingId { get; set; }
        public List<ChartViewModel> Charts { get; set; } 
   
        public StatusViewModel()
        {
            Charts = new List<ChartViewModel>();
        }
    }
}