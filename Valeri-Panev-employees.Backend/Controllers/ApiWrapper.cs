namespace Valeri_Panev_employees.Backend.Controllers
{
	/// <summary>
	/// A generic wrapper class for API responses that provides a consistent structure
	/// for returning data, success status, and error messages.
	/// </summary>
	/// <typeparam name="T">The type of data being wrapped in the API response.</typeparam>
	public class ApiWrapper<T>
	{
		/// <summary>
		/// Gets or sets the data payload of the API response.
		/// This property will be null when the response contains errors.
		/// </summary>
		public T? Data { get; set; }

		/// <summary>
		/// Gets or sets the list of error messages associated with the API response.
		/// An empty list indicates no errors occurred.
		/// </summary>
		public List<string> Errors { get; set; } = new();

		/// <summary>
		/// Gets a value indicating whether the API operation was successful.
		/// Returns true when there are no errors, otherwise false.
		/// </summary>
		public bool Success => Errors.Count == 0;

		public ApiWrapper() { }

		public ApiWrapper(T data)
		{
			Data = data;
		}

		/// <summary>
		/// Creates a successful API response containing the specified data.
		/// </summary>
		/// <param name="data">The data to include in the API response.</param>
		/// <returns>A new <see cref="ApiWrapper{T}"/> instance with the provided data and no errors.</returns>
		/// <remarks>
		/// This is a convenience factory method for creating successful responses.
		/// </remarks>
		public static ApiWrapper<T> CreateSuccess(T data)
		{
			return new ApiWrapper<T>(data);
		}

		/// <summary>
		/// Creates a failed API response containing the specified error message.
		/// </summary>
		/// <param name="error">The error message to include in the API response.</param>
		/// <returns>A new <see cref="ApiWrapper{T}"/> instance with the provided error message and no data.</returns>
		/// <remarks>
		/// This is a convenience factory method for creating error responses.
		/// </remarks>
		public static ApiWrapper<T> CreateError(string error)
		{
			return new ApiWrapper<T> { Errors = new List<string> { error } };
		}
	}
}