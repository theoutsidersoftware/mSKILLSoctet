using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using mSKILLSoctet.Source.Core;

namespace mSKILLSoctet.Source.Config
{
    public class PresetsParser
    {
        /// <summary>
        /// The path to the presets.ini file.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Name and description of every skill.
        /// </summary>
        private readonly IDictionary<long, Skill> _definitions;

        /// <summary>
        /// Initializes the parser for presets.ini located at the given path.
        /// </summary>
        /// <param name="path">The path to presets.ini.</param>
        /// <param name="definitions">Name and description of every skill.</param>
        public PresetsParser(string path, IDictionary<long, Skill> definitions)
        {
            _path = path;
            _definitions = definitions;
        }

        /// <summary>
        /// Reads presets.ini and returns the raw ini, line by line.
        /// </summary>
        /// <returns>The raw ini, line by line.</returns>
        public string[] Read()
        {
            return File.ReadAllLines(_path);
        }

        /// <summary>
        /// Reads presets.ini and parses it into a list preset object.
        /// </summary>
        /// <returns>A list of Preset objects, each representing a preset.</returns>
        public IList<Preset> Parse()
        {
            IList<Preset> presets = new List<Preset>();

            string[] lines = Read();

            Preset currentPreset = null;
            foreach (string line in lines)
            {
                // Try to match a section start. The name of the section is the name of the preset.
                string name = TryParseName(line);
                if (name != null)
                {
                    if (currentPreset != null)
                    {
                        presets.Add(currentPreset);
                    }

                    currentPreset = new Preset(name);
                    continue;
                }

                // Try to match a setting.
                string octet = TryParseSkills(line);
                if (octet != null)
                {
                    OctetParser octetParser = new OctetParser(octet, _definitions);
                    if (currentPreset != null)
                    {
                        currentPreset.Skills = octetParser.Parse();
                    }

                    continue;
                }
            }

            if (currentPreset != null)
            {
                presets.Add(currentPreset);
            }

            return presets;
        }

        /// <summary>
        /// Try to parse the name of a preset.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The name of the preset, or null if unable to parse.</returns>
        public string TryParseName(string line)
        {
            Regex regex = new Regex(@"\[(\w+)\]", RegexOptions.None);
            Match match = regex.Match(line);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// Try to parse a skill octet for a preset.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>An the skill octet.</returns>
        public string TryParseSkills(string line)
        {
            // Try to match a setting.
            Regex regex = new Regex(@"(\w+)=(.+)", RegexOptions.None);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (key == "skills" && value.Length > 0)
                {
                    return value;
                }
            }

            return null;
        }
    }
}
