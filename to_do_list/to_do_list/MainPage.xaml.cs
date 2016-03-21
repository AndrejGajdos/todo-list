/*
 ============================================================================
 Name        : MainPage.xaml.cs
 Author      : Andrej Gajdos
 Version     : 3.6.2013
 Copyright   : 359234@mail.muni.cz
 Description : MainPage.xaml class
 ============================================================================
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel; 
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTDatePicker;
using System.Reactive.Linq;
using System.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace To_Do_List_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region FIELDS

        private EventManager man = new EventManager();
        private string filename = "events.xml";

        // find whether ListView columns were ordered ascending
        Dictionary<string, bool> isAscending = new Dictionary<string, bool>{
            { "name", false },
            { "description", false },
            { "status", false },
            { "priority", false },
            { "estimatedLength", false },
            { "startDate", false },
            { "endDate", false }
        };

        private Event selectedEvent;
        public Event SelectedEvent
        {
            get
            {
                return this.selectedEvent;
            }

            set
            {
                if (value != null)
                {
                    this.selectedEvent = value;
                }

            }
        }

        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events
        {
            get
            {
                return this.events;
            }

            set
            {
                if (value != null)
                {
                    this.events = value;
                }

            }
        }

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.SelectedEvent = new Event();
            this.Events = new ObservableCollection<Event>();

            eventsTable.Loaded += eventsTable_Loaded;
            eventsTable.LayoutUpdated += eventsTable_LayoutUpdated;
        }

        #region ListView help methods

        void eventsTable_LayoutUpdated(object sender, object e)
        {
            setWidth();
        }

        void eventsTable_Loaded(object sender, RoutedEventArgs e)
        {
            setWidth();
        }

        void setWidth()
        {
            var runtimeWidth = eventsTable.ActualWidth;

            var grids = UIHelper.FindChildrenByName<Grid>(eventsTable, "Data");

            if (grids != null)
            {
                foreach (var item in grids)
                {
                    item.Width = runtimeWidth;
                }
            }
        }

        #endregion

        private async void LoadEvents()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.GetFileAsync(filename);    
                string result = await FileIO.ReadTextAsync(file);
                this.Events = CollectionExtensions.ToObservableCollection(man.GetAllEvents(result));
                eventsTable.ItemsSource = this.Events;
            }
            catch (Exception)
            {
                CreateEventFile();
            } 

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            filterInDay.SelectedDateChanged += filterInDayOnSelectedDateChanged;
            filterInWeek.SelectedDateChanged += filterInWeekOnSelectedDateChanged;
            filterInMonth.SelectedDateChanged += filterInMonthOnSelectedDateChanged;

            this.SelectedEvent = e.Parameter as Event;

            if (e.Parameter.ToString() != "")
            {
                this.Events = (ObservableCollection<Event>)App.Current.Resources["AllEvents"];

                int numberOfItems = this.Events.Count;

                if (numberOfItems == 0)
                    this.Events.Add(this.SelectedEvent);

                for (int i = 0; i < numberOfItems; i++)
                {
                    if (this.Events[i].Id == this.SelectedEvent.Id)
                    {
                        this.Events[i] = this.SelectedEvent;
                        break;
                    }

                    if (i == numberOfItems-1)
                        this.Events.Add(this.SelectedEvent);
                }

                eventsTable.ItemsSource = this.Events;
                this.SelectedEvent = null;
                return;
            }

            this.LoadEvents();

        }

        #region Filters handlers

        private void filterInDayOnSelectedDateChanged(object sender, SelectedDateChangedEventArgs selectedDateChangedEventArgs)
        {
            this.Events = CollectionExtensions.ToObservableCollection(man.GetEventsInDay(selectedDateChangedEventArgs.NewDate));
            this.eventsTable.ItemsSource = this.Events;
        }

        private void filterInWeekOnSelectedDateChanged(object sender, SelectedDateChangedEventArgs selectedDateChangedEventArgs)
        {
            this.Events = CollectionExtensions.ToObservableCollection(man.GetEventsInWeek(selectedDateChangedEventArgs.NewDate));
            this.eventsTable.ItemsSource = this.Events;
        }

        private void filterInMonthOnSelectedDateChanged(object sender, SelectedDateChangedEventArgs selectedDateChangedEventArgs)
        {
            this.Events = CollectionExtensions.ToObservableCollection(man.GetEventsInMonth(selectedDateChangedEventArgs.NewDate));
            this.eventsTable.ItemsSource = this.Events;
        }

        private void searchTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            this.Events = CollectionExtensions.ToObservableCollection(man.SearchEvents(searchTextBox.Text));
            this.eventsTable.ItemsSource = this.Events;
        }

        #endregion

        #region ListView column pressed listeners

        private void orderName_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["name"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.Name));
                isAscending["name"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.Name));
                isAscending["name"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderDescription_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["description"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.Description));
                isAscending["description"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.Description));
                isAscending["description"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderStatus_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["status"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.Status));
                isAscending["status"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.Status));
                isAscending["status"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderPriority_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["priority"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.Priority));
                isAscending["priority"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.Priority));
                isAscending["priority"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderEstimatedLength_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["estimatedLength"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.EstimatedLength));
                isAscending["estimatedLength"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.EstimatedLength));
                isAscending["estimatedLength"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderStartDate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["startDate"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.StartDate));
                isAscending["startDate"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.StartDate));
                isAscending["startDate"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        private void orderEndDate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isAscending["endDate"])
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderByDescending(eve => eve.EndDate));
                isAscending["endDate"] = false;
            }
            else
            {
                this.Events = CollectionExtensions.ToObservableCollection(this.Events.OrderBy(eve => eve.EndDate));
                isAscending["endDate"] = true;
            }
            this.eventsTable.ItemsSource = this.Events;
        }

        #endregion

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                Application.Current.Resources["AllEvents"] = this.Events;
                this.Frame.Navigate(typeof(AddEvent));
            }
        }

        private void eventsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedEvent = (Event)eventsTable.SelectedItem;
        }

        private void eventsTable_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.SelectedEvent.Id != null)
            {
                if (this.Frame != null)
                {
                    Application.Current.Resources["AllEvents"] = this.Events;
                    this.Frame.Navigate(typeof(AddEvent), this.SelectedEvent);
                }
            }
        }

        private async void ShowErrMessage(string message)
        {
            MessageDialog dlg = new MessageDialog(message); await dlg.ShowAsync();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.SelectedEvent.Id != null)
            {
                man.DeleteEvent(this.SelectedEvent);
                this.Events.Remove(this.SelectedEvent);
                return;
            }
            ShowErrMessage("Please select an event");
        }

        public async static void CreateEventFile()
        {
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await localFolder.CreateFileAsync("events.xml");
            StorageFile sampleFile = await localFolder.CreateFileAsync("events.xml", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n <events>\n </events>\n");
        }

        private void Show_All_Click(object sender, RoutedEventArgs e)
        {
            this.LoadEvents();
        }

        private void eventsTable_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.SelectedEvent = (Event)eventsTable.SelectedItem;
        }

    }
}
