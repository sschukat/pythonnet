namespace Python.Test
{
    /// <summary>
    /// Supports CLR class unit tests.
    /// </summary>
    public interface IPublicInterface
    {
    }

    internal interface IInternalInterface
    {
    }

    public interface ISayHello1
    {
        string SayHello();
    }

    public interface ISayHello2
    {
        string SayHello();
    }

    public class InterfaceTest : ISayHello1, ISayHello2
    {
        public InterfaceTest()
        {
        }

        public string HelloProperty
        {
            get { return "hello"; }
        }

        string ISayHello1.SayHello()
        {
            return "hello 1";
        }

        string ISayHello2.SayHello()
        {
            return "hello 2";
        }

        public ISayHello1 GetISayHello1()
        {
            return this;
        }

        public void GetISayHello2(out ISayHello2 hello2)
        {
            hello2 = this;
        }

        public ISayHello1 GetNoSayHello(out ISayHello2 hello2)
        {
            hello2 = null;
            return null;
        }

        public ISayHello1 [] GetISayHello1Array()
        {
            return new[] { this };
        }

        public interface IPublic
        {
        }

        protected interface IProtected
        {
        }

        internal interface IInternal
        {
        }

        private interface IPrivate
        {
        }
    }
}
