using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lench.AdvancedControls.UI
{
    internal class ACMUpdater : Scripter.Updater
    {
        public override string Name { get { return "ACM Updater"; } }

        /// <summary>
        /// Current installed version.
        /// </summary>
        public override Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// GitHub API URL for checking the latest release.
        /// </summary>
        public override string APIURL { get; set; } = "https://api.github.com/repos/lench4991/AdvancedControlsMod/releases";

        /// <summary>
        /// Update checker window name.
        /// </summary>
        public override string WindowName { get; set; } = "Advanced Controls Mod";

        /// <summary>
        /// Links to be displayed below the notification.
        /// </summary>
        public override List<Link> Links { get; set; } = new List<Link>()
            {
                new Link() { DisplayName = "Spiderling forum page", URL = "http://forum.spiderlinggames.co.uk/index.php?threads/3150/" },
                new Link() { DisplayName = "GitHub release page", URL = "https://github.com/lench4991/AdvancedControlsMod/releases/latest"}
            };

    }
}