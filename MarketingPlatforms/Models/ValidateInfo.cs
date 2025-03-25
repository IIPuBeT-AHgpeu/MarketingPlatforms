namespace MarketingPlatforms.Models
{
    public class ValidateInfo<T>
    {
        /// <summary>
        /// Элементы, которые не прошли валидацию.
        /// </summary>
        public IEnumerable<T> InvalidRows { get; private set; }

        /// <summary>
        /// Элементы, успешно прошедшие валидацию.
        /// </summary>
        public IEnumerable<T> ValidRows { get; private set; }

        public ValidateInfo(IEnumerable<T> invalidRows, IEnumerable<T> validRows)
        {
            InvalidRows = invalidRows;
            ValidRows = validRows;
        }
    }
}
