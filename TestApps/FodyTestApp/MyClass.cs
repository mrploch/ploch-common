// unset

using Ploch.TestApps.WorkerServiceApp;
using System;

namespace FodyTestApp
{
    public class MyClass
    {
        [Interceptor]
        public void DoSomething(string str, int i)
        {
            Console.WriteLine("Doing something....");
        }
    }
}