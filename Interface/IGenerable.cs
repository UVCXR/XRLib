using System;

namespace WIFramework
{
    public interface IGenerable
    {
        public string id { get; }

        public bool GetValue<T>(ref T result);
    }
}