using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using mSKILLSoctet.Source.Core;

namespace mSKILLSoctet.Source.Config
{
    public class Preset
    {
        /// <summary>
        /// The name of the preset;
        /// </summary>
        public string Name;

        /// <summary>
        /// Set of skills in this preset.
        /// If this set is null, then the preset contains all skills.
        /// </summary>
        public ISet<Skill> Skills { get; set; }

        public Preset(string name)
        {
            Name = name;

            // The empty string is a special case. If Skills is null, then the preset contains all skills.
            if (name != "")
            {
                Skills = new HashSet<Skill>();
            }
            else
            {
                Skills = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
