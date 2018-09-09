using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eda.bot.eda; 

namespace eda.console
{
    class Program
    {

        static void Main(string[] args)
        { 
          
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");
            //Console.ReadKey();

            var r = EdaClient.GetAtoDataTypesAsync("ato").Result;

            foreach (var item in r)
            {
                Console.WriteLine($"{item.ColName}, {item.ColType} "); 
               
            }


            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
