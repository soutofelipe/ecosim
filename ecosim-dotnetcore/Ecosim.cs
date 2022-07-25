using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Ecosim
    {
        static Simulator simulator;
        static void Main(string[] args)
        {
            bool graphics = true;
            int simulations = 1;
            int time = 1200;
            Dictionary<string,dynamic> paramsToChange = new Dictionary<string, dynamic>();
            string[] paramSplit;
            if (args.Length >= 2)
            {
                simulations = int.Parse(args[0]);
                time = int.Parse(args[1]);
                for(int i=2;i<args.Length;i++)
                {
                    paramSplit = args[i].Split('=');
                    paramsToChange.Add(paramSplit[0], paramSplit[1]);
                }

            }
            simulator = new Simulator();
            for (int i = 1; i <= simulations; i++)
            {
                simulator.RunSimulation(graphics, i, time, paramsToChange);
                graphics = false;
            }
        }
    }
}
