using System.Diagnostics;

namespace daap.rules.fulfilmentoutcomes;

public class Notifier : IDoSomething
{
    public void Notify()
    {
        Trace.WriteLine("Injected Service");
    }
}