using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RxDemo
{
    public class Market
    {
        public void AddPrice(float price)
        {
            Prices.Add(price);
        }
        public BindingList<float> Prices = new BindingList<float>();
    }



    public class PriceAddedEventArgs
    {
        public float Price;
    }

    public static class Demo1
    {
        public static void Main(string[] args)
        {
            Market market = new Market();
            //      market.PriceAdded += (sender, eventArgs) =>
            //      {
            //        Console.WriteLine($"Added price {eventArgs.Price}");
            //      };
            //      market.AddPrice(123);
            market.Prices.ListChanged += (sender, eventArgs) => // Subscribe
            {
                if (eventArgs.ListChangedType == ListChangedType.ItemAdded)
                {
                    Console.WriteLine($"Added price {((BindingList<float>)sender)[eventArgs.NewIndex]}");
                }
            };
            market.AddPrice(123);
            market.AddPrice(45);
            market.AddPrice(67);
            market.AddPrice(89);
            // 1) How do we know when a new item becomes available?

            // 2) how do we know when the market is done supplying items?
            // maybe you are trading a futures contract that expired and there will be no more prices

            // 3) What happens if the market feed is broken?
            Console.ReadLine();

        }
    }
}


