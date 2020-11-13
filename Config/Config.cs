using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace TFA.Config
{
	public class Config
	{
		public static Config Read()
		{
			Config result;
			if (!File.Exists(TShock.SavePath + "\\TFA.json"))
			{
				Config config = new Config();
				File.WriteAllText(TShock.SavePath + "\\TFA.json", JsonConvert.SerializeObject(config, 1));
				result = config;
			}
			else
			{
				result = JsonConvert.DeserializeObject<Config>(File.ReadAllText(TShock.SavePath + "\\TFA.json"));
			}
			return result;
		}

		public void Write()
		{
			File.WriteAllText(TShock.SavePath + "\\TFA.json", JsonConvert.SerializeObject(this, 1));
		}
		
		public Dictionary<string, string> Words = new Dictionary<string, string>();
	}
}
