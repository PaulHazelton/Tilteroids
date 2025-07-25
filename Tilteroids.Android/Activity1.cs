using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Tilteroids.Android
{
	[Activity(
		Label = "@string/app_name",
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/DarkTheme",
		AlwaysRetainTaskState = true,
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.Landscape,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
	)]
	public class Activity1 : AndroidGameActivity
	{
		private Tilteroids.Core.Framework.TilteroidsManager _game;
		private View _view;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			_game = new Tilteroids.Core.Framework.TilteroidsManager();
			_view = _game.Services.GetService(typeof(View)) as View;

			SetContentView(_view);
			_game.Run();
		}
	}
}
