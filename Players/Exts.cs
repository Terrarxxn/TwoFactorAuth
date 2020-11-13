using System;
using TShockAPI;

namespace TFA.Players
{
	public static class Exts
	{
		public static TFAInfo GetPlayerInfo(this TSPlayer tsplayer)
		{
			if (!tsplayer.ContainsData("TFA_Data"))
			{
				tsplayer.SetData<TFAInfo>("TFA_Data", new TFAInfo());
			}
			return tsplayer.GetData<TFAInfo>("TFA_Data");
		}
	}
}
