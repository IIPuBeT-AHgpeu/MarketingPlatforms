using MarketingPlatforms.Business;
using MarketingPlatforms.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatforms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingPlatformsController : ControllerBase
    {
        private readonly IFromFileReader<string> _fr;
        private readonly IRepository<IEnumerable<string>> _rep;

        public MarketingPlatformsController(IFromFileReader<string> fr, IRepository<IEnumerable<string>> rep)
        {
            _fr = fr;
            _rep = rep;
        }

        [HttpPost]
        [Route("/")]
        public async Task<IActionResult> SetConfig(IFormFile file)
        {
            // Проверка входных данных.
            if (file is null) return BadRequest("В запросе отсутствует файл конфигурации.");
#if DEBUG
            Console.WriteLine($"Получен файл. Тип: {file.ContentType}\tимя: {file.FileName}");
#endif
            if (file.ContentType != "text/plain") return BadRequest("Тип файла в запросе не соответствует типу text/plain.");

            // Чтение файла.
            string text = await _fr.ReadFileAsync(file);
            if (String.IsNullOrEmpty(text)) return BadRequest("Файл конфигурации пуст!");

            // Разделение текста на строки.
            string[] lines = text.Split("\r\n");

            // Валидация строк.
            var validator = new RowsValidator(@"^[A-za-zА-Яа-я0-9_ \.]+:((\/[a-z0-9]+)+,?)+$");
            // В переменной validateRows хранятся как валидные, так и невалидные записи. Невалидные записи можно использовать для поиска ошибок или для логгирования.
            var validatedRows = validator.ValidateRows(lines);

            if (validatedRows.ValidRows.Count() == 0) return BadRequest("Не удалось найти данные валидного формата в переданном файле конфигурации.");

            // Сохранение данных, если это возможно.
            if (!_rep.TrySetValue(validatedRows.ValidRows)) return BadRequest("Не удалось распарсить конфигурацию, возможна ошибка в форматировании данных.");

            return NoContent();
        }

        [HttpGet]
        [Route("/platforms")]
        public IActionResult GetPlatformsByLocation([FromQuery] string location)
        {           
            RowsValidator validator = new RowsValidator(@"(\/[a-z0-9]+)+");
            var inputIsValid = validator.ValidateRow(location);
            if (!inputIsValid) return BadRequest("Переданная строка location не соответствует формату.");

            var platforms = _rep.GetDataByKey(location);
            if (platforms == null) return BadRequest("Не был установлен файл конфигурации!");

            return Ok(platforms); 
        }
    }
}
