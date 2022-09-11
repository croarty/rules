using NRules.Fluent.Dsl;

namespace test.daap.rules;

public class PreferredCustomerDiscountRule : Rule
{
    public override void Define()
    {
        Customer customer = null;
        IEnumerable<Order> orders = null;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => o.IsOpen,
                    o => !o.IsDiscounted)
                .Collect()
                .Where(c => c.Any()));

        Then()
            .Do(ctx => ApplyDiscount(orders, 10.0))
            .Do(ctx => ctx.UpdateAll(orders));
    }

    private static void ApplyDiscount(IEnumerable<Order> orders, double discount)
    {
        foreach (var order in orders)
        {
            order.ApplyDiscount(discount);
        }
    }
}