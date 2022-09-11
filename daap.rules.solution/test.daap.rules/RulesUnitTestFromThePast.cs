using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using NRules;
using NRules.Fluent;
using NRules.Json;
using NRules.RuleModel;

namespace test.daap.rules;

public class RulesUnitTestFromThePast
{
    [Test]
    public void TestMethod1()
    {
        //Load rules
        var repository = new RuleRepository();
        repository.Load(x => x.From(Assembly.GetExecutingAssembly()));

//Compile rules
        var factory = repository.Compile();

//Create a working session
        var session = factory.CreateSession();
            
//Load domain model
        var customer = new Customer("John Doe") {IsPreferred = true};
        var order1 = new Order(123456, customer, 2, 25.0);
        var order2 = new Order(123457, customer, 1, 100.0);

//Insert facts into rules engine's memory
        session.Insert(customer);
        session.Insert(order1);
        session.Insert(order2);

//Start match/resolve/act cycle
        session.Fire();
    }

    [Test]
    public void Serializer()
    {
        SerializeRuleRepository(GetRuleRepository());
    }

    public static void SerializeRuleRepository(IRuleRepository repository)
    {
        var _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        RuleSerializer.Setup(_options);

        var ruleSets = repository.GetRuleSets();

        foreach (IRuleSet ruleSet in ruleSets)
        {
            foreach (var ruleDefinition in ruleSet.Rules)
            {
                if (ruleDefinition != null)
                {
                    if (_options != null)
                    {
                        var jsonString = JsonSerializer.Serialize(ruleDefinition, _options);
                        Trace.WriteLine(jsonString);
                    }
                }
            }
        }
    }

    private static RuleRepository GetRuleRepository()
    {
        var repository = new RuleRepository();
        repository.Load(x => x.From(Assembly.GetExecutingAssembly()));
        return repository;
    }
}