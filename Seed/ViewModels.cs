using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seed.ViewModels
{
    class MainVM
    {
        public Configure conf { get; set; }
        public MainVM()
        {
            conf = new Configure();
            conf.MeasureStatus = true;
        }
    }
}
