using Autofac;
using daap.rules.fulfilmentoutcomes;
using NRules;
using NRules.Fluent;
using NRules.Integration.Autofac;
using NRules.RuleModel;

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

    [Test]
    public void Test2()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<Notifier>().As<IDoSomething>();
        // var types = builder.RegisterRules(x => x.AssemblyOf(typeof(MyTestRule)));
        builder.RegisterRuleRepository(rrr =>
        {
            var type = typeof(MyTestRule);
            rrr.AssemblyOf(type);
        });
        builder.RegisterSessionFactory();
        builder.RegisterSession();
        var container = builder.Build();

        var ruleRepository = container.Resolve<IRuleRepository>();
        var sessionFactory = ruleRepository.Compile();
        
        ISession session = sessionFactory.CreateSession();
        session.DependencyResolver = new AutofacDependencyResolver(container);
        
        Context d = new Context();
        session.Insert(d);

        session.Fire();
    }
    
}