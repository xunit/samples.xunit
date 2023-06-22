using System;

namespace XunitExtensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ObservationAttribute : Attribute
    {
        public int Order { get; set; } = 0;
    }
}
