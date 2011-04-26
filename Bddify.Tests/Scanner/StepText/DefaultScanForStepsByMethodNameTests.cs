using Bddify.Core;
using Bddify.Scanners;
using NUnit.Framework;
using System.Linq;

namespace Bddify.Tests.Scanner.StepText
{
    public class DefaultScanForStepsByMethodNameTests
    {
        class ScenarioWithVaryingStepTexts
        {
            public void GivenThePascalCaseForMethodName() { }
            public void When_Step_Name_Uses_Underscore_With_Pascal_Case() { }
            public void Then_with_lower_case_underscored_method_name() { }
            
            [RunStepWithArgs(1, 2, 3)]
            [RunStepWithArgs(3, 4, 5)]
            public void WhenStepIsRunWithArgumentsWithoutProvidedText(int input1, int input2, int input3) { }

            [RunStepWithArgs(1, 2, 3, StepTextTemplate = "The step text gets argument {0}, {1} and then {2}")]
            [RunStepWithArgs(3, 4, 5, StepTextTemplate = "The step text gets argument {0}, {1} and then {2}")]
            public void WhenStepIsRunWithArgumentsWithProvidedText(int input1, int input2, int input3) { }

            public void WhenStepNameEndsWithNumber29()
            {
            }
        }

        static void VerifyMethod(string expectedReadableMethodName, bool exists = true)
        {
            var scanner = new DefaultScanForStepsByMethodName();
            var steps = scanner.Scan(typeof(ScenarioWithVaryingStepTexts)).ToList();
            var theStep = steps.Where(s => s.ReadableMethodName == expectedReadableMethodName);
            
            if(exists)
                Assert.That(theStep.Count(), Is.EqualTo(1));
            else
                Assert.That(theStep.Count(), Is.EqualTo(0));
        }

        [Test]
        public void TheMethodWithPascalCaseIsSeparatedAndTurnedIntoLowerCaseExceptTheFirstWord()
        {
            VerifyMethod("Given the pascal case for method name");
        }

        [Test]
        public void TheMethodWithUnderscoreAndPascalCaseIsSeparatedButCaseIsRetained()
        {
            VerifyMethod("When Step Name Uses Underscore With Pascal Case");
        }

        [Test]
        public void TheMethodWithUnderscoreAndLowerCaseWordsIsSeparatedAndCaseIsRetained()
        {
            VerifyMethod("Then with lower case underscored method name");
        }

        [Test]
        public void TrailingNumberGetsTheSameTreatmentAsWords()
        {
            VerifyMethod("When step name ends with number 29");
        }

        [Test]
        public void TheMethodWithArgumentWithoutProvidedTextGetsArgumentsAppendedToTheMethodName()
        {
            VerifyMethod("When step is run with arguments without provided text 1, 2, 3");
            VerifyMethod("When step is run with arguments without provided text 3, 4, 5");
        }

        [Test]
        public void TheMethodWithArgumentWithProvidedTextUsesTheProvidedTextAsTemplate()
        {
            VerifyMethod("The step text gets argument 1, 2 and then 3");
            VerifyMethod("The step text gets argument 3, 4 and then 5");
        }

        [Test]
        public void TheMethodWithArgumentWithProvidedTextDoesNotUseTheMethodName()
        {
            VerifyMethod("When step is run with arguments with provided text 1, 2, 3", false);
            VerifyMethod("When step is run with arguments with provided text 3, 4, 5", false);
        }
    }
}