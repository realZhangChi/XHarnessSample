using Android.App;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHarnessSample.Platforms.Android;

[Instrumentation(Name = "com.companyname.xharnesssample.MauiTestInstrumentation")]
public class MauiTestInstrumentation : Instrumentation
{
    public IServiceProvider Services { get; private set; } = null!;

    public override async void OnStart()
    {
        base.OnStart();

        Services = MauiApplication.Current.Services;

        var bundle = await RunTestsAsync();

        Finish(Result.Ok, bundle);
    }

    Task<Bundle> RunTestsAsync()
    {
        var runner = Services.GetRequiredService<HeadlessTestRunner>();

        return runner.RunTestsAsync();

    }
}
