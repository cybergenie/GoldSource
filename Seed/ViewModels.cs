using System;
using System.Collections.Generic;
using System.Text;

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
