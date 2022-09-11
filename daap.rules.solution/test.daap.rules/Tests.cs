using System.Diagnostics;
using System.Linq.Expressions;
using NRules;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace test.daap.rules;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var repository = new CustomRuleRepository();
        repository.LoadRules();
        
        RulesUnitTestFromThePast.SerializeRuleRepository(repository);
        
        ISessionFactory factory = repository.Compile();

        ISession session = factory.CreateSession();
        var customer = new CustomerForRuleBuilder("John Do");
        session.Insert(customer);
        session.Insert(new OrderForRuleBuilder(customer, 90.00m));
        session.Insert(new OrderForRuleBuilder(customer, 110.00m));

        session.Fire();
    }
}

public class CustomRuleRepository : IRuleRepository
{
    private readonly IRuleSet _ruleSet = new RuleSet("MyRuleSet");

    public IEnumerable<IRuleSet> GetRuleSets()
    {
        return new[] {_ruleSet};
    }

    public void LoadRules()
    {
        //Assuming there is only one rule in this example
        var rule = BuildRule();
        _ruleSet.Add(new []{rule});
    }

    private IRuleDefinition BuildRule()
    {
        //Create rule builder
        var builder = new RuleBuilder();
        builder.Name("TestRule");

        //Build conditions
        PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof (CustomerForRuleBuilder), "customer");
        Expression<Func<CustomerForRuleBuilder, bool>> customerCondition = 
            customer => customer.Name == "John Do";
        customerPattern.Condition(customerCondition);

        PatternBuilder orderPattern = builder.LeftHandSide().Pattern(typeof (OrderForRuleBuilder), "order");
        Expression<Func<OrderForRuleBuilder, CustomerForRuleBuilder, bool>> orderCondition1 = 
            (order, customer) => order.Customer == customer;
        Expression<Func<OrderForRuleBuilder, bool>> orderCondition2 = 
            order => order.Amount > 100.00m;
        orderPattern.Condition(orderCondition1);
        orderPattern.Condition(orderCondition2);

        //Build actions
        Expression<Action<IContext, CustomerForRuleBuilder, OrderForRuleBuilder>> action = 
            (ctx, customer, order) => Trace.WriteLine($"Customer {customer.Name} has an order in amount of {order.Amount}");
        builder.RightHandSide().Action(action);

        //Build rule model
        return builder.Build();
    }
}

public class CustomerForRuleBuilder
{
    public CustomerForRuleBuilder(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}

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