using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Marshlight.TestRecording
{
    [InitializeOnLoad]
    public static class MarshTestResultRecorder
    {
        [Serializable] public sealed class Case
        {
            public string name;
            public string state;
            public int assertions;
            public string message;
        }
        [Serializable] public sealed class Report
        {
            public string utc;
            public string state;
            public int passed;
            public int failed;
            public int skipped;
            public int assertions;
            public Case[] cases;
        }
        private static TestRunnerApi api;
        static MarshTestResultRecorder()
        {
            api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new Recorder());
        }
        private sealed class Recorder : ICallbacks
        {
            public void RunStarted(ITestAdaptor test) { SessionState.SetString("Marshlight.Iteration1.TestStatus", "Running"); }
            public void TestStarted(ITestAdaptor test) { }
            public void TestFinished(ITestResultAdaptor result) { }
            public void RunFinished(ITestResultAdaptor result)
            {
                Case[] cases = Leaves(result).Where(r => r.FullName.StartsWith("Marshlight.PlayModeTests.")).Select(r => new Case { name = r.FullName, state = r.ResultState, assertions = r.AssertCount, message = r.Message }).ToArray();
                if (cases.Length == 0) return;
                Report report = new Report { utc = DateTime.UtcNow.ToString("o"), state = result.ResultState, passed = cases.Count(c => c.state == "Passed"), failed = cases.Count(c => c.state == "Failed"), skipped = cases.Count(c => c.state != "Passed" && c.state != "Failed"), assertions = cases.Sum(c => c.assertions), cases = cases };
                string json = JsonUtility.ToJson(report, true);
                string directory = Path.Combine(Application.persistentDataPath, "Marshlight", "Tests"); Directory.CreateDirectory(directory);
                File.WriteAllText(Path.Combine(directory, "iteration1-latest.json"), json);
                SessionState.SetString("Marshlight.Iteration1.TestStatus", json);
            }
            private static IEnumerable<ITestResultAdaptor> Leaves(ITestResultAdaptor result)
            {
                if (!result.HasChildren) { yield return result; yield break; }
                foreach (ITestResultAdaptor child in result.Children) foreach (ITestResultAdaptor leaf in Leaves(child)) yield return leaf;
            }
        }
    }
}
