using System;
using System.Reflection;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TFA.Config;
using TFA.Players;
using TShockAPI;
using TShockAPI.Hooks;

namespace TFA
{
	[ApiVersion(2, 1)]
	public class MainClass : TerrariaPlugin
	{
		public MainClass(Main game) : base(game)
		{
			base.Order = 0;
		}

		public override string Author => "Terrarian"; 
		public override string Description => "Двух-этапная аутентификация, помогает избавиться от взлома по UUID.";
		public override string Name => "TwoFactorAuth"; 
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version; 

		public override void Initialize()
		{
			MainClass.Cfg = Config.Read();
			PlayerHooks.PlayerPostLogin += MainClass.OnPlayerPostLogin;
			Commands.ChatCommands.Add(new Command("14.twofactor", new CommandDelegate(MainClass.TwoFactorSettings), "tfas"));
			Commands.ChatCommands.Add(new Command("14.twofactor", new CommandDelegate(MainClass.TwoFactorLogin), "tfal"));
			new Timer(1000.0)
			{
				Enabled = true,
				AutoReset = true
			}.Elapsed += new ElapsedEventHandler(MainClass.OnTimer);
		}

		public static void TwoFactorSettings(CommandArgs e)
		{
			if (!e.Player.IsLoggedIn || !e.Player.GetPlayerInfo().Loggined)
			{
				e.Player.SendErrorMessage("You are not logged in.");
				return;
			}
			if (e.Parameters.Count < 1)
			{
				e.Player.SendErrorMessage("Error. Command syntax: /tfas <code>");
				return;
			}
			if (MainClass.Cfg.Words.ContainsKey(e.Player.Account.Name))
			{
				MainClass.Cfg.Words.Remove(e.Player.Account.Name);
			}
			MainClass.Cfg.Words.Add(e.Player.Account.Name, e.Parameters[0]);
			MainClass.Cfg.Write();
			e.Player.SendSuccessMessage("The password was successfully changed.");
		}

		public static void TwoFactorLogin(CommandArgs e)
		{
			if (!e.Player.IsLoggedIn)
			{
				e.Player.SendErrorMessage("You are not logged in.");
				return;
			}
			if (e.Player.GetPlayerInfo().Loggined)
			{
				e.Player.SendErrorMessage("You are already logged in.");
				return;
			}
			if (e.Parameters.Count < 1)
			{
				e.Player.SendErrorMessage("Error. Command syntax: /tfal <code>");
				return;
			}
			if (e.Parameters[0] == MainClass.Cfg.Words[e.Player.Account.Name])
			{
				e.Player.GetPlayerInfo().Loggined = true;
				e.Player.SendSuccessMessage("The account was unlocked!");
			}
		}

		public static void OnTimer(object null1, object null2)
		{
			TSPlayer[] players = TShock.Players;
			for (int i = 0; i < players.Length; i++)
			{
				TSPlayer plr = players[i];
				if (plr.GetPlayerInfo().Loggined == MainClass.off)
				{
					plr.tempGroup = Group.DefaultGroup;
					plr.tempGroupTimer = new Timer(5000.0)
					{
						AutoReset = false,
						Enabled = true
					};
					plr.tempGroupTimer.Elapsed += delegate(object x, ElapsedEventArgs y)
					{
						MainClass.TempGroupTimerElapsed(x, y, plr);
					};
				}
			}
		}

		public static void TempGroupTimerElapsed(object sender, ElapsedEventArgs args, TSPlayer plr)
		{
			plr.tempGroup = null;
			if (sender != null)
			{
				((Timer)sender).Stop();
			}
		}

		public static void OnPlayerPostLogin(PlayerPostLoginEventArgs e)
		{
			if (MainClass.Cfg.Words.ContainsKey(e.Player.Account.Name))
			{
				e.Player.GetPlayerInfo().Loggined = false;
				e.Player.SendErrorMessage("Your account is blocked!");
				e.Player.SendErrorMessage("Enter /tfal <code> to unblock your account.");
				Console.WriteLine("Waiting " + e.Player.Account.Name + " to login...");
				return;
			}
			e.Player.GetPlayerInfo().Loggined = true;
		}

		public static Config Cfg;
		public static bool off;
	}
}
