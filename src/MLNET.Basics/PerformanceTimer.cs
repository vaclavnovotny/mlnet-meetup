using System;
using System.Diagnostics;

namespace MLNET.Core
{
    public class PerformanceTimer : IDisposable
    {
        private readonly string _message;
        private readonly bool _showTicks;
        private readonly Stopwatch _timer;

        public PerformanceTimer(string message, bool showTicks = false)
        {
            _message = message;
            _showTicks = showTicks;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            Console.WriteLine($"{_message} took {_timer.ElapsedMilliseconds}ms{(_showTicks?$" ({_timer.ElapsedTicks} ticks)":string.Empty)}.");
        }
    }
}
