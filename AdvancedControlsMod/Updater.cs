using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;

namespace AdvancedControls.UI
{
    public class Updater : SingleInstance<Updater>
    {
        public override string Name { get { return "ACM Update Checker"; } }

        public bool UpdateAvailable { get; private set; }

        private IEnumerator Start()
        {
            var www = new WWW(
              "https://api.github.com/repos/lench4991/AdvancedControlsMod/releases");

            yield return www;

            if (!www.isDone || !string.IsNullOrEmpty(www.error))
            {
                yield break;
            }

            string response = www.text;

            var releases = JSON.Parse(response);
            var latestVersion = releases[0]["tag_name"];
            var latestName = releases[0]["name"].ToString().Trim('"');
            var latestBody = releases[0]["body"].ToString().Trim('"');

            var newestVersion = new Version(latestVersion.ToString().Trim('"', 'v'));

            var myVersion = AdvancedControlsMod._version;

            if (newestVersion > myVersion)
            {
                UpdateAvailable = true;
                Debug.Log("[ACM]: Update available (v"+newestVersion+": "+latestName+"):");
                Debug.Log("\t"+latestBody.Replace(@"\r\n", "\n\t"));
            }
        }
    }
}