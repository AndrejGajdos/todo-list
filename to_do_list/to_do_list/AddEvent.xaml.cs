/*
 ============================================================================
 Name        : AddEvent.xaml.cs
 Author      : Andrej Gajdos
 Version     : 3.6.2013
 Copyright   : 359234@mail.muni.cz
 Description : AddEvent.xaml class
 ============================================================================
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace To_Do_List_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEvent : Page
    {
        #region FIELDS

        private bool validEstimatedLength = false;
        private bool create = false;
        private EventManager man = new EventManager();

        private Event currentEvent;
        public Event CurrentEvent
        {
            get
            {
                return this.currentEvent;
            }

            set
            {
                if (value != null)
                {
                    this.currentEvent = value;
                }

            }
        }

        #endregion

        public AddEvent()
        {
            this.InitializeComponent(); 

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.CurrentEvent = new Event();
            this.CurrentEvent = e.Parameter as Event;

            if (this.CurrentEvent.Id != null)
            {
                this.create = false;
                EventNameTextBox.Text = this.CurrentEvent.Name;
                EventDescriptionTextBox.Text = this.CurrentEvent.Description;
                EventPriorityComboBox.SelectedIndex = (int)this.CurrentEvent.Priority;
                EventStatusComboBox.SelectedIndex = (int)this.CurrentEvent.Status;
                EventEstimatedLengthComboBox.Text = this.CurrentEvent.EstimatedLength.ToString();
                StartDatePicker.SelectedDate = this.CurrentEvent.StartDate;
                EndDatePicker.SelectedDate = this.CurrentEvent.EndDate;
            }
            else
                this.create = true;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            this.CurrentEvent.Name = EventNameTextBox.Text;
            this.CurrentEvent.Description = EventDescriptionTextBox.Text;
            this.CurrentEvent.Priority = (EventPriority)EventPriorityComboBox.SelectedIndex;
            this.CurrentEvent.Status = (EventStatus)EventStatusComboBox.SelectedIndex;
            this.CurrentEvent.EstimatedLength = ParseTimeSpan(EventEstimatedLengthComboBox.Text);
            this.CurrentEvent.StartDate = StartDatePicker.SelectedDate;
            this.CurrentEvent.EndDate = StartDatePicker.SelectedDate;

            if (validEstimatedLength)
            {
                if (create)
                    man.CreateEvent(this.CurrentEvent);
                else
                    man.UpdateEvent(this.CurrentEvent);

                if (this.Frame != null)
                {
                    this.Frame.Navigate(typeof(MainPage), this.CurrentEvent);
                }
            }
        }

        private void Image_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

        }

        private void EventEstimatedLengthComboBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TimeSpan a = ParseTimeSpan(EventEstimatedLengthComboBox.Text);
        }

        public TimeSpan ParseTimeSpan(string s)
        {
            const string Quantity = "quantity";
            const string Unit = "unit";

            const string Days = @"(d(ays?)?)";
            const string Hours = @"(h((ours?)|(rs?))?)";
            const string Minutes = @"(m((inutes?)|(ins?))?)";
            const string Seconds = @"(s((econds?)|(ecs?))?)";

            Regex timeSpanRegex = new Regex(
                string.Format(@"\s*(?<{0}>\d+)\s*(?<{1}>({2}|{3}|{4}|{5}|\Z))",
                              Quantity, Unit, Days, Hours, Minutes, Seconds),
                              RegexOptions.IgnoreCase);
            MatchCollection matches = timeSpanRegex.Matches(s);

            if ((matches.Count == 0) && (s != ""))
            {
                validEstimatedLength = false;
                ErrorTextBlock.Visibility = Visibility.Visible;
                EventEstimatedLengthComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                validEstimatedLength = true;
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                EventEstimatedLengthComboBox.BorderBrush = new SolidColorBrush(Colors.White);
            }

            TimeSpan ts = new TimeSpan();
            foreach (Match match in matches)
            {
                if (Regex.IsMatch(match.Groups[Unit].Value, @"\A" + Days))
                {
                    ts = ts.Add(TimeSpan.FromDays(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Hours))
                {
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Minutes))
                {
                    ts = ts.Add(TimeSpan.FromMinutes(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Seconds))
                {
                    ts = ts.Add(TimeSpan.FromSeconds(double.Parse(match.Groups[Quantity].Value)));
                }
                else
                {
                    // Quantity given but no unit, default to Hours
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[Quantity].Value)));
                }
            }
            return ts;
        }


    }
}
