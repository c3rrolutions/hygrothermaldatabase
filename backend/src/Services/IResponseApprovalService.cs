using System.Threading;
using System.Threading.Tasks;
using Database.Data;

namespace Database.Services;

/// <summary>
/// Service to create response approval
/// </summary>
public interface IResponseApprovalService
{
    /// <summary>
    /// Create response approval by calling graphql Api and signing responce.
    /// </summary>
    /// <param name="dataObject">        <see cref="IData"/> </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="ResponseApproval"/> </returns>
    Task<ResponseApproval> CreateResponseApproval(IData dataObject, CancellationToken cancellationToken);
}