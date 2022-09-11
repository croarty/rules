namespace test.daap.rules;

public class CustomerForRuleBuilder
{
    public CustomerForRuleBuilder(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}