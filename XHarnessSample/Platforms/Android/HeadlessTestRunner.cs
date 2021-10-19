using Android.App;
using Android.OS;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Microsoft.DotNet.XHarness.TestRunners.Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHarnessSample.Platforms.Android;

public class HeadlessTestRunner : AndroidApplicationEntryPoint
{
    readonly string _resultsPath;

    public override TextWriter Logger => null;

    public override string TestsResultsFinalPath => _resultsPath;

    protected override int? MaxParallelThreads => System.Environment.ProcessorCount;

    protected override IDevice Device => new TestDevice();

    public HeadlessTestRunner(string testResultsFileName)
    {
        var cache = Application.Context.CacheDir!.AbsolutePath;
        _resultsPath = Path.Combine(cache, testResultsFileName);
    }

    protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
    {
        var assembly = typeof(MauiProgram).Assembly;
        var path = Path.Combine(Application.Context.CacheDir!.AbsolutePath, assembly.GetName().Name + ".dll");
        if (!File.Exists(path))
            File.Create(path).Close();
        yield return new TestAssemblyInfo(assembly, path);
    }

    protected override void TerminateWithSuccess()
    {
    }

    public async Task<Bundle> RunTestsAsync()
    {
        var bundle = new Bundle();

        TestsCompleted += OnTestsCompleted;

        await RunAsync();

        TestsCompleted -= OnTestsCompleted;

        if (File.Exists(TestsResultsFinalPath))
            bundle.PutString("test-results-path", TestsResultsFinalPath);

        if (bundle.GetLong("return-code", -1) == -1)
            bundle.PutLong("return-code", 1);

        return bundle;

        void OnTestsCompleted(object sender, TestRunResult results)
        {
            var message =
                $"Tests run: {results.ExecutedTests} " +
                $"Passed: {results.PassedTests} " +
                $"Inconclusive: {results.InconclusiveTests} " +
                $"Failed: {results.FailedTests} " +
                $"Ignored: {results.SkippedTests}";

            bundle.PutString("test-execution-summary", message);

            bundle.PutLong("return-code", results.FailedTests == 0 ? 0 : 1);
        }
    }
}
