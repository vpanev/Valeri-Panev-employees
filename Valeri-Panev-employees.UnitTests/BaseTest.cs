using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Valeri_Panev_employees.UnitTests
{
	public class BaseTest
	{
		protected IFormFile CreateMockFile(string content, string fileName, long? fileSize = null)
		{
			var bytes = Encoding.UTF8.GetBytes(content);
			var stream = new MemoryStream(bytes);

			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns(fileName);
			fileMock.Setup(f => f.Length).Returns(fileSize ?? bytes.Length);
			fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

			fileMock
				.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
				.Callback<Stream, CancellationToken>((s, token) =>
				{
					stream.Position = 0;
					stream.CopyTo(s);
				})
				.Returns(Task.CompletedTask);

			return fileMock.Object;
		}
	}
}
