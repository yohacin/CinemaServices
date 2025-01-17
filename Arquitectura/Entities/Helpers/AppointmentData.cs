using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Helpers
{
    public class AppointmentData
    {
        public string Subject { get; set; }
        public long IdEngagement { get; set; }
        public long IdDetalleEngagementEmpleado { get; set; }
        public string Engagement { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double HorasAsignadas { get; set; }
        public bool IsReadonly { get; set; }
        public bool FullDay { get; set; }
        public string PrimaryColor { get; set; }
        public bool CurrentEngagement { get; set; }
    }
}
