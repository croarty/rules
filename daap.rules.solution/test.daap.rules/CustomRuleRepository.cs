using System.Diagnostics;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace test.daap.rules;

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