using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

public interface IResponseApprovalService
{
    Task<ResponseApproval> CreateResponseApproval(object dataObject, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken);
}