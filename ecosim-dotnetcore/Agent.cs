using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Agent
    {
        public enum AgentDiet
        {
            NONE = -1,
            MEAT = 0,
            PLANT = 1,
            SELF = 2
        }
        public enum AgentReprpoduction
        {
            ASSEXUED = 0,
            SEXUED = 1
        }
        public enum AgentStates
        {
            PRUNE = -1,
            DEAD = 0,
            LIVING = 1
        }

        public enum AgentAttraction
        {
            NONE = -1,
            FLOCK = 0,
            SEEK = 1,
            AVOID = 2
        }

        public struct AgentDNA
        {
            public AgentDiet diet;
            public AgentReprpoduction reprpoduction;
            public double metabolism;
            public double vision;
            public double rebirth;
            public double flock;
            public double wobble;
        }
        public struct AgentColor
        {
            public double r;
            public double g;
            public double b;
        }

        public struct VisionQuad
        {
            public SpaceCoordinates upBoyndary;
            public SpaceCoordinates lowBoyndary;
        }

        public AgentDNA DNA;
        public AgentStates state;
        public double energy;
        public AgentColor color;
        public SpaceCoordinates position;
        public SpaceCoordinates velocity;
        public VisionQuad visionQuad;

        public int id;
        public string origin;
        public bool betaAgente;

        public int interactions;
        public List<int>[] nearAgents;

        public int analised;
        public int inVision;

        public double birthDate;
        public int idSpecies;
        public double lastParenting;
        //public double deathDate;

        int ecosystemId;

        public Agent(int paramEcossytemID, AgentStates stateParam, string originParam, double originEnergy, int parentID = -1, AgentDNA paranDNA = new AgentDNA(), bool beta = false, AgentDiet betaType = AgentDiet.NONE)
        {
            ecosystemId = paramEcossytemID;
            betaAgente = beta;
            id = NextAgentId(true);
            birthDate = Cache.ecossytemData[ecosystemId].lastUpdateTime;
            lastParenting = Cache.ecossytemData[ecosystemId].lastUpdateTime;
            state = stateParam;
            origin = originParam;
            idSpecies = -1;

            nearAgents = new List<int>[3];
            interactions = 0;

            if (stateParam == AgentStates.LIVING)
            {
                energy = GetConfiguration("AGENT_ENERGY_DEFAULT");
                if (parentID != -1)
                {
                    SpaceCoordinates positionIncremet = new SpaceCoordinates(0.03);
                    DNA = paranDNA;
                    Mutate(GetConfiguration("AGENT_DNA_MUTATE_PROBABILITY"));
                    if (Cache.RANDF(1) > 0.1)
                    {
                        idSpecies = GetAgent(parentID).idSpecies;
                        Cache.ecossytemData[ecosystemId].species[idSpecies].IncludeIndividual();
                    }
                    else
                    {
                        SetupSpecies();
                    }
                    position = new SpaceCoordinates(GetAgent(parentID).position);
                    if (Cache.RANDF(1) < 0.5)
                        positionIncremet.x *= -1;
                    if (Cache.RANDF(1) < 0.5)
                        positionIncremet.y *= -1;
                    position.AddCoordinates(positionIncremet);
                    velocity = new SpaceCoordinates(GetAgent(parentID).velocity);
                    SetupVisionQuad();
                    Cache.ecossytemData[ecosystemId].metrics.RegisterEvent(Cache.ecossytemData[ecosystemId].lastUpdateTime, GetAgent(parentID), this, originParam);
                }
                else
                {
                    DNA = RadomDNA();
                    if (beta)
                        DNA.diet = betaType;
                    position = new SpaceCoordinates(Cache.RANDF_MIN(GetConfiguration("WORLD_MIN_COORD"), GetConfiguration("WORLD_MAX_COORD")), Cache.RANDF_MIN(GetConfiguration("WORLD_MIN_COORD"), GetConfiguration("WORLD_MAX_COORD")));
                    velocity = new SpaceCoordinates(Cache.RANDF_MIN(GetConfiguration("AGENT_MIN_VELOCITY"), GetConfiguration("AGENT_MAX_VELOCITY")), Cache.RANDF_MIN(GetConfiguration("AGENT_MIN_VELOCITY"), GetConfiguration("AGENT_MAX_VELOCITY")));
                    SetupVisionQuad();
                    SetupSpecies();
                    Cache.ecossytemData[ecosystemId].metrics.RegisterEvent(Cache.ecossytemData[ecosystemId].lastUpdateTime, this, this, originParam);
                }
                for (int i = 0; i < nearAgents.Length; i++)
                    nearAgents[i] = new List<int>();
            }
            else
            {
                //state = AgentStates.DEAD;
                energy = GetConfiguration("INSTANCE_FOOD_ENERGY");
                DNA = new AgentDNA();
                DNA.diet = AgentDiet.SELF;
                DNA.metabolism = GetConfiguration("INSTANCE_FOOD_METAB");
                position = new SpaceCoordinates(Cache.RANDF_MIN(GetConfiguration("WORLD_MIN_COORD"), GetConfiguration("WORLD_MAX_COORD")), Cache.RANDF_MIN(GetConfiguration("WORLD_MIN_COORD"), GetConfiguration("WORLD_MAX_COORD")));
                velocity = new SpaceCoordinates();
            }
            Cache.ecossytemData[ecosystemId].metrics.AddEnergyAdded(energy - originEnergy);
            SetupColors();
        }
        Agent GetAgent(int agentID)
        {
            return Cache.ecossytemData[ecosystemId].agents[agentID];
        }
        int NextAgentId()
        {
            return Cache.ecossytemData[ecosystemId].agentNextId;
        }
        int NextAgentId(bool add)
        {
            int returnID = Cache.ecossytemData[ecosystemId].agentNextId;
            if (add)
                Cache.ecossytemData[ecosystemId].agentNextId++;
            return returnID;
        }
        dynamic GetConfiguration(string configurationParam)
        {
            return Cache.ecossytemData[ecosystemId].congiguration.parameters[configurationParam];
        }
        double GetClock()
        {
            return Cache.ecossytemData[ecosystemId].clock;
        }
        AgentDNA RadomDNA()
        {
            AgentDNA tempDNA = new AgentDNA();
            double tempRand;
            tempDNA.metabolism = Cache.RANDF_MIN(GetConfiguration("AGENT_METAB_MIN"), GetConfiguration("AGENT_METAB_MAX"));
            tempDNA.vision = Cache.RANDF_MIN(GetConfiguration("AGENT_VISION_MIN"), GetConfiguration("AGENT_VISION_MAX"));
            tempDNA.rebirth = Cache.RANDF_MIN(GetConfiguration("AGENT_REBIRTH_MIN"), GetConfiguration("AGENT_REBIRTH_MAX"));
            tempDNA.flock = Cache.RANDF_MIN(GetConfiguration("AGENT_FLOCK_MIN"), GetConfiguration("AGENT_FLOCK_MAX"));
            tempDNA.wobble = Cache.RANDF_MIN(GetConfiguration("AGENT_WOBBLE_MIN"), GetConfiguration("AGENT_WOBBLE_MAX"));
            tempRand = Cache.RANDF_MIN(GetConfiguration("AGENT_DIET_MIN"), GetConfiguration("AGENT_DIET_MAX"));
            if (tempRand < GetConfiguration("AGENT_DIET_BOUNDARY"))
                tempDNA.diet = AgentDiet.MEAT;
            else
                tempDNA.diet = AgentDiet.PLANT;

            tempRand = Cache.RANDF_MIN(GetConfiguration("AGENT_SEXUED_MIN"), GetConfiguration("AGENT_SEXUED_MAX"));
            if (tempRand < GetConfiguration("AGENT_SEXUED_BOUNDARY"))
                tempDNA.reprpoduction = AgentReprpoduction.ASSEXUED;
            else
                tempDNA.reprpoduction = AgentReprpoduction.SEXUED;
            return tempDNA;
        }
        public void SetupSpecies()
        {
            foreach (KeyValuePair<int, Species> specieWithKey in Cache.ecossytemData[ecosystemId].species)
            {
                if (idSpecies == -1 && specieWithKey.Value.SameSpecies(DNA))
                {
                    idSpecies = specieWithKey.Key;
                    Cache.ecossytemData[ecosystemId].species[idSpecies].IncludeIndividual();
                }
            }
            if (idSpecies == -1)
            {
                Cache.ecossytemData[ecosystemId].species.Add(Cache.ecossytemData[ecosystemId].speciestNextId, new Species(DNA, ecosystemId));
                idSpecies = Cache.ecossytemData[ecosystemId].speciestNextId;
                Cache.ecossytemData[ecosystemId].speciestNextId++;
            }
        }
        public void SetupColors()
        {
            if (DNA.diet == AgentDiet.SELF)
            {
                color.r = 0.2;
                color.g = 0.2;
                color.b = 0.2;
            }
            else if (origin == "FOREIGNER")
            {
                if (DNA.diet == AgentDiet.MEAT)
                {
                    color.r = 1;
                    color.g = 0.7;
                }
                else if (DNA.diet == AgentDiet.PLANT)
                {
                    color.r = 0.7;
                    color.g = 1;
                }
                color.b = 0.7;
            }
            else
            {
                if (DNA.diet == AgentDiet.MEAT)
                {
                    color.r = 1;
                    color.g = 0;
                }
                else if (DNA.diet == AgentDiet.PLANT)
                {
                    color.r = 0;
                    color.g = 1;
                }
                if (betaAgente)
                    color.b = 1;
                else
                    color.b = idSpecies / (double)Cache.ecossytemData[ecosystemId].species.Count;
            }
        }
        void UpdateColors()
        {
            color.b = idSpecies / (double)Cache.ecossytemData[ecosystemId].species.Count;
        }
        void SetupVisionQuad()
        {
            double halfRad = DNA.vision * 0.5;
            visionQuad.upBoyndary = new SpaceCoordinates(position);
            visionQuad.upBoyndary.AddCoordinates(halfRad);
            visionQuad.lowBoyndary = new SpaceCoordinates(position);
            visionQuad.lowBoyndary.SubtractCoordinates(halfRad);
        }
        public AgentAttraction GetAttraction(AgentDiet targetDiet, int targetIdSpecies)
        {
            if (targetDiet == AgentDiet.NONE) return AgentAttraction.NONE;

            /* meat eater vs other meat eater */
            else if (DNA.diet == AgentDiet.MEAT && targetDiet == AgentDiet.MEAT && idSpecies == targetIdSpecies)
                return AgentAttraction.FLOCK;

            /* meat eater vs any living */
            else if (DNA.diet == AgentDiet.MEAT && targetDiet == AgentDiet.PLANT)
                return AgentAttraction.SEEK;

            /* meat eater vs dead */
            else if (DNA.diet == AgentDiet.MEAT && targetDiet == AgentDiet.SELF)
                return AgentAttraction.NONE;

            /* plant eater vs dead */
            else if (DNA.diet == AgentDiet.PLANT && targetDiet == AgentDiet.SELF)
                return AgentAttraction.SEEK;

            /* plant eater vs platn eater */
            else if (DNA.diet == AgentDiet.PLANT && targetDiet == AgentDiet.PLANT && idSpecies == targetIdSpecies)
                return AgentAttraction.FLOCK;

            /* plant eater vs meat eater*/
            else if (DNA.diet == AgentDiet.PLANT && targetDiet == AgentDiet.MEAT)
                return AgentAttraction.AVOID;

            /* plant eater vs living */
            else if (DNA.diet == AgentDiet.PLANT)
                return AgentAttraction.NONE;

            return AgentAttraction.NONE;
        }
        public Agent Update()
        {
            Agent childAgents = null;
            FindNearAgents();
            MoveFlock();
            MoveSeekOrAvoid();
            Collision();
            UpdateLocation(GetClock());
            UpdateEnergy();
            UpdateColors();
            if (!IsAlive())
            {
                Die();
            }
            if (CanReproduct())
            {
                if (DNA.reprpoduction == Agent.AgentReprpoduction.SEXUED)
                {
                    childAgents = Mate();
                }
                else
                {
                    childAgents = Reproduct();
                }
            }
            return childAgents;
        }
        public void FindNearAgents()
        {
            Agent.AgentAttraction attraction;
            inVision = 0;
            analised = 0;
            for (int i = 0; i < nearAgents.Length; i++)
                nearAgents[i] = new List<int>();
            foreach (KeyValuePair<int, Agent> currentAgent in Cache.ecossytemData[ecosystemId].agents)
            {
                if (id != currentAgent.Key)
                    analised++;
                if (CanSee(currentAgent.Value))
                {
                    inVision++;
                    attraction = GetAttraction(currentAgent.Value.DNA.diet, currentAgent.Value.idSpecies);
                    if (attraction != Agent.AgentAttraction.NONE)
                        nearAgents[(int)attraction].Add(currentAgent.Key);
                }

            }
        }
        public void MoveFlock()
        {
            SpaceCoordinates finalVelocity = new SpaceCoordinates();
            SpaceCoordinates alignVelocity = FlockAlign();
            SpaceCoordinates cohesionVelocity = FlockCohesion();
            SpaceCoordinates seperationVelocity = FlockSeperation();
            /* Velocity alignment */
            finalVelocity.AddCoordinates(alignVelocity);
            /* Cohesion: Go to center of mass */
            finalVelocity.AddCoordinates(cohesionVelocity);
            /* Seperation: Avoid otherss */
            finalVelocity.AddCoordinates(seperationVelocity);

            finalVelocity.MultiplyCoordinates(DNA.flock);

            velocity.AddCoordinates(finalVelocity);

            velocity.Normalize();
        }
        public SpaceCoordinates FlockAlign()
        {
            SpaceCoordinates returnVelocity;
            int nearAgentsCount = nearAgents[(int)Agent.AgentAttraction.FLOCK].Count;
            returnVelocity = new SpaceCoordinates();

            if (nearAgentsCount == 0) return returnVelocity;

            for (int i = 0; i < nearAgentsCount; i++)
            {
                returnVelocity.AddCoordinates(GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).velocity);
            }

            returnVelocity.DevideCoordinates(nearAgentsCount);
            returnVelocity.Normalize();

            return returnVelocity;
        }
        public SpaceCoordinates FlockCohesion()
        {
            SpaceCoordinates returnVelocity;
            int nearAgentsCount = nearAgents[(int)Agent.AgentAttraction.FLOCK].Count;
            returnVelocity = new SpaceCoordinates();

            if (nearAgentsCount == 0) return returnVelocity;

            for (int i = 0; i < nearAgentsCount; i++)
            {
                returnVelocity.AddCoordinates(GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position);
            }

            returnVelocity.DevideCoordinates(nearAgentsCount);
            returnVelocity.SubtractCoordinates(position);
            returnVelocity.Normalize();

            return returnVelocity;
        }
        public SpaceCoordinates FlockSeperation()
        {
            SpaceCoordinates returnVelocity;
            int nearAgentsCount = nearAgents[(int)Agent.AgentAttraction.FLOCK].Count;
            returnVelocity = new SpaceCoordinates();

            if (nearAgentsCount == 0) return returnVelocity;

            for (int i = 0; i < nearAgentsCount; i++)
            {
                returnVelocity.AddCoordinates(GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position);
                returnVelocity.SubtractCoordinates(position);
            }

            returnVelocity.DevideCoordinates(-1 * nearAgentsCount);
            returnVelocity.Normalize();

            return returnVelocity;
        }
        public void MoveSeekOrAvoid()
        {
            SpaceCoordinates newVelocity = new SpaceCoordinates(position);
            for (int i = 0; i < nearAgents[(int)Agent.AgentAttraction.SEEK].Count; i++)
            {
                newVelocity.SubtractCoordinates(GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).position);
                newVelocity.Normalize();
                velocity.SubtractCoordinates(newVelocity);
            }
            newVelocity = new SpaceCoordinates(position);
            for (int i = 0; i < nearAgents[(int)Agent.AgentAttraction.AVOID].Count; i++)
            {
                newVelocity.SubtractCoordinates(GetAgent(nearAgents[(int)Agent.AgentAttraction.AVOID][i]).position);
                newVelocity.Normalize();
                velocity.AddCoordinates(newVelocity);
            }
            velocity.Normalize();
        }
        public void Collision()
        {
            double reachSpace = GetConfiguration("AGENT_EAT_REACH_SPACE");
            double minMetab = GetConfiguration("AGENT_METAB_MIN");
            double maxMetab = GetConfiguration("AGENT_METAB_MAX");
            reachSpace += reachSpace * (maxMetab - DNA.metabolism) /(maxMetab - minMetab);
            double minPressure = GetConfiguration("INSTANCE_PREDATOR_PREY_STRENGTH_RATIO_MIN");
            double maxPressure = GetConfiguration("INSTANCE_PREDATOR_PREY_STRENGTH_RATIO_MAX");
            for (int i = 0; i < nearAgents[(int)Agent.AgentAttraction.SEEK].Count; i++)
            {
                if (!GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).betaAgente)
                {
                    if (((position.x - reachSpace < GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).position.x) &
                    (position.x + reachSpace > GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).position.x)) &&
                    ((position.y - reachSpace < GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).position.y) &
                    (position.y + reachSpace > GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).position.y)))
                    {
                        if(Cache.RANDF_MIN(minPressure, maxPressure) < ((energy / 3) + DNA.metabolism ) / ((GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).energy / 3) + GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).DNA.metabolism)) { 
                            energy += GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).energy;
                            Cache.ecossytemData[ecosystemId].metrics.AddEnergyFlow(GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).energy);
                            interactions++;
                            GetAgent(nearAgents[(int)Agent.AgentAttraction.SEEK][i]).Prune(id);
                            Cache.ecossytemData[ecosystemId].pruneList.Add(nearAgents[(int)Agent.AgentAttraction.SEEK][i]);
                        }
                    }
                }
            }
        }
        public void UpdateLocation(double clock)
        {
            double maxSpeed = AgentMaxSpeed();
            double wobbleFactor = 1.0;
            if(DNA.diet == AgentDiet.PLANT)
                wobbleFactor = (2.0 + Math.Sin(DNA.wobble + DNA.wobble * clock)) * 0.5;
            else
                wobbleFactor = (2.0 + Math.Sin(DNA.wobble + DNA.wobble * clock) + 0.5* Math.Sin(43*(DNA.wobble + DNA.wobble * clock))) * 0.5;
            SpaceCoordinates wobble = new SpaceCoordinates(velocity);
            wobble.MultiplyCoordinates(maxSpeed * Math.Abs(wobbleFactor));
            position.AddCoordinates(wobble);
            WarpLocation();
            SetupVisionQuad();
        }
        public void UpdateEnergy()
        {
            energy -= AGENT_METAB_ENERGY_SCALE(DNA.metabolism) * GetConfiguration("AGENT_TIME_FACTOR");
        }
        public bool IsAlive()
        {
            return betaAgente || (energy >= GetConfiguration("AGENT_ENERGY_DEAD"));
        }
        public bool HasEnergy()
        {
            return betaAgente || (energy >= 0);
        }
        public double AgentMaxSpeed()
        {
            return DNA.metabolism * GetConfiguration("AGENT_MAX_SPEED");
        }
        public void WarpLocation()
        {
            if (position.x < GetConfiguration("WORLD_MIN_COORD"))
                position.x = GetConfiguration("WORLD_MAX_COORD");
            else if (position.x > GetConfiguration("WORLD_MAX_COORD"))
                position.x = GetConfiguration("WORLD_MIN_COORD");

            if (position.y < GetConfiguration("WORLD_MIN_COORD"))
                position.y = GetConfiguration("WORLD_MAX_COORD");
            else if (position.y > GetConfiguration("WORLD_MAX_COORD"))
                position.y = GetConfiguration("WORLD_MIN_COORD");
        }
        public void Die()
        {
            Cache.ecossytemData[ecosystemId].metrics.RegisterEvent(Cache.ecossytemData[ecosystemId].lastUpdateTime, this, this, "DIE");
            state = AgentStates.DEAD;
            DNA.diet = AgentDiet.SELF;
            DNA.metabolism = GetConfiguration("INSTANCE_FOOD_METAB");
            velocity.Set(0.0);
            color.r = 0.2;
            color.g = 0.2;
            color.b = 0.2;
            Cache.ecossytemData[ecosystemId].species[idSpecies].RemoveIndividual();
        }
        public void Prune(int eaterId)
        {
            Cache.ecossytemData[ecosystemId].metrics.RegisterEvent(Cache.ecossytemData[ecosystemId].lastUpdateTime, GetAgent(eaterId), this, "EAT");
            if (state == AgentStates.LIVING)
                Cache.ecossytemData[ecosystemId].species[idSpecies].RemoveIndividual();
            state = AgentStates.PRUNE;
            DNA.diet = AgentDiet.NONE;
        }
        public bool CanReproduct()
        {
            return ((energy > DNA.rebirth) && (Cache.ecossytemData[ecosystemId].lastUpdateTime > lastParenting + GetConfiguration("INSTANCE_REPRODUCTION_TIME_INTERVAL"))) ;
        }
        public Agent Reproduct(int agentID = 0)
        {
            double transferedEnergy;
            interactions++;
            if(DNA.reprpoduction == AgentReprpoduction.SEXUED)
             {
                transferedEnergy = energy * GetConfiguration("AGENT_ENERGY_SEXUAL_REPRODUCTION");
                energy *= (1 -  GetConfiguration("AGENT_ENERGY_SEXUAL_REPRODUCTION"));
                lastParenting = Cache.ecossytemData[ecosystemId].lastUpdateTime;
                transferedEnergy += GetAgent(agentID).energy * GetConfiguration("AGENT_ENERGY_SEXUAL_REPRODUCTION");
                GetAgent(agentID).energy *=  (1 -  GetConfiguration("AGENT_ENERGY_SEXUAL_REPRODUCTION"));
                GetAgent(agentID).lastParenting = Cache.ecossytemData[ecosystemId].lastUpdateTime;
                Cache.ecossytemData[ecosystemId].metrics.AddEnergyFlow(transferedEnergy);
                return new Agent(ecosystemId, AgentStates.LIVING, "REPRODUCT", transferedEnergy, id, MixDNA(GetAgent(agentID).DNA));
            }
            else
            {
                transferedEnergy = energy * GetConfiguration("AGENT_ENERGY_ASEXUAL_REPRODUCTION");
                energy *= (1 - GetConfiguration("AGENT_ENERGY_ASEXUAL_REPRODUCTION"));
                lastParenting = Cache.ecossytemData[ecosystemId].lastUpdateTime;
                Cache.ecossytemData[ecosystemId].metrics.AddEnergyFlow(transferedEnergy);
                return new Agent(ecosystemId, AgentStates.LIVING, "SPLIT", transferedEnergy, id, DNA);
            }
        }
        public Agent Mate()
        {
            Agent childAgents = null;
            double reachSpace = GetConfiguration("AGENT_MATE_REACH_SPACE");
            for (int i = 0; i < nearAgents[(int)Agent.AgentAttraction.FLOCK].Count; i++)
            {
                if (GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).CanReproduct() &&
                    (((position.x - reachSpace < GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position.x) &
                    (position.x + reachSpace > GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position.x)) &&
                    ((position.y - reachSpace < GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position.y) &
                    (position.y + reachSpace > GetAgent(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]).position.y))))
                {
                    childAgents = Reproduct(nearAgents[(int)Agent.AgentAttraction.FLOCK][i]);
                    continue;
                }
            }
            bool varStop = false;
            if (childAgents == null)
                varStop = true;
            return childAgents;
        }
        public AgentDNA MixDNA(AgentDNA otherDNA)
        {
            AgentDNA tempDNA = new AgentDNA();
            tempDNA = new AgentDNA();
            tempDNA.diet = DNA.diet;
            tempDNA.reprpoduction = DNA.reprpoduction;
            if (Cache.RANDF(1) > 0.5)
                tempDNA.metabolism = DNA.metabolism;
            else
                tempDNA.metabolism = otherDNA.metabolism;

            if (Cache.RANDF(1) > 0.5)
                tempDNA.vision = DNA.vision;
            else
                tempDNA.vision = otherDNA.vision;

            if (Cache.RANDF(1) > 0.5)
                tempDNA.rebirth = DNA.rebirth;
            else
                tempDNA.rebirth = otherDNA.rebirth;

            if (Cache.RANDF(1) > 0.5)
                tempDNA.flock = DNA.flock;
            else
                tempDNA.flock = otherDNA.flock;

            if (Cache.RANDF(1) > 0.5)
                tempDNA.wobble = DNA.wobble;
            else
                tempDNA.wobble = otherDNA.wobble;
            return tempDNA;
        }
        public void Mutate(double probability)
        {
            if(birthDate > GetConfiguration("INSTANCE_CONTROL_MUTATE_START") && birthDate < GetConfiguration("INSTANCE_CONTROL_MUTATE_STOP"))
            { 
                int mutatedGens = 0;
                if (Cache.RANDF(1) < probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                {
                    DNA.metabolism = Cache.RANDF_MIN(GetConfiguration("AGENT_METAB_MIN"), GetConfiguration("AGENT_METAB_MAX"));
                    mutatedGens++;
                }
                if (Cache.RANDF(1) < probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                {
                    DNA.vision = Cache.RANDF_MIN(GetConfiguration("AGENT_VISION_MIN"), GetConfiguration("AGENT_VISION_MAX"));
                    mutatedGens++;
                }
                if (Cache.RANDF(1) < probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                {
                    DNA.rebirth = Cache.RANDF_MIN(GetConfiguration("AGENT_REBIRTH_MIN"), GetConfiguration("AGENT_REBIRTH_MAX"));
                    mutatedGens++;
                }
                if (Cache.RANDF(1) < probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                {
                    DNA.flock = Cache.RANDF_MIN(GetConfiguration("AGENT_FLOCK_MIN"), GetConfiguration("AGENT_FLOCK_MAX"));
                    mutatedGens++;
                }
                if (Cache.RANDF(1) < probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                {
                    DNA.wobble = Cache.RANDF_MIN(GetConfiguration("AGENT_WOBBLE_MIN"), GetConfiguration("AGENT_WOBBLE_MAX"));
                    mutatedGens++;
                }
                //if (Cache.RANDF(1) <= probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                //{
                //    if (Cache.RANDF_MIN(GetConfiguration("AGENT_DIET_MIN"), GetConfiguration("AGENT_DIET_MAX")) < GetConfiguration("AGENT_DIET_BOUNDARY"))
                //        DNA.diet = AgentDiet.MEAT;
                //    else
                //        DNA.diet = AgentDiet.PLANT;
                //    mutatedGens++;
                //}
                //if (Cache.RANDF(1) <= probability && mutatedGens <= GetConfiguration("AGENT_DNA_MAX_MUTATED_GENS"))
                //{
                //    if (Cache.RANDF_MIN(GetConfiguration("AGENT_SEXUED_MIN"), GetConfiguration("AGENT_SEXUED_MAX")) < GetConfiguration("AGENT_SEXUED_BOUNDARY"))
                //        DNA.reprpoduction = AgentReprpoduction.ASSEXUED;
                //    else
                //        DNA.reprpoduction = AgentReprpoduction.SEXUED;
                //    mutatedGens++;
                //}
            }
        }
        public void RangeMutate(double probability)
        {
            double temporaryGen;
            temporaryGen = DNA.metabolism + (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(-1.0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0);
            if (temporaryGen > GetConfiguration("AGENT_METAB_MAX"))
                DNA.metabolism = GetConfiguration("AGENT_METAB_MAX");
            else if (temporaryGen < GetConfiguration("AGENT_METAB_MIN"))
                DNA.metabolism = GetConfiguration("AGENT_METAB_MIN");
            else
                DNA.metabolism = temporaryGen;

            temporaryGen = DNA.vision + (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(-1.0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0);
            if (temporaryGen > GetConfiguration("AGENT_VISION_MAX"))
                DNA.vision = GetConfiguration("AGENT_VISION_MAX");
            else if (temporaryGen < GetConfiguration("AGENT_VISION_MIN"))
                DNA.vision = GetConfiguration("AGENT_VISION_MIN");
            else
                DNA.vision = temporaryGen;

            temporaryGen = DNA.rebirth + (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(-1.0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0);
            if (temporaryGen > GetConfiguration("AGENT_REBIRTH_MAX"))
                DNA.rebirth = GetConfiguration("AGENT_REBIRTH_MAX");
            else if (temporaryGen < GetConfiguration("AGENT_REBIRTH_MIN"))
                DNA.rebirth = GetConfiguration("AGENT_REBIRTH_MIN");
            else
                DNA.rebirth = temporaryGen;

            temporaryGen = DNA.flock + (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(-1.0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0);
            if (temporaryGen > GetConfiguration("AGENT_FLOCK_MAX"))
                DNA.flock = GetConfiguration("AGENT_FLOCK_MAX");
            else if (temporaryGen < GetConfiguration("AGENT_FLOCK_MIN"))
                DNA.flock = GetConfiguration("AGENT_FLOCK_MIN");
            else
                DNA.flock = temporaryGen;

            temporaryGen = DNA.wobble + (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(-1.0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0);
            if (temporaryGen > GetConfiguration("AGENT_WOBBLE_MAX"))
                DNA.wobble = GetConfiguration("AGENT_WOBBLE_MAX");
            else if (temporaryGen < GetConfiguration("AGENT_WOBBLE_MIN"))
                DNA.wobble = GetConfiguration("AGENT_WOBBLE_MIN");
            else
                DNA.wobble = temporaryGen;

            temporaryGen = Math.Abs((double)DNA.diet - (Cache.RANDF(1) > probability ? Cache.RANDF_MIN(0, 1.0) * GetConfiguration("AGENT_DNA_MUTATE_RATE") : 0));
            if (temporaryGen < GetConfiguration("AGENT_DIET_BOUNDARY"))
                DNA.diet = AgentDiet.MEAT;
            else
                DNA.diet = AgentDiet.PLANT;

        }
        public bool CanSee(Agent targetAgent)
        {
            return ((visionQuad.upBoyndary.x >= targetAgent.position.x) &
                (visionQuad.upBoyndary.y >= targetAgent.position.y)) &&
                ((visionQuad.lowBoyndary.x <= targetAgent.position.x) &
                (visionQuad.lowBoyndary.y <= targetAgent.position.y));
        }
        public List<float> Vertex()
        {
            List<float>  agentVertex = new List<float>();
            agentVertex.Add((float)position.x);
            agentVertex.Add((float)position.y);
            agentVertex.Add((float)energy);
            agentVertex.Add(0.5f);
            agentVertex.Add((float)color.r);
            agentVertex.Add((float)color.g);
            agentVertex.Add((float)color.b);
            agentVertex.Add(energy < 1.0 ? (float)energy : 1.0f);
            return agentVertex;
        }
        /* The rate energy is burned over time, with respect to metabolism rates = (where x is agents metabolism */
        public double AGENT_METAB_ENERGY_SCALE(double x) { return (0.001 * x); }
        /* How large agents are, with respect to their energy = (Where x is energy); */
        public double AGENT_ENERGY_SIZE_SCALE(double x) { return ((10 * x) + 4); }

    }
}
