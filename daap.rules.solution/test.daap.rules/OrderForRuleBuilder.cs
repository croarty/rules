namespace test.daap.rules;

public class OrderForRuleBuilder
{
    public OrderForRuleBuilder(CustomerForRuleBuilder customer, decimal amount)
    {
        Customer = customer;
        Amount = amount;
    }

    public CustomerForRuleBuilder Customer { get; private set; }
    public decimal Amount { get; private set; }
}