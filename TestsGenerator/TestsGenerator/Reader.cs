using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class Reader
    {
		private int threadsCount;
		public Reader(int threadsCount)
		{
			this.threadsCount = threadsCount;
		}

		public List<Task<string>> ReadFromFilesAsync(List<string> paths)
		{
			List<Task<string>> readTasks = new List<Task<string>>();
			foreach (string path in paths)
			{
				readTasks.Add(ReadFromFile(path));
			}

		}

		private async void RunFreeReadTask(List<Task<string>> readTasks)
		{
			foreach(Task<string> readTask in readTasks)
			{
				if (readTask.Status == TaskStatus.Created)
					await readTask;
			}
			
		}

		private Task<string> ReadFromFile(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return new Task<string>(reader.ReadToEnd);
			}
		}
		/*
		private async Task<string> ReadFromFileAsync(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return await reader.ReadToEndAsync();
			}
		}*/
	}
}
