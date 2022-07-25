using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class SimulatorTime
    {
        //public double deltaTime { get; set; }
        public double totalElapsedSeconds { get; set; }
        public double lastUpdateTime { get; set; }
        public SimulatorTime()
        {
            //deltaTime = 0.0;
            totalElapsedSeconds = 0.0;
            lastUpdateTime = 0.0;
        }
    }
}
