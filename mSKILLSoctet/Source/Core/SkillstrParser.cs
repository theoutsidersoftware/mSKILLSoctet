using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace mSKILLSoctet.Source.Core
{
    /// <summary>
    /// Parser for skillstr.txt.
    /// </summary>
    public class SkillstrParser
    {
        /// <summary>
        /// The path to skillstr.txt.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Initializes the parser for skillstr.txt located at the given path.
        /// </summary>
        /// <param name="path">The path to skillstr.txt.</param>
        public SkillstrParser(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Reads skillstr.txt and returns the raw skillstr.
        /// </summary>
        /// <returns>The raw skillstr.</returns>
        public string Read()
        {
            return File.ReadAllText(_path);
        }

        /// <summary>
        /// Reads skillstr.txt and parses it into a dictionary. The key is the id of the skill and the value is the skill object.
        /// </summary>
        /// <returns>A dictionary where the key is the id of the skill and the value is the skill object.</returns>
        /// <exception cref="InvalidDataException">If skillstr.txt is malformed.</exception>
        public IDictionary<long, Skill> Parse()
        {
            IDictionary<long, Skill> skills = new Dictionary<long, Skill>();

            string skillstr = Read();

            // This regex parses the skill name and description at the same time, as a pair.
            Regex regex = new Regex(@"([0-9]+)0\s+""(.*?)"".*?([0-9]+)1\s+""(.*?)""\s*?", RegexOptions.Singleline);

            Match match = regex.Match(skillstr);
            while (match.Success)
            {
                int id = int.Parse(match.Groups[1].Value);
                string name = match.Groups[2].Value;
                int idCheck = int.Parse(match.Groups[3].Value);
                string description = CleanDescription(match.Groups[4].Value);

                // Sanity check to ensure that we are matching the name to the correct description. This should never fail if
                // skillstr.txt is properly formatted.
                if (id != idCheck)
                {
                    throw new InvalidDataException("skillstr.txt is malformed");
                }

                Skill skill = new Skill(id, name, description);
                skills.Add(id, skill);

                match = match.NextMatch();
            }

            return skills;
        }

        /// <summary>
        /// Cleans a description string by removing all the colour tags and cleaning double percent signs.
        /// </summary>
        /// <param name="description">The description string to clean.</param>
        /// <returns>The cleaned description string.</returns>
        public string CleanDescription(string description)
        {
            // This regex searches for the colour tags.
            Regex regex = new Regex(@"\^[a-zA-Z0-9]{6}", RegexOptions.Singleline);

            return regex.Replace(description, "").Replace("%%", "%");
        }
    }
}