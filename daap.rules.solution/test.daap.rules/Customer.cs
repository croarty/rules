using System.Diagnostics;

namespace test.daap.rules;

public class Customer
{
    public string Name { get; private set; }
    public bool IsPreferred { get; set; }

    public Customer(string name)
    {
        Name = name;
    }

    public void NotifyAboutDiscount()
    {
        Debug.WriteLine("Customer {0} was notified about a discount", Name);
    }
}