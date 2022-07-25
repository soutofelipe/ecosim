using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Metrics
    {
        enum BasicMetricsOrder
        {
            //GENERAL
            LOG_TIME,
            LOG_POPULATION,
            LOG_FOOD,
            LOG_HERBIVOUR,
            LOG_CARNIVOUR,
            LOG_HERB_ENERGY_AVG,
            LOG_CARN_ENERGY_AVG,
            LOG_TOTAL_ENERGY,
            LOG_ENERGY_FLOW,
            LOG_ENERGY_ADDED,
            //HERB
            LOG_METABOLISM_AVG_H,
            LOG_VISION_AVG_H,
            LOG_REBIRTH_AVG_H,
            LOG_DIET_AVG_H,
            LOG_FLOCK_AVG_H,
            LOG_WOBBLE_AVG_H,
            LOG_LIFE_TIME_MAX_H,
            LOG_LIFE_TIME_AVG_H,

            LOG_METABOLISM_VAR_H,
            LOG_VISION_VAR_H,
            LOG_REBIRTH_VAR_H,
            LOG_DIET_VAR_H,
            LOG_FLOCK_VAR_H,
            LOG_WOBBLE_VAR_H,
            
            //CARNI
            LOG_METABOLISM_AVG_C,
            LOG_VISION_AVG_C,
            LOG_REBIRTH_AVG_C,
            LOG_DIET_AVG_C,
            LOG_FLOCK_AVG_C,
            LOG_WOBBLE_AVG_C,
            LOG_LIFE_TIME_MAX_C,
            LOG_LIFE_TIME_AVG_C,

            LOG_METABOLISM_VAR_C,
            LOG_VISION_VAR_C,
            LOG_REBIRTH_VAR_C,
            LOG_DIET_VAR_C,
            LOG_FLOCK_VAR_C,
            LOG_WOBBLE_VAR_C

        };

        enum BasicMetricsAnalyticsOrder
        {
            LOG_METABOLISM,
            LOG_VISION,
            LOG_REBIRTH,
            LOG_DIET,
            LOG_FLOCK,
            LOG_WOBBLE
        };

        public int ecosystemId;

        public const int BASIC_METRICS_QUANT = 38;
        public const int GENETIC_METRICS_QUANT = 8;
        public const int ANALYTICS_METRICS_QUANT = 6;
        public const int HERB_METRICS_START = 10;
        public const int CARN_METRICS_START = 24;

        public double[] basicMetrics;
        public string[] basicMetricsLable;

        public List<double>[] herbMetricsAnalytics;
        public List<double>[] carnMetricsAnalytics;
        public StringBuilder registerBook;
        public StringBuilder speciesLog;

        public double energyFlow;
        public double energyAdded;

        List<float> agentVertex;
        public Metrics(int paramEcosystemId)
        {
            ecosystemId = paramEcosystemId;
            energyFlow = 0.0;
            energyAdded = 0.0;
            basicMetrics = new double[BASIC_METRICS_QUANT];
            herbMetricsAnalytics = new List<double>[ANALYTICS_METRICS_QUANT];
            carnMetricsAnalytics = new List<double>[ANALYTICS_METRICS_QUANT];
            basicMetricsLable = new string[] { "x_time", "y_pop", "y_food", "y_herb", "y_meat", "y_energ_h_a", "y_energ_c_a", "y_total_energ", "y_energ_flow", "y_energ_added", "y_metab_a_h", "y_vision_a_h", "y_rebirth_a_h", "y_diet_a_h", "y_flock_a_h", "y_wobble_a_h", "y_life_time_m_h", "y_life_time_a_h", "y_metab_v_h", "y_vision_v_h", "y_rebirth_v_h", "y_diet_v_h", "y_flock_v_h", "y_wobble_v_h", "y_metab_a_c", "y_vision_a_c", "y_rebirth_a_c", "y_diet_a_c", "y_flock_a_c", "y_wobble_a_c", "y_life_time_m_c", "y_life_time_a_c", "y_metab_v_c", "y_vision_v_c", "y_rebirth_v_c", "y_diet_v_c", "y_flock_v_c", "y_wobble_v_c" };
            Clear();
        }
        dynamic GetConfiguration(string configurationParam)
        {
            return Cache.ecossytemData[ecosystemId].congiguration.parameters[configurationParam];
        }
        public void Clear()
        {
            agentVertex = new List<float>();
            for (int i = 0; i < basicMetrics.Length; i++)
                basicMetrics[i] = 0.0;
            for (int i = 0; i < ANALYTICS_METRICS_QUANT; i++)
            {
                herbMetricsAnalytics[i] = new List<double>();
                carnMetricsAnalytics[i] = new List<double>();
            }
            registerBook = new StringBuilder();
            speciesLog = new StringBuilder();
        }
        public float[] GetVertex()
        {
            return agentVertex.ToArray();
        }
        public void GenerateLog(double time)
        {
            bool log = (Cache.ecossytemData[ecosystemId].lastLogTime + GetConfiguration("LOGGER_FREQ")) < time;
            foreach (KeyValuePair<int, Agent> currentAgent in Cache.ecossytemData[ecosystemId].agents)
            {
                agentVertex.AddRange(currentAgent.Value.Vertex());
                if (log)
                    AddToMetrics(currentAgent.Value, time);
            }
            if (log)
            {
                Summarize(time);
                //Cache.ecossytemData[ecosystemId].logger.LogMetrics(Cache.ecossytemData[ecosystemId].metrics);
                Cache.ecossytemData[ecosystemId].logger.LogMetricsCSV(Cache.ecossytemData[ecosystemId].metrics);
                //LogSpecies(Cache.ecossytemData[ecosystemId].species, time);
                Cache.ecossytemData[ecosystemId].logger.LogSpecies(Cache.ecossytemData[ecosystemId].metrics.speciesLog);
                Cache.ecossytemData[ecosystemId].lastLogTime = time;
            }
            //Cache.ecossytemData[ecosystemId].logger.LogRegisterBook(Cache.ecossytemData[ecosystemId].metrics.registerBook);

        }
        public void AddEnergyFlow(double energy)
        {
            energyFlow += energy;
        }
        public void AddEnergyAdded(double energy)
        {
            energyAdded += energy;
        }
        public void AddToMetrics(Agent agent, double time)
        {
            if (agent.state == Agent.AgentStates.LIVING)
            {
                basicMetrics[(int)BasicMetricsOrder.LOG_POPULATION] += 1;
                basicMetrics[(int)BasicMetricsOrder.LOG_TOTAL_ENERGY] += agent.energy;          

                if (agent.DNA.diet == Agent.AgentDiet.MEAT)
                {
                    basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR] += 1;
                    basicMetrics[(int)BasicMetricsOrder.LOG_CARN_ENERGY_AVG] += agent.energy;
                    basicMetrics[(int)BasicMetricsOrder.LOG_METABOLISM_AVG_C] += agent.DNA.metabolism * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_VISION_AVG_C] += agent.DNA.vision * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_REBIRTH_AVG_C] += agent.DNA.rebirth * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_DIET_AVG_C] += (double)agent.DNA.diet * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_FLOCK_AVG_C] += agent.DNA.flock * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_WOBBLE_AVG_C] += agent.DNA.wobble * 100;
                    if (basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_MAX_C] < (time - agent.birthDate))
                        basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_MAX_C] = (time - agent.birthDate);
                    basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_AVG_C] += (time - agent.birthDate);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_METABOLISM].Add(agent.DNA.metabolism * 100);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_VISION].Add(agent.DNA.vision * 100);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_REBIRTH].Add(agent.DNA.rebirth * 100);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_DIET].Add((double)agent.DNA.diet * 100);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_FLOCK].Add(agent.DNA.flock * 100);
                    carnMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_WOBBLE].Add(agent.DNA.wobble * 100);
                }
                else
                {
                    basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR] += 1;
                    basicMetrics[(int)BasicMetricsOrder.LOG_HERB_ENERGY_AVG] += agent.energy;
                    basicMetrics[(int)BasicMetricsOrder.LOG_METABOLISM_AVG_H] += agent.DNA.metabolism * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_VISION_AVG_H] += agent.DNA.vision * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_REBIRTH_AVG_H] += agent.DNA.rebirth * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_DIET_AVG_H] += (double)agent.DNA.diet * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_FLOCK_AVG_H] += agent.DNA.flock * 100;
                    basicMetrics[(int)BasicMetricsOrder.LOG_WOBBLE_AVG_H] += agent.DNA.wobble * 100;
                    if (basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_MAX_H] < (time - agent.birthDate))
                        basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_MAX_H] = (time - agent.birthDate);
                    basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_AVG_H] += (time - agent.birthDate);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_METABOLISM].Add(agent.DNA.metabolism * 100);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_VISION].Add(agent.DNA.vision * 100);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_REBIRTH].Add(agent.DNA.rebirth * 100);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_DIET].Add((double)agent.DNA.diet * 100);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_FLOCK].Add(agent.DNA.flock * 100);
                    herbMetricsAnalytics[(int)BasicMetricsAnalyticsOrder.LOG_WOBBLE].Add(agent.DNA.wobble * 100);
                }
            }
            else if (agent.state == Agent.AgentStates.DEAD)
            {
                basicMetrics[(int)BasicMetricsOrder.LOG_FOOD] += 1;
            }
        }
        public void Summarize(double time)
        {
            basicMetrics[(int)BasicMetricsOrder.LOG_TIME] = time;
            basicMetrics[(int)BasicMetricsOrder.LOG_METABOLISM_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_VISION_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_REBIRTH_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_DIET_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_FLOCK_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_WOBBLE_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_AVG_H] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];

            basicMetrics[(int)BasicMetricsOrder.LOG_METABOLISM_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_VISION_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_REBIRTH_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_DIET_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_FLOCK_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_WOBBLE_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_LIFE_TIME_AVG_C] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];

            //basicMetrics[(int)BasicMetricsOrder.LOG_HERB_ENERGY_AVG] /= basicMetrics[(int)BasicMetricsOrder.LOG_HERBIVOUR];
            //basicMetrics[(int)BasicMetricsOrder.LOG_CARN_ENERGY_AVG] /= basicMetrics[(int)BasicMetricsOrder.LOG_CARNIVOUR];
            basicMetrics[(int)BasicMetricsOrder.LOG_ENERGY_FLOW] = energyFlow;
            basicMetrics[(int)BasicMetricsOrder.LOG_ENERGY_ADDED] = energyAdded;

            for (int i = 0; i < ANALYTICS_METRICS_QUANT; i++)
            {
                for(int j = 0; j < herbMetricsAnalytics[i].Count; j++)
                    basicMetrics[i + HERB_METRICS_START + GENETIC_METRICS_QUANT] += Math.Pow(herbMetricsAnalytics[i][j] - basicMetrics[i + HERB_METRICS_START], 2);
                basicMetrics[i + HERB_METRICS_START + GENETIC_METRICS_QUANT] /= herbMetricsAnalytics[i].Count;

                for (int j = 0; j < carnMetricsAnalytics[i].Count; j++)
                    basicMetrics[i + CARN_METRICS_START + GENETIC_METRICS_QUANT] += Math.Pow(carnMetricsAnalytics[i][j] - basicMetrics[i + CARN_METRICS_START], 2);
                basicMetrics[i + CARN_METRICS_START + GENETIC_METRICS_QUANT] /= carnMetricsAnalytics[i].Count;
            }
            for (int i = 0; i < basicMetrics.Length; i++)
            {
                if (double.IsNaN(basicMetrics[i]))
                    basicMetrics[i] = 0;
                else
                    basicMetrics[i] = Math.Round(basicMetrics[i]);
            }
            energyFlow = 0.0;
            energyAdded = 0.0;
        }
        public void RegisterEvent(double time, Agent actAgent, Agent targetAgent, string eventType)
        {
            //registerBook.Append(time);
            //registerBook.Append(";");
            //registerBook.Append(eventType);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.id);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.birthDate);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.idSpecies);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.DNA.diet);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.state);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.origin);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.position.x);
            //registerBook.Append(";");
            //registerBook.Append(actAgent.position.y);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.id);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.birthDate);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.idSpecies);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.DNA.diet);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.state);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.origin);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.position.x);
            //registerBook.Append(";");
            //registerBook.Append(targetAgent.position.y);
            //registerBook.AppendLine();
        }
        public void LogSpecies(Dictionary<int, Species> species, double time)
        {
            foreach (KeyValuePair<int, Species> specieWithKey in species)
            {
                speciesLog.Append(time);
                speciesLog.Append(";");
                speciesLog.Append(specieWithKey.Value.referenceDNA.diet);
                speciesLog.Append(";");
                speciesLog.Append(specieWithKey.Key);
                speciesLog.Append(";");
                speciesLog.Append(specieWithKey.Value.individuals);
                speciesLog.AppendLine();
            }
        }
    }
}
