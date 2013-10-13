using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smarterdam.Web.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<SelectListItem> Measurements;

        public List<string> CurrentTasks = new List<string>();
    }
}