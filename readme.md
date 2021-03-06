BDDfy is the simplest BDD framework to use, customize and extend! The framework is explained on Mehdi Khalili's blog series in full details [here] (http://www.mehdi-khalili.com/bddify-in-action/introduction). This is a very short tutorial and quickstart.

To use BDDfy:

 - Install NuGet if you have not already.
 - Go to 'Tools', 'Library Package Manager', and click 'Package Manager Console'.
 - In the console, type 'Install-Package TestStack.BDDfy' and enter.

This adds BDDfy assembly and its dependencies to your test project. If this is the first time you are using BDDfy you may want to check out the samples on NuGet. Just run Install-Package TestStack.BDDfy.Samples and it will load two fully working samples to your project.

A few quick facts about BDDfy:
 - It can run with any testing framework. Actually you don't have to use a testing framework at all. You can just apply it on your POCO (test) classes!
 - It does not need a separate test runner. You can use your runner of choice. For example, you can write your BDDfy tests using NUnit and run them using NUnit console or GUI runner, Resharper or TD.Net and regardless of the runner, you will get the same result.
 - It can run standalone scenarios. In other words, although BDDfy supports stories, you do not necessarily have to have or make up a story to use it. This is useful for developers who work in non-Agile environments but would like to get some decent testing experience.
 - You can use underscored or pascal or camel cased method names for your steps.
 - You do not have to explain your scenarios or stories or steps in string, but you can if you need full control over what gets printed into console and HTML reports.
 - BDDfy is very extensible: it's core barely has any logic in it and it delegates all it's responsibilities to it's extensions all of which are configurable; e.g. if you don't like the reports it generates, you can write your custom reporter in a few lines of code.

##Quick start
Now that you have installed BDDfy, write your first test (this test is borrowed from ATM sample that you can install using nuget package TestStack.BDDfy.Samples):

	[Story(
    AsA = "As an Account Holder",
    IWant = "I want to withdraw cash from an ATM",
    SoThat = "So that I can get money when the bank is closed")]
	public class AccountHasInsufficientFund
	{
	    private Card _card;
	    private Atm _atm;
	
	    // You can override step text using executable attributes
	    [Given(StepText = "Given the account balance is $10")]
	    void GivenAccountHasEnoughBalance()
	    {
	        _card = new Card(true, 10);
	    }
	
	    void AndGivenTheCardIsValid()
	    {
	    }
	
	    void AndGivenTheMachineContainsEnoughMoney()
	    {
	        _atm = new Atm(100);
	    }
	
	    void WhenTheAccountHolderRequests20()
	    {
	        _atm.RequestMoney(_card, 20);
	    }
	
	    void ThenTheAtmShouldNotDispenseAnyMoney()
	    {
	        Assert.AreEqual(0, _atm.DispenseValue);
	    }
	
	    void AndTheAtmShouldSayThereAreInsufficientFunds()
	    {
	        Assert.AreEqual(DisplayMessage.InsufficientFunds, _atm.Message);
	    }
	
	    void AndTheCardShouldBeReturned()
	    {
	        Assert.IsFalse(_atm.CardIsRetained);
	    }
	
	    [Test]
	    public void Execute()
	    {
	        this.BDDfy();
	    }
	}


And this gives you a report like:

	Story: Account holder withdraws cash
    	As an Account Holder
    	I want to withdraw cash from an ATM
    	So that I can get money when the bank is closed

	Scenario: Account has insufficient fund
    	Given the account balance is $10
      		And the card is valid
    	When the account holder requests $20
    	Then the atm should not dispense any money
      		And the atm should say there are insufficient funds
      		And the card should be returned

This is just the console report. Have a look at your output folder and you should see a nice html report too.

If you want more control you can also use BDDfy's Fluent API. Here is another example done using the Fluent API:


	[Test]
	public void CardHasBeenDisabled()
	{
	    this.Given(s => s.GivenTheCardIsDisabled())
	        .When(s => s.WhenTheAccountHolderRequests(20))
	        .Then(s => s.CardIsRetained(true), "Then the ATM should retain the card")
	            .And(s => s.AndTheAtmShouldSayTheCardHasBeenRetained())
	        .BDDfy(htmlReportName: "ATM");
	}

which gives you a report like:

	Scenario: Card has been disabled
    	Given the card is disabled
    	When the account holder requests 20
    	Then the ATM should retain the card
      		And the atm should say the card has been retained


##How does BDDfy work?
BDDfy uses quite a few conventions to make it frictionless to use. For your convenience, I will try to provide a quick tutorial below:

###Finding steps
BDDfy scans your BDDfyed classes for steps. Currently it has three ways of finding a step: 

* Using attributes 
* Using method name conventions 
* And using fluent API.

BDDfy runs your steps in the following order: SetupState, ConsecutiveSetupState, Transition, ConsecutiveTransition, Assertion, ConsecutiveAssertion and TearDown. Some of these steps are reported in the console and html reports and some of them are not. Please read below for further information.

###Attributes
BDDfy looks for a custom attribute called ExecutableAttribute on your method. To make it easier to use, ExecutableAttribute has a few subclasses that you can use: you may apply Given, AndGiven, When, AndWhen, Then, and AndThen attributes on any method you want to make available to BDDfy.

###Method name convention
BDDfy uses reflection to scan your classes for steps. In this mode, known as reflective mode, it has two ways of finding a step: using attributes and method name conventions. The following is the list of method name conventions:

* Method name ending with `Context` is considered a setup method but doesn't get shown in the reports 
* Method name equaling `Setup` is a setup method but doesn't get showin in the reports
* Method name starting with `Given` is a setup step that gets reported in the reports
* Method name starting with `AndGiven` and 'And_given_' are considered setup steps running after 'Given' steps which is reported.
* Method name starting with `When` is considered a state transition step and is reported
* Method name starting with `AndWhen` and `And_when_` are considered state transition steps running after 'When' steps and is reported
* Method name starting with `Then` is an asserting step and is reported
* Method name starting with `And` and `AndThen` and `And_then_` are considered an asserting steps running after 'Then' step and is reported
* Method name starting with `TearDown` is considered as tear down method that is always run at the end but doesn't get shown in the reports.

As you see in the above example you can mix and match the executable attributes and method name conventions to acheive great flexibility and power.

###Fluent API
Fluent API gives you the absolute power over step selection and their titles. When you use Fluent API for a test, the attributes and method name conventions are ignored for that test. 

Please note that you can have some tests using fluent API and some using a combination of attributes and method name conventions. Each .BDDfy() test works in isolation of others.

###Other conventions
BDDfy prefers convention over configuration; but it also allows you to configure pretty much all the conventions. 