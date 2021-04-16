using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using mSKILLSoctet.Source.Config;
using mSKILLSoctet.Source.Core;
using mSKILLSoctet.Source.Core.Exceptions;

namespace mSKILLSoctet.Source.UI
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _presenter = new MainWindowPresenter(this);
                SelectedSkillsView.ItemsSource = _presenter.SelectedSkills;
                AvaliableSkillsView.ItemsSource = _presenter.AvaliableSkills;

                if (_presenter.Presets != null)
                {
                    PresetsSelector.ItemsSource = _presenter.Presets;
                }
                else
                {
                    MessageBox.Show(Properties.Strings.PresetsNotFound, Properties.Strings.ConfigurationFailed,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
            catch (SkillstrException)
            {
                MessageBox.Show(Properties.Strings.SkillstrNotFound, Properties.Strings.ConfigurationFailed,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Refresh the selected and avaliable skills itemlist with the new item source.
        /// </summary>
        public void RefreshSkillsView()
        {
            SelectedSkillsView.ItemsSource = null;
            SelectedSkillsView.ItemsSource = _presenter.SelectedSkills;

            AvaliableSkillsView.ItemsSource = null;
            AvaliableSkillsView.ItemsSource = _presenter.AvaliableSkills;
        }

        /// <summary>
        /// Selects the given skills in the select skills listview.
        /// </summary>
        /// <param name="skills">The skills to select.</param>
        public void SelectSelectedSkills(IEnumerable<Skill> skills)
        {
            SelectedSkillsView.SelectItems(skills);
        }

        /// <summary>
        /// Sets the octet textbox to contain the given octet.
        /// </summary>
        /// <param name="octet"></param>
        public void SetOctetTextbox(string octet)
        {
            OctetTextbox.Text = octet;
        }

        /// <summary>
        /// Clears the octet textbox.
        /// </summary>
        public void ClearOctetTextbox()
        {
            OctetTextbox.Clear();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _presenter.ImportOctet(OctetTextbox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Strings.OctetInvalid, Properties.Strings.ImportOctetFailed, MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.GenerateOctet();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.FilterName(OctetTextbox.Text);
        }

        private void IncreaseLevelButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.IncreaseLevel(skills);
        }

        private void DecreaseLevelButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.DecreaseLevel(skills);
        }

        private void AddSkill_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = AvaliableSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.AddSkills(skills);
        }

        private void RemoveSkill_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.RemoveSkills(skills);
        }

        private void MinLevelButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.MinLevel(skills);
        }

        private void MaxLevelButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.MaxLevel(skills);
        }

        private void PresetsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preset preset = (Preset)PresetsSelector.SelectedItem;
            _presenter.UsePreset(preset);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.Show();
        }

        private void PayStripeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://plasso.com/s/VCeoRPJhKo");
        }

        private void PayPalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.paypal.com/cgi-bin/webscr";

            string cmd = "_xclick";
            string currency_code = "CAD";
            string business = "822WZLSHSFD9Q";
            string item_name = "mSKILLSoctet";
            string button_subtype = "services";
            string bn = "PP-BuyNowBF:btn_buynowCC_LG.gif:NonHosted";

            url +=
                "?cmd=" + cmd +
                "&currency_code=" + currency_code +
                "&business=" + business +
                "&item_name=" + item_name +
                "&button_subtype=" + button_subtype +
                "&bn=" + bn;

            Process.Start(url);
        }

        private void DecreaseMasteryButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.DecreaseMastery(skills);
        }

        private void IncreaseMasteryButton_Click(object sender, RoutedEventArgs e)
        {
            IList<Skill> skills = SelectedSkillsView.SelectedItems.Cast<Skill>().ToList();
            _presenter.IncreaseMastery(skills);
        }
    }
}
