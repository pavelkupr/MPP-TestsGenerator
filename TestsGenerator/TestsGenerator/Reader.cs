using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
    public class Reader
    {
		public async Task<string> ReadFromFileAsync(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}
