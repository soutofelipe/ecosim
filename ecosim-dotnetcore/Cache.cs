using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class EcossystemData
    {
        public Congiguration congiguration;

        public Dictionary<int, Agent> agents;
        public int agentNextId;

        public Dictionary<int, Species> species;
        public int speciestNextId;

        public List<int> pruneList;

        public Logger logger;
        public Metrics metrics;

        public double lastUpdateTime;
        public double lastFoodTime;
        public double lastLogTime;
        public double clock;
    }
    static class Cache
    {
        public static dynamic GetConfiguration(string configurationParam)
        {
            return Cache.ecossytemData[0].congiguration.parameters[configurationParam];
        }
       
        public static Dictionary<int, EcossystemData> ecossytemData;

        public static Random rand = new Random();
        public static double RANDF(double x) { return ((double)rand.NextDouble() * x); }
        public static double RANDF_MIN(double min, double max) { return (((double)rand.NextDouble() * (max - min)) + min); }
        public static double MIN(double a, double b) { return (((a) < (b)) ? (a) : (b)); }
        public static double MAX(double a, double b) { return (((a) > (b)) ? (a) : (b)); ; }

    }
}
