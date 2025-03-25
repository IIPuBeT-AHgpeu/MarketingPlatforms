namespace MarketingPlatforms.Business
{
    public interface IFromFileReader<T> where T : class
    {
        public Task<T> ReadFileAsync(IFormFile file);
    }
}
