using System;

namespace Bddify.Tests.Exceptions.OtherExceptions
{
    public class OtherExceptionBase
    {
        protected readonly ExceptionThrowingTest<Exception> Sut;

        public OtherExceptionBase()
        {
            Sut = new ExceptionThrowingTest<Exception>();
        }
    }
}