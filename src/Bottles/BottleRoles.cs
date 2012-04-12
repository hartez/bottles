﻿namespace Bottles
{
    public static class BottleRoles
    {
        /// <summary>
        /// The bottle should just contain dlls. Useful for getting 3rd party dlls
        /// into the right spot during deployment.
        /// </summary>
        public const string Binaries = "binaries";

        /// <summary>
        /// Bottles that extend 'application' with new functionality. Another
        /// way to express this is 'plugin'
        /// </summary>
        public const string Module = "module";

        /// <summary>
        /// This bottle should just contain config files.
        /// </summary>
        public const string Config = "config";

        /// <summary>
        /// This represents an application like a console or website project
        /// </summary>
        public const string Application = "application";


        /// <summary>
        /// Bottles that are strictly data. aka just files
        /// </summary>
        public const string Data = "data";
    }
}