using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Task<string> readTasks = new Task<string>(ReadFromFileAsync);
			Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
			Task<string>  l = Test(readTasks);
			l.Wait();
		}
		static string ReadFromFileAsync()
		{
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
			return "lol";
		}
		static async Task<string> Test(Task<string> readTasks)
		{
			Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
			readTasks.Start();
			await readTasks;
			Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
			System.Threading.Thread.Sleep(1000);
			return "test";
		}
	}
}
