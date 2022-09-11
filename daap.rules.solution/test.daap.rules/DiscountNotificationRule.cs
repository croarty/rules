using NRules.Fluent.Dsl;

namespace test.daap.rules;

public class DiscountNotificationRule : Rule
{
    public override void Define()
    {
        Customer customer = null;

        When()
            .Match<Customer>(() => customer)
            .Exists<Order>(o => o.Customer == customer, o => o.PercentDiscount > 0.0);

        Then()
            .Do(_ => customer.NotifyAboutDiscount());
    }
}