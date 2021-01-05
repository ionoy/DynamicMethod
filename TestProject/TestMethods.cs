using System;

namespace TestProject
{
    public class TestMethods
    {
        public void SimpleMethod()
        {
            Console.WriteLine("Hello, world!");
        }

        public void TryCatch()
        {
            try {
                Console.WriteLine("try");
                throw new Exception("Hello from try block");
            }
            catch (Exception e) {
                Console.WriteLine("catch " + e);
            }
        }
    }
}