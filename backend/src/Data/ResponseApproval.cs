using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data
{
    [Owned]
    public sealed class ResponseApproval
    : IApproval
    {
        public ResponseApproval(
            DateTime timestamp,
            string signature,
            string keyFingerprint,
            string query,
            string response
        )
        {
            Timestamp = timestamp;
            Signature = signature;
            KeyFingerprint = keyFingerprint;
            Query = query;
            Response = response;
        }

        public DateTime Timestamp { get; private set; }
        public string Signature { get; private set; }
        public string KeyFingerprint { get; private set; }
        public string Query { get; private set; }
        public string Response { get; private set; }
    }
}