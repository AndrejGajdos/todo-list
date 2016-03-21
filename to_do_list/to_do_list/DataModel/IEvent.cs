using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List_2.DataModel
{
    interface IEvent
    {
        /// <summary>
        /// Event identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Event description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Event status
        /// </summary>
        EventStatus Status { get; set; }

        /// <summary>
        /// Event priority
        /// </summary>
        EventPriority Priority { get; set; }

        /// <summary>
        /// Estimated length od event
        /// </summary>
        TimeSpan EstimatedLength { get; set; }

        /// <summary>
        /// Possible start date of event
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Event deadline
        /// </summary>
        DateTime EndDate { get; set; }
    }
}
