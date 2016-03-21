/*
 ============================================================================
 Name        : Event.cs
 Author      : Andrej Gajdos
 Version     : 27.5.2013
 Copyright   : 359234@mail.muni.cz
 Description : Event class
 ============================================================================
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do_List_2.DataModel;

namespace To_Do_List_2
{
    public class Event : IEvent, INotifyPropertyChanged
    {
        private string id;
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                this.id = value;
                this.NotifyPropertyChanged("Id");
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                this.description = value;
                this.NotifyPropertyChanged("Description");
            }
        }

        private EventStatus status;
        public EventStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                this.status = value;
                this.NotifyPropertyChanged("Status");
            }
        }

        private EventPriority priority;
        public EventPriority Priority
        {
            get
            {
                return priority;
            }

            set
            {
                this.priority = value;
                this.NotifyPropertyChanged("Priority");
            }
        }

        private TimeSpan estimatedLength;
        public TimeSpan EstimatedLength
        {
            get
            {
                return estimatedLength;
            }

            set
            {
                this.estimatedLength = value;
                this.NotifyPropertyChanged("EstimatedLength");
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }

            set
            {
                this.startDate = value;
                this.NotifyPropertyChanged("StartDate");
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                this.endDate = value;
                this.NotifyPropertyChanged("EndDate");
            }
        }

        /// <summary>
        /// Override default functionality so that it is based on Id
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if obj equals to this, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is Event && (obj as Event).Id == Id;
        }

        /// <summary>
        /// </summary>
        /// <returns>Hash code based on Event Id.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Override default ToString funcionality
        /// </summary>
        /// <returns>A string that represents current object</returns>
        public override string ToString()
        {
            return "Event ID: " + this.Id + ", name: " + this.Name + ", description: " + this.Description + ", status: " + this.Status + ", priority: "
                + this.Priority + ", length: " + this.EstimatedLength + ", startDate: " + this.StartDate + ", endDate " + this.EndDate;
        }

        /// <summary>
        /// Method for PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}
