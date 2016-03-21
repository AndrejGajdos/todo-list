/*
 ============================================================================
 Name        : EventManager.cs
 Author      : Andrej Gajdos
 Version     : 27.5.2013
 Copyright   : 359234@mail.muni.cz
 Description : EventManager class
 ============================================================================
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;

namespace To_Do_List_2
{
    class EventManager : IEventManager
    {
        private string filename = "events.xml";
        
        private List<Event> events;
        public List<Event> Events
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

        public EventManager()
        {
            this.Events = new List<Event>();
        }

        public async void CreateEvent(Event newEvent)
        {
            this.Events.Add(newEvent);

            if (newEvent == null)
            {
                throw new ArgumentNullException("newEvent");
            }

            try
            {
                string result = "";
                Windows.Storage.StorageFolder localFolder = null;
                try
                {
                    localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile file = await localFolder.GetFileAsync(filename);
                    result = await FileIO.ReadTextAsync(file);
                }
                catch (Exception)
                {
                    // File not found
                }

                XDocument doc = XDocument.Parse(result);

                XElement nodeFromEvent = new XElement("event",

                    new XElement("name", newEvent.Name),
                    new XElement("description", newEvent.Description),
                    new XElement("status", newEvent.Status.ToString()),
                    new XElement("priority", newEvent.Priority.ToString()),
                    new XElement("length", newEvent.EstimatedLength.ToString()),
                    new XElement("start-date", newEvent.StartDate.ToString("dd.M.yyyy H:m:s")),
                    new XElement("end-date", newEvent.EndDate.ToString("dd.M.yyyy H:m:s"))

                    );

                nodeFromEvent.SetAttributeValue("id", Guid.NewGuid().ToString("N"));

                doc.Element("events").Add(nodeFromEvent);
                StorageFile sampleFile = await localFolder.CreateFileAsync("events.xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + doc.ToString());

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        public async void UpdateEvent(Event updatedEvent)
        {   

            if (updatedEvent == null)
            {
                throw new ArgumentNullException("updatedEvent");
            }

            string result = "";
            Windows.Storage.StorageFolder localFolder = null;
            try
            {
                localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.GetFileAsync(filename);
                result = await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                // File not found
            }

            foreach (Event eve in this.Events)
            {
                if (eve.Id == updatedEvent.Id)
                {
                    eve.Id = updatedEvent.Id;
                    eve.Name = updatedEvent.Name;
                    eve.Description = updatedEvent.Description;
                    eve.EstimatedLength = updatedEvent.EstimatedLength;
                    eve.Priority = updatedEvent.Priority;
                    eve.Status = updatedEvent.Status;
                    eve.StartDate = updatedEvent.StartDate;
                    eve.EndDate = updatedEvent.EndDate;
                    break;
                }
            }

            XDocument doc = XDocument.Parse(result);
            
            var removeNode = (from node in doc.Descendants("event") where node.Attribute("id").Value == updatedEvent.Id select node).FirstOrDefault();

            XElement nodeFromEvent = new XElement("event",

                    new XElement("name", updatedEvent.Name),
                    new XElement("description", updatedEvent.Description),
                    new XElement("status", updatedEvent.Status.ToString()),
                    new XElement("priority", updatedEvent.Priority.ToString()),
                    new XElement("length", updatedEvent.EstimatedLength.ToString()),
                    new XElement("start-date", updatedEvent.StartDate.ToString("dd.M.yyyy H:m:s")),
                    new XElement("end-date", updatedEvent.EndDate.ToString("dd.M.yyyy H:m:s"))

                    );

            nodeFromEvent.SetAttributeValue("id", Guid.NewGuid().ToString("N"));
            
            removeNode.ReplaceWith(nodeFromEvent);

            StorageFile sampleFile = await localFolder.CreateFileAsync("events.xml", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + doc.ToString());

        }

        public async void DeleteEvent(Event deletedEvent)
        {
            if (deletedEvent == null)
            {
                throw new ArgumentNullException("deletedEvent");
            }

            string result = "";
            Windows.Storage.StorageFolder localFolder = null;
            try
            {
                localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.GetFileAsync(filename);
                result = await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                // File not found
            }

            XDocument doc = XDocument.Parse(result);

            int numberofNodes = doc.Descendants("event").Count();
            var removeNode = (from node in doc.Descendants("event") where node.Attribute("id").Value == deletedEvent.Id select node).FirstOrDefault();

            removeNode.Remove();

            StorageFile sampleFile = await localFolder.CreateFileAsync("events.xml", CreationCollisionOption.ReplaceExisting);

            if (numberofNodes == 1)
                await FileIO.WriteTextAsync(sampleFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n <events>\n </events>\n");
            else
                await FileIO.WriteTextAsync(sampleFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + doc.ToString());

            this.Events.Remove(deletedEvent);

        }

        /*
        public Event GetEvent(string Id)
        {
            return this.Events.FirstOrDefault(eve => eve.Id == Id);
        }
         */

        public List<Event> GetAllEvents(string fileContent)
        {
            if (fileContent == null)
            {
                throw new ArgumentNullException("fileContent");
            }

            XDocument document = XDocument.Parse(fileContent);

            foreach (var xElem in document.Descendants("event"))
            {
                string eventPriority = xElem.Element("priority").Value;
                string eventStatus = xElem.Element("status").Value;

                EventPriority eventPrioritySt = new EventPriority();
                EventStatus eventStatusSt = new EventStatus();

                // http://blogs.msdn.com/b/tims/archive/2004/04/02/106310.aspx
                if (Enum.IsDefined(typeof(EventPriority), eventPriority) && Enum.IsDefined(typeof(EventStatus), eventStatus))
                {
                    eventPrioritySt = (EventPriority)Enum.Parse(typeof(EventPriority), eventPriority, true);
                    eventStatusSt = (EventStatus)Enum.Parse(typeof(EventStatus), eventStatus, true);
                }
                else
                {
                    //   MessageBox.Show("Damn");

                }

                this.Events.Add(new Event()
                {

                    Id = xElem.Attribute("id").Value,
                    Name = xElem.Element("name").Value,
                    Description = xElem.Element("description").Value,
                    EstimatedLength = TimeSpan.Parse(xElem.Element("length").Value),
                    StartDate = DateTime.ParseExact(xElem.Element("start-date").Value, "dd.M.yyyy H:m:s", CultureInfo.InvariantCulture),
                    EndDate = DateTime.ParseExact(xElem.Element("end-date").Value, "dd.M.yyyy H:m:s", CultureInfo.InvariantCulture),
                    Priority = eventPrioritySt,
                    Status = eventStatusSt

                });

            }

            return this.Events;

        }

        public List<Event> SearchEvents(string exp)
        {
            return this.Events.Where(eve => (eve.Name.ToLower().Contains(exp.ToLower()) || eve.Description.ToLower().Contains(exp.ToLower()))).ToList();
        }


        public List<Event> GetEventsInDay(DateTime day)
        {
            if (day == null)
            {
                throw new ArgumentNullException("day");
            }

            List<Event> eventsInDay = new List<Event>();
            Event parsedEvent = new Event();
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            foreach (Event eve in this.Events)
            {
                int givenDateWeek = cal.GetWeekOfYear(day, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int startDateWeek = cal.GetWeekOfYear(eve.StartDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int endDateWeek = cal.GetWeekOfYear(eve.EndDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                if (((givenDateWeek == startDateWeek) && (day.Day >= eve.StartDate.Day)) || ((givenDateWeek == endDateWeek) && (day.Day <= eve.EndDate.Day)))
                    eventsInDay.Add(eve);
                else
                    continue;

            }

            return eventsInDay;
        }

        public List<Event> GetEventsInWeek(DateTime week)
        {
            if (week == null)
            {
                throw new ArgumentNullException("week");
            }

            List<Event> eventsInDay = new List<Event>();
            Event parsedEvent = new Event();
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            foreach (Event eve in this.Events)
            {
                int givenDateWeek = cal.GetWeekOfYear(week, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int startDateWeek = cal.GetWeekOfYear(eve.StartDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int endDateWeek = cal.GetWeekOfYear(eve.EndDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                if ((givenDateWeek == startDateWeek) || (givenDateWeek == endDateWeek))
                    eventsInDay.Add(eve);
                else
                    continue;

            }

            return eventsInDay;
        }

        public List<Event> GetEventsInMonth(DateTime month)
        {
            if (month == null)
            {
                throw new ArgumentNullException("month");
            }

            List<Event> eventsInDay = new List<Event>();
            Event parsedEvent = new Event();
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            foreach (Event eve in this.Events)
            {

                if ((cal.GetMonth(month) == cal.GetMonth(eve.StartDate)) || (cal.GetMonth(month) == cal.GetMonth(eve.EndDate)))
                    eventsInDay.Add(eve);
                else
                    continue;
            }

            return eventsInDay;
        }


    }
    
}
