using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lab17_server
{
    abstract public class Worker
    {
        public bool completed = false;
        public int TickMS = 1000;
        virtual public void Run()
        {
            while (!completed)
            {
                Update();
                Thread.Sleep(TickMS);
            }
        }
        abstract public void Update();
        
        public void Stop()
        {
            completed = true;
        }
    }
}
