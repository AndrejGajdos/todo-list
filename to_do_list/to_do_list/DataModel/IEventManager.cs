/*
 ============================================================================
 Name        : IEventManager.cs
 Author      : Andrej Gajdos
 Version     : 27.5.2013
 Copyright   : 359234@mail.muni.cz
 Description : IEventManager interface
 ============================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List_2
{
    interface IEventManager
    {
        /// <summary>
        /// Adds event to database and to the end of List of events
        /// </summary>
        /// <param name="newEvent">The event to be added to database and List</param>
        void CreateEvent(Event newEvent);
        
        /// <summary>
        /// Update event in database and in the List of events
        /// </summary>
        /// <param name="updatedEvent">The event to be updated in database and List</param>
        void UpdateEvent(Event updatedEvent);

        /// <summary>
        /// Delete event in database and in the List of events
        /// </summary>
        /// <param name="deletedEvent">The event to be removed from database and List</param>
        void DeleteEvent(Event deletedEvent);
        
        //void GetEvent(string Id);

        /// <summary>
        /// Get all events from database
        /// </summary>
        /// <param name="filename">string containg data in database</param>
        /// <returns>List of all events from database</returns>
        List<Event> GetAllEvents(string fileContent);

        /// <summary>
        /// Search for all events in which every event name or description contains parameter 
        /// </summary>
        /// <param name="exp">string which we want to find in event name or description</param>
        /// <returns>List of events</returns>
        List<Event> SearchEvents(string exp);

        /// <summary>
        /// Get all events in which every event StartDate or EndDate dates equals given parameter
        /// </summary>
        /// <param name="day">Datetime day</param>
        /// <returns>List of events</returns>
        List<Event> GetEventsInDay(DateTime day);

        /// <summary>
        /// Get all events in which every event StartDate or EndDate number of week equals given parameter
        /// </summary>
        /// <param name="week">DaTeTime week</param>
        /// <returns>List of events</returns>
        List<Event> GetEventsInWeek(DateTime week);

        /// <summary>
        /// Get all events in which every event StartDate or EndDate month equals given parameter
        /// </summary>
        /// <param name="month">DaTime month</param>
        /// <returns>List of events</returns>
        List<Event> GetEventsInMonth(DateTime month);
    }
}
