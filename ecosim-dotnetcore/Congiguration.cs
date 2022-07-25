using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class Congiguration
    {
        public Dictionary<string, dynamic> parameters;

        public Congiguration(double x)
        {
            parameters = new Dictionary<string, dynamic>();

            /* Engine config: */
            parameters.Add("ENGINE_FPS", 60);
            parameters.Add("ENGINE_WINDOW_X", 1600);
            parameters.Add("ENGINE_WINDOW_Y", 900);
            /* End engine config */

            /* Main World config */
            parameters.Add("WORLD_MIN_COORD", -1.0);
            parameters.Add("WORLD_MAX_COORD", 1.0);          
            /* End world config */

            /* Instance config */
            parameters.Add("INSTANCE_INIT_AGENT_COUNT",90);
      
            parameters.Add("INSTANCE_FOOD_SPAWN_FREQ",4);
            parameters.Add("INSTANCE_FOOD_SPAWN_INIT",50);
            parameters.Add("INSTANCE_FOOD_SPAWN_MIN",5);
            parameters.Add("INSTANCE_FOOD_SPAWN_MAX",7);
            parameters.Add("INSTANCE_FOOD_ENERGY",0.5);
            parameters.Add("INSTANCE_FOOD_METAB", 0.0001);

            parameters.Add("INSTANCE_PREDATOR_PREY_STRENGTH_RATIO_MIN", 0.1);
            parameters.Add("INSTANCE_PREDATOR_PREY_STRENGTH_RATIO_MAX", 1.0);

            parameters.Add("INSTANCE_REPRODUCTION_TIME_INTERVAL", 4.0);


            parameters.Add("INSTANCE_CONTROL_MUTATE_START",6000.0);
            parameters.Add("INSTANCE_CONTROL_MUTATE_STOP",12000.0);

            parameters.Add("INSTANCE_FOREIGNER_START", 18000.0);
            parameters.Add("INSTANCE_FOREIGNER_STOP", 24000.0);
            parameters.Add("INSTANCE_FOREIGNER_SPAWN_MIN",0);
            parameters.Add("INSTANCE_FOREIGNER_SPAWN_MAX",3);
            parameters.Add("INSTANCE_FOREIGNER_SPAWN_PROBABILITY",0.01);
            /* Instance config */

            /* Agent general config */
            /* Agent transparency */
            //parameters.Add("AGENT_RGB_ALPHA",0.9);
            /* Agent vision field transparency */
            //parameters.Add("AGENT_VIS_ALPHA",0.2);
            /* Maximum agent velocity */
            parameters.Add("AGENT_MAX_VELOCITY",1.0);
            parameters.Add("AGENT_MIN_VELOCITY",-1.0);
            /* Default energy for new-spawned agents */
            parameters.Add("AGENT_ENERGY_DEFAULT",1.0);
            parameters.Add("AGENT_ENERGY_SEXUAL_REPRODUCTION", 0.25);
            parameters.Add("AGENT_ENERGY_ASEXUAL_REPRODUCTION", 0.5);
            /* The maximum speed any agents can move at */
            parameters.Add("AGENT_MAX_SPEED",0.0015);
            parameters.Add("AGENT_MATE_REACH_SPACE", 0.08);
            parameters.Add("AGENT_EAT_REACH_SPACE", 0.02);
            /* The energy level at which an agent dies */
            parameters.Add("AGENT_ENERGY_DEAD",0.3);
            /* How quickly ageing effects the agents */
            parameters.Add("AGENT_TIME_FACTOR",0.3);

            parameters.Add("AGENT_DIET_BOUNDARY",0.5);
            parameters.Add("AGENT_SEXUED_BOUNDARY",0.0);
            parameters.Add("AGENT_DNA_SPECIES_RANGE", 0.5);

            /* The amount a DNA trait changes if mutation occurs */
            parameters.Add("AGENT_DNA_MUTATE_PROBABILITY", 0.01);
            parameters.Add("AGENT_DNA_MAX_MUTATED_GENS", 2);
            parameters.Add("AGENT_DNA_MUTATE_RATE", 0.1);

            /* End agent general config */

            /* Agent DNA config */
            /* Metabolism trait max/min 
             * How quickly an agent can move. Faster moving agents burn energy a lot
             * quicker */
            parameters.Add("AGENT_METAB_MAX",0.8);
            parameters.Add("AGENT_METAB_MIN",0.1);
            /* Vision trait max/min 
             * How wide the agents field of view is */
            parameters.Add("AGENT_VISION_MAX",0.2);
            parameters.Add("AGENT_VISION_MIN",0.1);
            /* Rebirth trait max/min 
             * How much energy is stored within an agent until it splits, creating a 
             * possibly mutated clone of itself, halving it's energy */
            parameters.Add("AGENT_REBIRTH_MAX",3.00);
            parameters.Add("AGENT_REBIRTH_MIN",1.00);
            /* Diet trait max/min
             * If greater or equal to zero, the agent eats other agents, if less than 
             * zero, the agent eats only dead agents */
            parameters.Add("AGENT_DIET_MIN",0.00);
            parameters.Add("AGENT_DIET_MAX",1.00);
            /* Diet trait max/min
             * If greater or equal to zero, the agent eats other agents, if less than 
             * zero, the agent eats only dead agents */
            parameters.Add("AGENT_SEXUED_MIN",0.00);
            parameters.Add("AGENT_SEXUED_MAX",1.00);
            /* Flock max/min
             * How strong flocking behaviours influence the movement of an agent */
            parameters.Add("AGENT_FLOCK_MAX",0.50);
            parameters.Add("AGENT_FLOCK_MIN",0.00);
            /* Wobble max/min
             * How many times per second the agent "wobbles" This is a movement based on
             * a sin wav, which gives a temporary boost of speed, followed by a equal
             * period of slower movement */
            parameters.Add("AGENT_WOBBLE_MAX",3.0);
            parameters.Add("AGENT_WOBBLE_MIN",1.0);
            /* End agent DNA config */

            /*Log config */
            parameters.Add("LOGGER_ENABLE",1);
            parameters.Add("LOGGER_FREQ",2);
            /*End log config */

        }

    }
}
