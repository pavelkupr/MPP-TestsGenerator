using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class Reader
    {
		static object locker = new object();
		private int threadsCount;
		public Reader(int threadsCount)
		{
			this.threadsCount = threadsCount;
		}

		public List<Task<string>> ReadFromFiles(List<string> paths)
		{
			List<Task<string>> readTasks = new List<Task<string>>();
			foreach (string path in paths)
			{
				readTasks.Add(new Task<string>(() => CreateReadFromFileTask(path)));
			}

			for (int i = 0; i< threadsCount; i++)
			{
				RunFreeTasksAsync(readTasks);
			}
			return new List<Task<string>>(readTasks);
		}

		private async void RunFreeTasksAsync(List<Task<string>> readTasks)
		{
			foreach(Task<string> readTask in readTasks)
			{
				lock (locker)
				{
					if (readTask.Status == TaskStatus.Created)
						readTask.Start();
				}
				if (readTask.Status != TaskStatus.Created)
					await readTask;
			}
			
		}

		private string CreateReadFromFileTask(string path)
		{
			System.Threading.Thread.Sleep(5000);
			using (StreamReader reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
