using System;
using System.Collections.Generic;
using System.Globalization;

namespace mSKILLSoctet.Source.Core
{
    /// <summary>
    /// Parser for a skills octet.
    /// </summary>
    public class OctetParser
    {
        /// <summary>
        /// The octet to parse.
        /// </summary>
        private readonly string _octet;

        /// <summary>
        /// Name and description of every skill.
        /// </summary>
        private readonly IDictionary<long, Skill> _definitions;

        /// <summary>
        /// The current position the parser is at.
        /// </summary>
        public int Position;

        public OctetParser(string octet, IDictionary<long, Skill> definitions)
        {
            _octet = octet;
            _definitions = definitions;
            Position = 0;
        }

        /// <summary>
        /// Parses the octet string into a set of skill objects.
        /// </summary>
        /// <returns>A set of skill objects representing each skill.</returns>
        public ISet<Skill> Parse()
        {
            ISet<Skill> skills = new SortedSet<Skill>();

            Position = 0;

            int numSkills = ReadInt32();

            for (int i = 0; i < numSkills; i++)
            {
                Skill skill = ReadSkill();
                skills.Add(skill);
            }

            return skills;
        }

        /// <summary>
        /// Reads the next 4 bytes of the octet as an int32.
        /// </summary>
        /// <returns>The next 4 bytes of the octet as an int32.</returns>
        public int ReadInt32()
        {
            // We need to read 8 characters because 2 chars = 1 byte.
            string hex = _octet.Substring(Position, 8);
            int toInt = Int32.Parse(hex, NumberStyles.HexNumber);

            // Octets will be interpreted as big endian, so we need to change the byte ordering if the current machine is little endian.
            if (BitConverter.IsLittleEndian)
            {
                toInt = System.Net.IPAddress.HostToNetworkOrder(toInt);
            }

            Position += 8;

            return toInt;
        }

        /// <summary>
        /// Reads the next 12 bytes of the octet as the id and level of a skill.
        /// </summary>
        /// <returns>The skill that the next 8 byte of octet represent.</returns>
        public Skill ReadSkill()
        {
            int id = ReadInt32();
            int mastery = ReadInt32();
            int level = ReadInt32();

            if (_definitions.ContainsKey(id))
            {
                Skill skill = new Skill(_definitions[id])
                {
                    Level = level,
                    Mastery = mastery
                };
                return skill;
            }

            return new Skill(id, level, mastery);
        }
    }
}
