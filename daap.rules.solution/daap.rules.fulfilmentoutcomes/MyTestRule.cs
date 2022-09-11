using NRules.Fluent.Dsl;

namespace daap.rules.fulfilmentoutcomes;

public class MyTestRule : Rule
{
    public override void Define()
    {
        IDoSomething something = null;
        Context context = null;

        Dependency().Resolve(() => something);

        When().Match<Context>(() => context);

        Then().Do(_ => something.Notify());
    }
}

public class Context
{
}