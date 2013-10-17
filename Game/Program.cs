// Copyright (C) 2006-2013 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Engine;
using Engine.MathEx;
using Engine.FileSystem;
using Engine.Renderer;
using Engine.Utils;
using GameCommon;
using GameEntities;

namespace AOT
{
	/// <summary>
	/// Defines an input point in the application.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			try
			{
				Main2();
			}
			catch( Exception e )
			{
				Log.FatalAsException( e.ToString() );
            }		
		}

		static void Main2()
		{
            if (!VirtualFileSystem.Init("user:Logs/Game.log", true, null, Directory.GetCurrentDirectory() + "\\..\\Data",
                Directory.GetCurrentDirectory() + "\\..\\UserSettings"))
				return;

			EngineApp.ConfigName = "user:Configs/Game.config";

			if( PlatformInfo.Platform == PlatformInfo.Platforms.Windows )
				EngineApp.UseDirectInputForMouseRelativeMode = true;
			EngineApp.AllowJoysticksAndCustomInputDevices = true;
			EngineApp.AllowWriteEngineConfigFile = true;
			EngineApp.AllowChangeVideoMode = true;

			// enable vsync. should no have verticalSync option in the Engine.config
			//RendererWorld.InitializationOptions.VerticalSync = true;

			//Change Floating Point Model for FPU math calculations. Default is Strict53Bits.
			//FloatingPointModel.Model = FloatingPointModel.Models.Strict53Bits;

			EngineApp.Init( new GameEngineApp() );
			EngineApp.Instance.WindowTitle = "AOT";

			if( PlatformInfo.Platform == PlatformInfo.Platforms.Windows )
				EngineApp.Instance.Icon = AOT.Properties.Resources.Logo;

			EngineConsole.Init();

			EngineApp.Instance.Config.RegisterClassParameters( typeof( GameEngineApp ) );

			//EngineApp.Instance.SuspendWorkingWhenApplicationIsNotActive = false;

			if( EngineApp.Instance.Create() )
				EngineApp.Instance.Run();

			EngineApp.Shutdown();

			Log.DumpToFile( "Program END\r\n" );

			VirtualFileSystem.Shutdown();
		}
	}
}
