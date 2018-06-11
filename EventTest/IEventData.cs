using System;
using System.Collections.Generic;
using System.Text;

namespace EventTest
{
    public interface IEventData
    {
        DateTime EventTime { get; set; }

        object EventSource { get; set; }
    }
}
