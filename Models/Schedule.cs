using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models.Abstracts {
    class Schedule : BaseSchedule {

        public DayOfWeek DayOfWeek { get; private set; }

        public void SetDayOfWeek(DayOfWeek day) {
            DayOfWeek = day;
        }

        public void SetDayOfWeek(ushort day) {
            DayOfWeek = (DayOfWeek)day;
        }

    }
}
