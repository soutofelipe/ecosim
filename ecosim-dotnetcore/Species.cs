using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Species
    {
        public int id;
        public int ecosystemId;
        public int individuals;
        public Agent.AgentDNA referenceDNA;
        Agent.AgentDNA upBoundarieDNA;
        Agent.AgentDNA downBoundarieDNA;
        public Species(Agent.AgentDNA agentDNA, int paramecossystemId)
        {
            ecosystemId = paramecossystemId;
            referenceDNA = agentDNA;
            upBoundarieDNA = agentDNA;
            upBoundarieDNA.flock += (GetConfiguration("AGENT_FLOCK_MAX") - GetConfiguration( "AGENT_FLOCK_MIN")) * (GetConfiguration("AGENT_DNA_SPECIES_RANGE") / 2);
            upBoundarieDNA.metabolism += (GetConfiguration( "AGENT_METAB_MAX") - GetConfiguration( "AGENT_METAB_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            upBoundarieDNA.rebirth += (GetConfiguration( "AGENT_REBIRTH_MAX") - GetConfiguration( "AGENT_REBIRTH_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            upBoundarieDNA.vision += (GetConfiguration( "AGENT_VISION_MAX") - GetConfiguration( "AGENT_VISION_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            upBoundarieDNA.wobble += (GetConfiguration( "AGENT_WOBBLE_MAX") - GetConfiguration( "AGENT_WOBBLE_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            downBoundarieDNA = agentDNA;
            downBoundarieDNA.flock -= (GetConfiguration( "AGENT_FLOCK_MAX") - GetConfiguration( "AGENT_FLOCK_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            downBoundarieDNA.metabolism -= (GetConfiguration( "AGENT_METAB_MAX") - GetConfiguration( "AGENT_METAB_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            downBoundarieDNA.rebirth -= (GetConfiguration( "AGENT_REBIRTH_MAX") - GetConfiguration( "AGENT_REBIRTH_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            downBoundarieDNA.vision -= (GetConfiguration( "AGENT_VISION_MAX") - GetConfiguration( "AGENT_VISION_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            downBoundarieDNA.wobble -= (GetConfiguration( "AGENT_WOBBLE_MAX") - GetConfiguration( "AGENT_WOBBLE_MIN")) * (GetConfiguration( "AGENT_DNA_SPECIES_RANGE") / 2);
            id = Cache.ecossytemData[ecosystemId].speciestNextId;
            individuals = 1;
        }
        Agent GetAgent(int agentID)
        {
            return Cache.ecossytemData[ecosystemId].agents[agentID];
        }

        dynamic GetConfiguration(string configurationParam)
        {
            return Cache.ecossytemData[ecosystemId].congiguration.parameters[configurationParam];
        }
        public bool SameSpecies(Agent.AgentDNA agentDNA)
        {
            bool same = true;
            if (agentDNA.diet != referenceDNA.diet)
                same = false;
            else if (agentDNA.flock < downBoundarieDNA.flock || agentDNA.flock > upBoundarieDNA.flock)
                same = false;
            else if (agentDNA.metabolism < downBoundarieDNA.metabolism || agentDNA.metabolism > upBoundarieDNA.metabolism)
                same = false;
            else if (agentDNA.rebirth < downBoundarieDNA.rebirth || agentDNA.rebirth > upBoundarieDNA.rebirth)
                same = false;
            else if (agentDNA.vision < downBoundarieDNA.vision || agentDNA.vision > upBoundarieDNA.vision)
                same = false;
            else if (agentDNA.wobble < downBoundarieDNA.wobble || agentDNA.wobble > upBoundarieDNA.wobble)
                same = false;

            return same;
        }
        public void IncludeIndividual()
        {
            individuals++;
        }
        public void RemoveIndividual()
        {
            individuals--;
        }
    }
}
