using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestsGenerator
{
	class Writer
	{
		static object locker = new object();
		private int threadsCount;
		private readonly string outputDirectoryPath;
		private int currWaiters;
		public Writer(int threadsCount, string outputDirectoryPath)
		{
			this.threadsCount = threadsCount;
			this.outputDirectoryPath = outputDirectoryPath;
			currWaiters = 0;
		}
		public Task WriteToFiles(List<Task<List<GeneratedResult>>> generationTasks)
		{
			Dictionary<Task<List<GeneratedResult>>, Task> keyValue = new Dictionary<Task<List<GeneratedResult>>, Task>();
			foreach (Task<List<GeneratedResult>> generationTask in generationTasks)
			{
				keyValue.Add(generationTask, new Task(() => WriteFile(generationTask)));
			}

			for (int i = 0; i < threadsCount; i++)
			{
				RunEveryReadyTaskAsync(generationTasks, keyValue);
			}

			return Task.WhenAll(keyValue.Values);
		}
		private async void RunEveryReadyTaskAsync(List<Task<List<GeneratedResult>>> generationTasks, Dictionary<Task<List<GeneratedResult>>, Task> keyValue)
		{
			Task<List<GeneratedResult>> complitedGenerationTask = null;
			Task writeTask = null;
			bool isWaitGeneration = false;
			while (true)
			{
				lock (locker)
				{
					if (generationTasks.Count - currWaiters > 0 && !isWaitGeneration)
					{
						currWaiters++;
						isWaitGeneration = true;
					}
					else if (isWaitGeneration)
					{
						keyValue.TryGetValue(complitedGenerationTask, out writeTask);
						if (writeTask.Status == TaskStatus.Created)
						{
							currWaiters--;
							isWaitGeneration = false;
							generationTasks.Remove(complitedGenerationTask);
							writeTask.Start();
						}
					}
					else
						break;
				}

				if (isWaitGeneration)
					complitedGenerationTask = await Task.WhenAny(generationTasks);
				else
					await writeTask;
			}

		}

		private void WriteFile(Task<List<GeneratedResult>> generateResults)
		{
			var results = generateResults.Result;
			foreach (var result in results)
			{
				string filePath = $"{outputDirectoryPath}\\{result.Name}";
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					sw.Write(result.Text);
				}
			}
		}
	}
}
