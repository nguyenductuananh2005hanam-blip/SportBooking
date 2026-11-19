using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Common.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(
        Stream content,
        string originalFileName,
        string folder,
        CancellationToken cancellationToken = default);

        Task DeleteImageAsync(string url, CancellationToken cancellationToken = default);
    }
}
