using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGenerator
{ 
	class Generator
	{
		static object locker = new object();
		private int currWaiters;
		private int threadsCount;
		
		public Generator(int threadsCount)
		{
			this.threadsCount = threadsCount;
			currWaiters = 0;
		}

		public List<Task<List<GeneratedResult>>> GenerateTestClasses(List<Task<string>> readTasks)
		{
			Dictionary<Task<string>, Task<List<GeneratedResult>>> keyValue = new Dictionary<Task<string>, Task<List<GeneratedResult>>>();
			foreach(Task<string> readTask in readTasks)
			{
				keyValue.Add(readTask,new Task<List<GeneratedResult>>(() => Generate(readTask)));
			}

			for(int i = 0;i < threadsCount;i++)
			{
				RunEveryReadyTaskAsync(readTasks, keyValue);
			}

			return new List<Task<List<GeneratedResult>>>(keyValue.Values);
		}

		private async void RunEveryReadyTaskAsync(List<Task<string>> readTasks, Dictionary<Task<string>, Task<List<GeneratedResult>>> keyValue)
		{
			Task<string> complitedReadTask = null;
			Task <List <GeneratedResult>> generationTask = null;
			bool isWaitRead = false;
			while (true)
			{
				lock (locker)
				{
					if (readTasks.Count - currWaiters > 0 && !isWaitRead)
					{
						currWaiters++;
						isWaitRead = true;
					}
					else if (isWaitRead)
					{
						keyValue.TryGetValue(complitedReadTask, out generationTask);
						if (generationTask.Status == TaskStatus.Created)
						{
							currWaiters--;
							isWaitRead = false;
							readTasks.Remove(complitedReadTask);
							generationTask.Start();
						}
					}
					else
						break;
				}

				if (isWaitRead)
					complitedReadTask = await Task.WhenAny(readTasks);
				else
					await generationTask;
			}
			
		}

		private List<GeneratedResult> Generate(Task<string> readTask)
		{
			TestClassCreator classCreator = new TestClassCreator();
			return classCreator.GenerateFileAsync(readTask.Result);
		}
	}
}
