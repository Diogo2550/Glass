using Glass.Core.Enums;
using Glass.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models {
    class EventualSchedule : BaseSchedule {

        public DateTime EventualDate { get; private set; }
        public EventualState EventualState { get; private set; }

        public void SetEventualDate(DateTime eventualDate) {
            EventualDate = eventualDate;
        }

        public void SetEventualState(EventualState state) {
            EventualState = state;
        }

        public void SetEventualState(ushort state) {
            EventualState = (EventualState)state;
        }

        public void SetEventualState(string state) {
            EventualState = (EventualState)Enum.Parse(EventualState.GetType(), state);
        }

    }
}
