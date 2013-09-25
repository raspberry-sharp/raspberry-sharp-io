using System;

namespace Raspberry.IO.Components
{
    public interface IOutputPin : IDisposable
    {
        void Write(bool state);
    }
}