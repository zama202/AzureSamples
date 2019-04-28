using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace RxDemo
{

    public class Demo2 : IObserver<float>
    {
        public Demo2()
        {
            var oo = new OperationObserver();

            // subject is both an observer and observable

            // vvvv go to base symbols
            var simulatedMarket = new Subject<float>();

            // note also the overloads of this!
            simulatedMarket.Subscribe(this);
            //simulatedMarket.Subscribe(this); // twice!

            simulatedMarket.OnNext(123);
            simulatedMarket.Subscribe(oo); // late subscription
            simulatedMarket.OnNext(456);
            simulatedMarket.OnCompleted();

            simulatedMarket.OnNext(321); // this will not work
            Console.WriteLine(simulatedMarket.HasObservers); // nope
            Console.ReadLine();
        }

        public static void Main(string[] args)
        {
            IObserver<float> o; // no need for nuget

            // OnNext* -> (OnError | OnCompleted)

            Demo2 demo2 = new Demo2();
        }

        public void OnNext(float value)
        {
            Console.WriteLine($"Market sent us {value}");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"We failed due to {error}");
        }

        public void OnCompleted()
        {
            Console.WriteLine($"Market is closed");
        }
    }

    public class OperationObserver : IObserver<float>
    {
        public void OnNext(float value)
        {
            Console.WriteLine($"Operations normal, value = {value}");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Operations interrupted");
        }

        public void OnCompleted()
        {
            Console.WriteLine("Operations complete");
        }
    }
}

