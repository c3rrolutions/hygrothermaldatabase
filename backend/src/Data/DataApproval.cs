using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class DataApproval(
    DateTime timestamp,
    string signature,
    string keyFingerprint,
    string query,
    string response,
    Guid approverId
    )
        : IApproval
{
    public Guid ApproverId { get; private set; } = approverId;
    public DateTime Timestamp { get; private set; } = timestamp;
    public string Signature { get; private set; } = signature;
    public string KeyFingerprint { get; private set; } = keyFingerprint;
    public string Query { get; private set; } = query;
    public string Response { get; private set; } = response;
    public Publication? Publication { get; set; }
    public Standard? Standard { get; set; }
    [NotMapped] public IReference? Statement => Standard is not null ? Standard : Publication;
}