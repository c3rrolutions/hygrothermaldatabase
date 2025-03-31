using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;

namespace Database.Services;

/// <summary>
/// Service to check if requested data can be returned regarding access rights.
/// </summary>
public interface IAccessRightsService
{
    /// <summary>
    /// Apply access rights to passed data list.
    /// </summary>
    /// <param name="data">              Data to apply acces rights on. </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> List of data that can be returned. </returns>
    Task<IQueryable<IData>> ApplyAccessRightsOnData(ICollection<IData> data, CancellationToken cancellationToken);

    /// <summary>
    /// Apply access rights on passed data item.
    /// </summary>
    /// <param name="data">              Data to apply acces rights on. </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> Data item. </returns>
    Task<IData?> ApplyAccessRightsOnData(IData data, CancellationToken cancellationToken);
}