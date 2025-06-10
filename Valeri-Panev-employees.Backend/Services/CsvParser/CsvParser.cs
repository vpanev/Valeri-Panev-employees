using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Valeri_Panev_employees.Backend.Common;
using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.Backend.Services.CsvParser
{
	public class CsvParser : ICsvParser
	{
		private readonly ILogger<CsvParser> _logger;

		public CsvParser(ILogger<CsvParser> logger)
		{
			_logger = logger;
		}

		public async Task<List<EmployeeData>> ParseAsync(IFormFile file)
		{
			_logger.LogInformation($"{nameof(CsvHelper.CsvParser)}: File with name ${file.Name} will be processed.");

			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
				TrimOptions = TrimOptions.Trim,
				BadDataFound = x => { _logger.LogWarning($"Bad data found at row {x.RawRecord}"); },
				MissingFieldFound = x =>
				{
					_logger.LogWarning($"Missing field at index {x.Index}, header: {x.HeaderNames?[x.Index] ?? "unknown"}");
				}
			};

			var list = new List<EmployeeData>();
			await using var stream = file.OpenReadStream();
			using var reader = new StreamReader(stream);
			using var csv = new CsvReader(reader, config);
			await csv.ReadAsync();
			csv.ReadHeader();

			while (await csv.ReadAsync())
			{
				try
				{
					if (!csv.TryGetField(StringConstants.EmpIdColumn, out int emp)
						|| !csv.TryGetField(StringConstants.ProjectIdColumn, out int proj))
						continue;

					var from = ParseDate(csv.GetField(StringConstants.DateFromColumn)!);
					var to = ParseDate(csv.GetField(StringConstants.DateToColumn)!, shouldAssignNullDate: true);

					list.Add(new EmployeeData { EmpID = emp, ProjectID = proj, DateFrom = from, DateTo = to });
				}
				catch (Exception ex) when (ex is not FormatException)
				{
					_logger.LogError(ex, $"Error processing CSV row: {csv.Context.Parser?.RawRecord}");
				}
			}

			return list;
		}

		private static DateTime ParseDate(string token, bool shouldAssignNullDate = false)
		{
			if (shouldAssignNullDate && token.Equals("NULL", StringComparison.OrdinalIgnoreCase))
				return DateTime.Today;

			if (DateTime.TryParseExact(token.Trim(),
					Constants.DateFormats,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var dt))
				return dt;

			throw new FormatException($"Unrecognised date format: '{token}'.");
		}
	}
}
