using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ecosim_dotnetcore
{
    class Logger
    {
        string directory = @"C:\Users\souto\OneDrive\Desktop\Temp\Logs\";
        string configFile = @"\config.json";
        string logFile = @"\logger_data.csv";
        string registerFile = @"\register_data.csv";
        string speciesControlFile = @"\species_data.csv";

        public Logger(int ecosystemID)
        {
            directory += DateTime.Now.ToString("yyyyMMddHHmmss");
            directory += @"\" + ecosystemID.ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            configFile = directory + configFile;
            logFile = directory + logFile;
            registerFile = directory + registerFile;
            speciesControlFile = directory + speciesControlFile;
            System.IO.File.WriteAllText(logFile, "x_time;y_pop;y_food;y_herb;y_meat;y_energ_h_a;y_energ_c_a;y_total_energ;y_energ_flow;y_energ_added;y_metab_a_h;y_vision_a_h;y_rebirth_a_h;y_diet_a_h;y_flock_a_h;y_wobble_a_h;y_life_time_m_h;y_life_time_a_h;y_metab_v_h;y_vision_v_h;y_rebirth_v_h;y_diet_v_h;y_flock_v_h;y_wobble_v_h;y_metab_a_c;y_vision_a_c;y_rebirth_a_c;y_diet_a_c;y_flock_a_c;y_wobble_a_c;y_life_time_m_c;y_life_time_a_c;y_metab_v_c;y_vision_v_c;y_rebirth_v_c;y_diet_v_c;y_flock_v_c;y_wobble_v_c\n");
            System.IO.File.WriteAllText(registerFile, "TIME;EVENT;ACT_ID;ACT_BIRTH;ACT_SPECIES;ACT_DIET;ACT_STATE;ACT_ORIGIN;ACT_X;ACT_Y;TARGET_ID;TARGET_BIRTH;TARGET_SPECIES;TARGET_DIET;TARGET_STATE;TARGET_ORIGIN;TARGET_X;TARGET_Y\n");
            System.IO.File.WriteAllText(speciesControlFile, "TIME;DIET;SPECIES;QTD\n");

        }

        public void LogMetrics(Metrics metrics)
        {
            string[] lines = System.IO.File.ReadAllLines(logFile);
            if (lines.Length < Metrics.BASIC_METRICS_QUANT)
            {
                lines = new string[Metrics.BASIC_METRICS_QUANT];
                for (int i = 0; i < Metrics.BASIC_METRICS_QUANT; i++)
                {
                    lines[i] = metrics.basicMetricsLable[i] + " = []";
                }
            }
            for (int i = 0; i < Metrics.BASIC_METRICS_QUANT; i++)
            {
                lines[i] = lines[i].Remove(lines[i].Length - 1) + metrics.basicMetrics[i].ToString().Replace(',','.') + ", ]";
            }
            using (StreamWriter outputFile = new StreamWriter(logFile))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }
        public void LogMetricsCSV(Metrics metrics)
        {
            StringBuilder register = new StringBuilder();
            string separator = "";
            for (int i = 0; i < Metrics.BASIC_METRICS_QUANT; i++)
            {
                register.Append(separator);
                register.Append(metrics.basicMetrics[i].ToString());
                separator = ";";
            }
            register.AppendLine();
            AppendLog(register, logFile);
        }
        public void LogRegisterBook(StringBuilder registers)
        {
            AppendLog(registers, registerFile);
        }
        public void LogSpecies(StringBuilder registers)
        {
            AppendLog(registers, speciesControlFile);
        }
        public void AppendLog(StringBuilder registers, string fileName)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName, true))
            {
                if (registers.ToString() != string.Empty)
                    outputFile.Write(registers.ToString());
            }
        }
        public void LogConfiguration(Congiguration config)
        {
            StringBuilder register = new StringBuilder();
            string separator = "";
            string line;
            register.AppendLine("{");
            register.Append("   \"" + config.ToString() + "\": {");
            foreach (KeyValuePair<string, dynamic> kv in config.parameters)
            {
                register.AppendLine(separator);
                line = "      \"" + kv.Key + "\": " + kv.Value;
                register.Append(line.Replace(",", "."));
                separator = ",";
            }
            register.AppendLine();
            register.AppendLine("   }");
            register.AppendLine("}");
            System.IO.File.WriteAllText(configFile, register.ToString());
        }
    }
}
