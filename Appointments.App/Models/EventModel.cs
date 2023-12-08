﻿using System;
using System.Drawing;

namespace Appointments.App.Models
{
    public class EventModel
    {
        public string AppointmentType { get; set; }
        public string UserInformation { get; set; }
        public string UserPhone { get; set; }
        public DateTime EventDate { get; set; }
        public Color AppointmentColor { get; set; }
        public string Time 
        {   get 
            {
                return EventDate.ToString("HH:mm");
            } 
        }
    }
}
