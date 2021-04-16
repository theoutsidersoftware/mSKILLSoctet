using System;

namespace mSKILLSoctet.Source.Core
{
    /// <summary>
    /// Represents an avaliable skill in the game.
    /// </summary>
    public class Skill : IComparable<Skill>
    {
        /// <summary>
        /// The id of the skill.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The level of the skill.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The level string to display. Includes the mastery level for crafting skills, excludes it otherwise.
        /// </summary>
        public string DisplayLevel => Mastery != 0 ? $"{Level} ({Mastery})" : Level.ToString();

        /// <summary>
        /// The mastery level of the skill.
        /// Only relevant for crafting skills.
        /// </summary>
        public int Mastery { get; set; }

        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the skill.
        /// </summary>
        public string Description { get; }

        public Skill(int id, string name, string description)
        {
            Id = id;
            Level = 1;
            Mastery = 0;
            Name = name;
            Description = description;
        }

        public Skill(int id, int level, int mastery)
        {
            Id = id;
            Level = level;
            Mastery = mastery;
            Name = "?";
            Description = "?";
        }

        public Skill(Skill skill)
        {
            Id = skill.Id;
            Level = skill.Level;
            Mastery = skill.Mastery;
            Name = skill.Name;
            Description = skill.Description;
        }

        public override int GetHashCode()
        {
            return 7 * Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Skill other && Equals(other);
        }

        public bool Equals(Skill other)
        {
            return Id == other.Id;
        }

        public int CompareTo(Skill other)
        {
            // Returns positive (greater) if higher id, and negative (less) if lower id.
            return Id - other.Id;
        }
    }
}
