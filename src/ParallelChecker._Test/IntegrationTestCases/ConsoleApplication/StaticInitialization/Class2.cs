namespace StaticInitialization
{
    class Class2
    {
        static object sync = new object();

        static Class1 y = new Class1();

        public Class2()
        {
            lock(sync) { }
        }
    }
}
