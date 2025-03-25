using MarketingPlatforms.Business;
using System.Text;

namespace MarketingPlatforms.Infrastructure
{
    public class TxtReader : IFromFileReader<string>
    {      
        public async Task<string> ReadFileAsync(IFormFile file)
        {
            byte[] bytes = new byte[file.Length];

            using (var s = file.OpenReadStream())
            {
                await s.ReadAsync(bytes, 0, bytes.Length);
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
