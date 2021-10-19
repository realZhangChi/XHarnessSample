using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

#if __ANDROID__

            builder.Services.AddTransient(svc => new HeadlessTestRunner("test-result-path"));

#endif

            return builder.Build();
        }
    }
}