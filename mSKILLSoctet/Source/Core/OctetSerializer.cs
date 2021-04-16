using System;
using System.Collections.Generic;

namespace mSKILLSoctet.Source.Core
{
    /// <summary>
    /// Serializes a set of skills into octet.
    /// </summary>
    public class OctetSerializer
    {
        private readonly ISet<Skill> _skills;

        private string _octet;

        public OctetSerializer(ISet<Skill> skills)
        {
            _skills = skills;
            _octet = "";
        }

        /// <summary>
        /// Serialize the set of skills into octet.
        /// </summary>
        /// <returns>The octet representing the set of skills..</returns>
        public string Serialize()
        {
            _octet = "";

            // Number of skills.
            WriteInt32(_skills.Count);

            foreach (Skill skill in _skills)
            {
                WriteSkill(skill);
            }

            return _octet;
        }

        /// <summary>
        /// Writes an int as an octet.
        /// </summary>
        /// <param name="toOctet">The int to serialize.</param>
        public void WriteInt32(int toOctet)
        {
            // Octets are stored as big endian, so we need to change the byte ordering if the current machine is little endian.
            if (BitConverter.IsLittleEndian)
            {
                toOctet = System.Net.IPAddress.NetworkToHostOrder(toOctet);
            }

            _octet += toOctet.ToString("x8");
        }

        /// <summary>
        /// Writes a skill as an octet.
        /// </summary>
        /// <param name="skill">The skill to serialize.</param>
        public void WriteSkill(Skill skill)
        {
            WriteInt32(skill.Id);
            WriteInt32(skill.Mastery);
            WriteInt32(skill.Level);
        }
    }
}
