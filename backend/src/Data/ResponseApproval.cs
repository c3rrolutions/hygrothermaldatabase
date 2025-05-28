using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data
{
    [Owned]
    public sealed class ResponseApproval(
        DateTime timestamp,
        string signature,
        string keyFingerprint,
        string query,
        string response
        )
        : IApproval
    {
        public DateTime Timestamp { get; private set; } = timestamp;
        public string Signature { get; private set; } = signature;
        public string KeyFingerprint { get; private set; } = keyFingerprint;
        public string Query { get; private set; } = query;
        public string Response { get; private set; } = response;
    }
}