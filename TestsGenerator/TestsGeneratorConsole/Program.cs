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
			Task<string>  l = Test(readTasks);
			l.Wait();
		}
		static string ReadFromFileAsync()
		{
			System.Threading.Thread.Sleep(2000);
			System.Threading.Thread.Sleep(2000);
			return "lol";
		}
		static async Task<string> Test(Task<string> readTasks)
		{
			readTasks.Start();
			await readTasks;
			return "test";
		}
	}
}
