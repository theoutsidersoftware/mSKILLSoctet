using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using mSKILLSoctet.Source.Config;
using mSKILLSoctet.Source.Core;
using mSKILLSoctet.Source.Core.Exceptions;

namespace mSKILLSoctet.Source.UI
{
    public class MainWindowPresenter
    {
        /// <summary>
        /// Skills that are selected.
        /// </summary>
        public ISet<Skill> SelectedSkills;

        /// <summary>
        /// Skills that can be selected.
        /// </summary>
        public ISet<Skill> AvaliableSkills;

        /// <summary>
        /// Name and description of every skill.
        /// </summary>
        private readonly IDictionary<long, Skill> _definitions;

        /// <summary>
        /// All avaliable presets.
        /// </summary>
        public IList<Preset> Presets;

        /// <summary>
        /// The currnetly selected preset.
        /// </summary>
        private Preset _currentPreset;

        /// <summary>
        /// The view that the presenter interacts with.
        /// </summary>
        private MainWindow _view;

        public MainWindowPresenter(MainWindow window)
        {
            try
            {
                SkillstrParser skillstrParser = new SkillstrParser(@"skillstr.txt");
                _definitions = skillstrParser.Parse();
            }
            catch (Exception e)
            {
                throw new SkillstrException(e);
            }

            try
            {
                PresetsParser presetParser = new PresetsParser(@"presets.ini", _definitions);
                Presets = presetParser.Parse();
                Presets.Insert(0, new Preset(""));
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Presets = null;
            }

            _view = window;
            Reset();
        }

        /// <summary>
        /// Parses the given skills octet string and set the selected skills to be the skills represented by the octet string.
        /// </summary>
        /// <param name="octet">The octet string to parse.</param>
        public void ImportOctet(string octet)
        {
            OctetParser parser = new OctetParser(octet, _definitions);
            ISet<Skill> octetSkills = parser.Parse();

            Reset();
            UsePreset(_currentPreset);

            SelectedSkills = octetSkills;
            AvaliableSkills.ExceptWith(SelectedSkills);

            _view.RefreshSkillsView();
            _view.ClearOctetTextbox();
        }

        /// <summary>
        /// Generates a skills octet string to represent the set of selected skills.
        /// </summary>
        public void GenerateOctet()
        {
            OctetSerializer serializer = new OctetSerializer(SelectedSkills);
            string octet = serializer.Serialize();

            _view.SetOctetTextbox(octet);
        }

        /// <summary>
        /// Adds skills to the selected skills, and remove it from the avaliable skills.
        /// </summary>
        /// <param name="skills">The skills to add.</param>
        public void AddSkills(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                SelectedSkills.Add(skill);
                AvaliableSkills.Remove(skill);
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Remove skills from the selected skills, and add it to the avaliable skills.
        /// </summary>
        /// <param name="skills">The skills to remove.</param>
        public void RemoveSkills(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                SelectedSkills.Remove(skill);
                AvaliableSkills.Add(skill);
            }

            _view.RefreshSkillsView();
        }

        /// <summary>
        /// Increase the level of the selected skills.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void IncreaseLevel(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Level += 1;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Decrease the level of the selected skills.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void DecreaseLevel(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Level -= 1;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Set the level of the selected skills to 10.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void MaxLevel(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Level = 10;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Set the level of the selected skills to 1.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void MinLevel(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Level = 1;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Increase the mastery of the selected skills.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void IncreaseMastery(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Mastery += 1;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Decrease the mastery of the selected skills.
        /// </summary>
        /// <param name="skills">The selected skills.</param>
        public void DecreaseMastery(IList<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                skill.Mastery -= 1;
            }

            _view.RefreshSkillsView();
            _view.SelectSelectedSkills(skills);
        }

        /// <summary>
        /// Use the selected preset as the set of avaliable skills.
        /// </summary>
        /// <param name="preset">The preset to use.</param>
        public void UsePreset(Preset preset)
        {
            _currentPreset = preset;

            AvaliableSkills = new SortedSet<Skill>(_definitions.Values);

            // If it is the null preset. If so, then it is equivalent to all skills. In which case we do nothing.
            // Otherwise, only use the set of skills in the preset.
            if (preset?.Skills != null)
            {
                AvaliableSkills.IntersectWith(preset.Skills);
            }

            AvaliableSkills.ExceptWith(SelectedSkills);

            _view.RefreshSkillsView();
        }

        /// <summary>
        /// Filter avaliable skills to only include skills whose name contains the given string.
        /// </summary>
        /// <param name="search">The string to search in the skills' name.</param>
        public void FilterName(string search)
        {
            UsePreset(_currentPreset);

            if (!string.IsNullOrEmpty(search))
            {
                ISet<Skill> filter = new HashSet<Skill>();

                foreach (Skill skill in AvaliableSkills)
                {
                    if (skill.Name.ToLower().Contains(search.ToLower()) || skill.Id.ToString().Equals(search))
                    {
                        filter.Add(skill);
                    }
                }

                AvaliableSkills.IntersectWith(filter);

                _view.RefreshSkillsView();
            }
        }

        /// <summary>
        /// Reset the avaliable skills and selected skills.
        /// </summary>
        private void Reset()
        {
            AvaliableSkills = new SortedSet<Skill>(_definitions.Values);
            SelectedSkills = new SortedSet<Skill>();
        }
    }
}
