#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace DuckAdventure.Agent
{
    /// <summary>Local development telemetry. This entire type is absent from release builds.</summary>
    public sealed class MarshPacingTelemetry : MonoBehaviour
    {
        [Serializable]
        public sealed class Section
        {
            public string name;
            public float activeSeconds;
            public float readingSeconds;
            public int deaths;
            public bool completed;
        }
        [Serializable]
        public sealed class Record
        {
            public string utc;
            public string runId;
            public string runLabel;
            public string eventName;
            public string detail;
            public int amount;
            public int section;
            public bool teleported;
            public Section[] sections;
        }
        private MarshLevel level;
        private GameObject overlay;
        private Text overlayText;
        private Section[] sections;
        private string runId;
        private string runLabel = "human";
        private string logPath;
        private int section;
        private float nextHeartbeat;
        private bool teleported;
        private bool configured;
        private bool finished;
        public Section[] Sections => sections;
        public string LogPath => logPath;
        public bool Teleported => teleported;
        public bool OverlayVisible => overlay.activeSelf;

        public void Configure(MarshLevel owner, Transform hud, Font font)
        {
            if (configured) throw new InvalidOperationException("Pacing telemetry was configured twice.");
            if (owner == null || hud == null || font == null) throw new InvalidOperationException("Pacing telemetry needs explicit level, HUD and font references.");
            level = owner;
            string[] names = { "Landing to Reedbank", "Reedbank to Archive", "Archive to Haven", "Haven to reunion" };
            sections = new Section[names.Length];
            for (int i = 0; i < sections.Length; i++) sections[i] = new Section { name = names[i] };
            foreach (Text existing in hud.GetComponentsInChildren<Text>(true))
                if (existing.name == "MarshPacingOverlayText") throw new InvalidOperationException("Stale pacing overlay found in the HUD.");
            overlay = new GameObject("MarshPacingOverlay", typeof(RectTransform), typeof(Image));
            RectTransform panel = (RectTransform)overlay.transform;
            panel.SetParent(hud, false); panel.anchorMin = Vector2.one; panel.anchorMax = Vector2.one;
            panel.pivot = Vector2.one; panel.anchoredPosition = new Vector2(-22, -150); panel.sizeDelta = new Vector2(450, 185);
            Image background = overlay.GetComponent<Image>(); background.color = new Color(0.04f, 0.09f, 0.12f, 0.97f); background.raycastTarget = false;
            GameObject textObject = new GameObject("MarshPacingOverlayText", typeof(RectTransform), typeof(Text));
            RectTransform textRect = (RectTransform)textObject.transform;
            textRect.SetParent(panel, false); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12, 10); textRect.offsetMax = new Vector2(-12, -10);
            overlayText = textObject.GetComponent<Text>(); overlayText.font = font; overlayText.fontSize = 14;
            overlayText.color = new Color(0.94f, 0.91f, 0.78f); overlayText.raycastTarget = false;
            overlay.SetActive(false);
            runId = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string directory = Path.Combine(Application.persistentDataPath, "Marshlight", "Telemetry");
            Directory.CreateDirectory(directory);
            logPath = Path.Combine(directory, "pacing-" + runId + ".jsonl");
            configured = true;
            WriteEvent("run_started", 0, "Editor/development only; pause, title and defeat waiting excluded");
        }

        private void EnsureConfigured()
        {
            if (!configured) throw new InvalidOperationException("MarshPacingTelemetry must be configured by MarshLevel.");
        }
        public void SetRunLabel(string value) { EnsureConfigured(); runLabel = value; WriteEvent("run_label", 0, value); }
        public void ToggleOverlay() { EnsureConfigured(); overlay.SetActive(!overlay.activeSelf); RenderOverlay(); }
        public void ReachedCheckpoint(int index)
        {
            EnsureConfigured();
            if (index > section) { sections[section].completed = true; WriteEvent("section_complete", 0, sections[section].name); section = index; }
            WriteEvent("checkpoint", index, sections[section].name);
        }
        public void Died() { EnsureConfigured(); sections[section].deaths++; WriteEvent("death", 1, sections[section].name); }
        public void MarkTeleport(int index) { EnsureConfigured(); teleported = true; section = index; WriteEvent("debug_teleport", index, "Excluded from pace estimates"); }
        public void Finished()
        {
            EnsureConfigured();
            if (finished) return;
            finished = true; sections[section].completed = true; WriteEvent("finished", 0, "Victory");
        }
        public void WriteEvent(string eventName, int amount, string detail)
        {
            EnsureConfigured();
            Record record = new Record { utc = DateTime.UtcNow.ToString("o"), runId = runId, runLabel = runLabel, eventName = eventName, detail = detail, amount = amount, section = section, teleported = teleported, sections = sections };
            File.AppendAllText(logPath, JsonUtility.ToJson(record) + Environment.NewLine);
        }
        public string SnapshotJson()
        {
            EnsureConfigured();
            return JsonUtility.ToJson(new Record { utc = DateTime.UtcNow.ToString("o"), runId = runId, runLabel = runLabel, eventName = "snapshot", section = section, teleported = teleported, sections = sections }, true);
        }
        private void Update()
        {
            EnsureConfigured();
            if (level.State == MarshState.Playing) sections[section].activeSeconds += Time.unscaledDeltaTime;
            else if (level.State == MarshState.Dialogue) sections[section].readingSeconds += Time.unscaledDeltaTime;
            if (Time.unscaledTime >= nextHeartbeat) { nextHeartbeat = Time.unscaledTime + 5; WriteEvent("heartbeat", 0, ""); }
            if (overlay.activeSelf) RenderOverlay();
        }
        private void RenderOverlay()
        {
            StringBuilder text = new StringBuilder("PACING  /  F7 HIDE\n" + runLabel + (teleported ? " — TELEPORTED, EXCLUDE" : " — no teleport") + "\n");
            for (int i = 0; i < sections.Length; i++)
            {
                Section row = sections[i];
                text.Append(i == section ? "> " : "  ").Append(row.name).Append(": ").Append(row.activeSeconds.ToString("0.0")).Append("s + ").Append(row.readingSeconds.ToString("0.0")).Append("s read; ").Append(row.deaths).Append(" deaths\n");
            }
            overlayText.text = text.ToString();
        }
        private void OnDestroy()
        {
            if (configured) WriteEvent("scene_closed", 0, finished ? "completed" : "incomplete");
        }
    }
}
#endif
