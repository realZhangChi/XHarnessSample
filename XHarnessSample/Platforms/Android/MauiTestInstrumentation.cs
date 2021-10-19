using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHarnessSample.Platforms.Android;

namespace XHarnessSample;

[Instrumentation(Name = "com.companyname.xharnesssample.MauiTestInstrumentation")]
public class MauiTestInstrumentation : Instrumentation
{
    public IServiceProvider Services { get; private set; } = null!;
    readonly TaskCompletionSource<Application> _waitForApplication = new();

    public override void CallApplicationOnCreate(Application app)
    {
        base.CallApplicationOnCreate(app);

        if (app == null)
            _waitForApplication.SetException(new ArgumentNullException(nameof(app)));
        else
            _waitForApplication.SetResult(app);
    }

    public MauiTestInstrumentation(IntPtr handle, JniHandleOwnership transfer)
        : base(handle, transfer)
    {

    }

    public override void OnCreate(Bundle arguments)
    {
        base.OnCreate(arguments);

        Start();
    }

    public override async void OnStart()
    {
        base.OnStart();

        await _waitForApplication.Task;

        Services = MauiApplication.Current.Services;

        var bundle = await RunTestsAsync();

		CopyFile(bundle);

		Finish(Result.Ok, bundle);
    }

    Task<Bundle> RunTestsAsync()
    {
        var runner = Services.GetRequiredService<HeadlessTestRunner>();

        return runner.RunTestsAsync();

    }

	void CopyFile(Bundle bundle)
	{
		var resultsFile = bundle.GetString("test-results-path");
		if (resultsFile == null)
			return;

		var guid = Guid.NewGuid().ToString("N");
		var name = Path.GetFileName(resultsFile);

		string finalPath;
		if ((int)Build.VERSION.SdkInt < 30)
		{
			var root = Application.Context.GetExternalFilesDir(null)!.AbsolutePath!;
			var dir = Path.Combine(root, guid);

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			finalPath = Path.Combine(dir, name);
			File.Copy(resultsFile, finalPath, true);
		}
		else
		{
			var downloads = Android.OS.Environment.DirectoryDownloads!;
			var relative = Path.Combine(downloads, Context!.PackageName!, guid);

			var values = new ContentValues();
			values.Put(MediaStore.IMediaColumns.DisplayName, name);
			values.Put(MediaStore.IMediaColumns.MimeType, "text/xml");
			values.Put(MediaStore.IMediaColumns.RelativePath, relative);

			var resolver = Context!.ContentResolver!;
			var uri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, values)!;
			using (var dest = resolver.OpenOutputStream(uri)!)
			using (var source = File.OpenRead(resultsFile))
				source.CopyTo(dest);

#pragma warning disable CS0618 // Type or member is obsolete
			var root = Android.OS.Environment.ExternalStorageDirectory!.AbsolutePath;
#pragma warning restore CS0618 // Type or member is obsolete
			finalPath = Path.Combine(root, relative, name);
		}

		bundle.PutString("test-results-path", finalPath);
	}
}
