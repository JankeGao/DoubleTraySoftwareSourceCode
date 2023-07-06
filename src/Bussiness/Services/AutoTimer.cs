using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Bussiness.Services
{
    public class AutoTimer
    {
        private System.Timers.Timer timer;
        private InServer inServer;
        private OutServer outServer;

        public AutoTimer()
        {
            timer = new System.Timers.Timer
            {
                Interval = 10000 // 每10秒触发一次
            };
            timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            timer.Start();
            inServer = new InServer();
            outServer = new OutServer();
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            inServer.CreateInEntityInterFace();
            outServer.CreateOutEntityInterFace();
        }
    }

}
