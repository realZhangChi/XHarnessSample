using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;

#if __ANDROID__

using XHarnessSample.Platforms.Android;

#endif

namespace XHarnessSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(life =>
             {
#if __ANDROID__
					life.AddAndroid(android =>
					{
						android.OnCreate((a, b) => Microsoft.Maui.Essentials.Platform.Init(a, b));
					});
#endif
             });
#if __ANDROID__

            builder.Services.AddTransient(svc => new HeadlessTestRunner("testresults.xml"));

#endif

            return builder.Build();
        }
    }
}