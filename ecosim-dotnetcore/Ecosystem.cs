using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Ecosystem
    {
        int ecosystemId; 
        public Ecosystem(int paramId, Dictionary<string,dynamic> paramsToChange)
        {
            ecosystemId = paramId;
            Cache.ecossytemData[ecosystemId].agentNextId = 0;
            Cache.ecossytemData[ecosystemId].agents = new Dictionary<int, Agent>();
            Cache.ecossytemData[ecosystemId].speciestNextId = 0;
            Cache.ecossytemData[ecosystemId].species = new Dictionary<int, Species>();

            Cache.ecossytemData[ecosystemId].clock = 0.0;
            Cache.ecossytemData[ecosystemId].congiguration = new Congiguration((double)ecosystemId);

            ChangeConfiguration(paramsToChange);

            Cache.ecossytemData[ecosystemId].lastLogTime = 0.0;
            Cache.ecossytemData[ecosystemId].metrics = new Metrics(ecosystemId);
            Cache.ecossytemData[ecosystemId].logger = new Logger(ecosystemId);

            Cache.ecossytemData[ecosystemId].logger.LogConfiguration(Cache.ecossytemData[ecosystemId].congiguration);

            Cache.ecossytemData[ecosystemId].lastUpdateTime = 0.0;
            IncludeAgents(GetConfiguration("INSTANCE_INIT_AGENT_COUNT"), Agent.AgentStates.LIVING, "START_LIVING", GetConfiguration("AGENT_ENERGY_DEFAULT"));
            Cache.ecossytemData[ecosystemId].lastFoodTime = 0.0;
            IncludeAgents(GetConfiguration("INSTANCE_FOOD_SPAWN_INIT"), Agent.AgentStates.DEAD, "START_DEAD", GetConfiguration("INSTANCE_FOOD_ENERGY"));
            IncludeBetaAgents();
        }
        dynamic GetConfiguration(string configurationParam)
        {
            return Cache.ecossytemData[ecosystemId].congiguration.parameters[configurationParam];
        }
        void ChangeConfiguration(Dictionary<string, dynamic> paramsToChange)
        {
            int intParam = 0;
            double doubleParam = 0.0;
            foreach (KeyValuePair<string, dynamic> currentParam in paramsToChange)
            {
                if(int.TryParse(currentParam.Value, out intParam))
                    Cache.ecossytemData[ecosystemId].congiguration.parameters[currentParam.Key] = intParam;
                else if (double.TryParse(currentParam.Value, out doubleParam))
                    Cache.ecossytemData[ecosystemId].congiguration.parameters[currentParam.Key] = doubleParam;
            }
        }
        void IncludeBetaAgents()
        {
            //agents.Add(agentNextId, new Agent(Agent.AgentStates.AGENT_STATE_LIVING, "BETA", agentNextId, true, Agent.AgentDiet.AGENT_DIET_LIVING));
            //agentNextId++;
            //agents.Add(agentNextId, new Agent(Agent.AgentStates.AGENT_STATE_LIVING, "BETA", agentNextId, true, Agent.AgentDiet.AGENT_DIET_DEAD));
            //agentNextId++;
            //agents.Add(agentNextId, new Agent(Agent.AgentStates.AGENT_STATE_DEAD, "BETA", agentNextId, true, Agent.AgentDiet.AGENT_DIET_SELF));
            //agentNextId++;
        }
        void IncludeAgents(int amount, Agent.AgentStates state, string origin, double originEnergy)
        {
            Agent temporaryAgent;
            for (int i = 0; i < amount; i++)
            {
                temporaryAgent = new Agent(ecosystemId, state, origin, originEnergy);
                Cache.ecossytemData[ecosystemId].agents.Add(temporaryAgent.id, temporaryAgent);
            }
        }
        public void RunInteration(double time)
        {
            Console.WriteLine(time);
            Cache.ecossytemData[ecosystemId].metrics.Clear();
            Cache.ecossytemData[ecosystemId].pruneList = new List<int>();
            Cache.ecossytemData[ecosystemId].lastUpdateTime = time;
            UpdateAgents();
            FoodDrop();
            InsertForeigner();
            Cache.ecossytemData[ecosystemId].metrics.GenerateLog(time);
        }
        public void UpdateAgents()
        {
            Cache.ecossytemData[ecosystemId].clock += 0.05;
            Agent temporaryAgent;
            List<Agent> childAgents = new List<Agent>();
            foreach (KeyValuePair<int, Agent> currentAgent in Cache.ecossytemData[ecosystemId].agents)
            {
                if (currentAgent.Value.state == Agent.AgentStates.LIVING)
                {
                    temporaryAgent = currentAgent.Value.Update();
                    if (temporaryAgent != null)
                        childAgents.Add(temporaryAgent);
                }
                else
                {
                    currentAgent.Value.UpdateEnergy();
                    if (!currentAgent.Value.HasEnergy())
                    {
                        currentAgent.Value.Prune(currentAgent.Key);
                        Cache.ecossytemData[ecosystemId].pruneList.Add(currentAgent.Key);
                    }
                }
            }
            for (int i = 0; i < childAgents.Count; i++)
                Cache.ecossytemData[ecosystemId].agents.Add(childAgents[i].id, childAgents[i]);
            for (int i = 0; i < Cache.ecossytemData[ecosystemId].pruneList.Count; i++)
                Cache.ecossytemData[ecosystemId].agents.Remove(Cache.ecossytemData[ecosystemId].pruneList[i]);
        }
        public void FoodDrop()
        {
            int foodAmount;
            if (Cache.ecossytemData[ecosystemId].lastUpdateTime > Cache.ecossytemData[ecosystemId].lastFoodTime + GetConfiguration("INSTANCE_FOOD_SPAWN_FREQ"))
            {
                foodAmount = (int)Cache.RANDF_MIN(GetConfiguration( "INSTANCE_FOOD_SPAWN_MIN"), GetConfiguration("INSTANCE_FOOD_SPAWN_MAX"));
                IncludeAgents(foodAmount, Agent.AgentStates.DEAD, "FOOD_DROP", 0.0);
                Cache.ecossytemData[ecosystemId].lastFoodTime = Cache.ecossytemData[ecosystemId].lastUpdateTime;
            }
        }
        public void InsertForeigner()
        {
            int foreignerAmount;
            
            if (Cache.ecossytemData[ecosystemId].lastUpdateTime > GetConfiguration("INSTANCE_FOREIGNER_START") && Cache.ecossytemData[ecosystemId].lastUpdateTime < GetConfiguration("INSTANCE_FOREIGNER_STOP") && GetConfiguration("INSTANCE_FOREIGNER_SPAWN_PROBABILITY") > Cache.RANDF(1))
            {
                foreignerAmount = (int)Cache.RANDF_MIN(GetConfiguration( "INSTANCE_FOREIGNER_SPAWN_MIN"), GetConfiguration( "INSTANCE_FOREIGNER_SPAWN_MAX"));
                IncludeAgents(foreignerAmount, Agent.AgentStates.LIVING, "FOREIGNER", 0.0);
            }
        }
    }
}
