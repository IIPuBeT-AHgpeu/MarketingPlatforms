using MarketingPlatforms.Models;
using System.Text.RegularExpressions;

namespace MarketingPlatforms.Business
{
    public class RowsValidator
    {
        private Regex _reg;

        public RowsValidator(string pattern)
        {
            _reg = new Regex(pattern);
        }

        public bool ValidateRow(string row) => _reg.IsMatch(row);

        public ValidateInfo<string> ValidateRows(IEnumerable<string> rows)
        {
            var validRows = new List<string>();
            var invalidRows = new List<string>();

            foreach (var row in rows)
            {
                if (ValidateRow(row)) validRows.Add(row);
                else invalidRows.Add(row);
            }

            return new ValidateInfo<string>(invalidRows, validRows);
        }
    }
}
