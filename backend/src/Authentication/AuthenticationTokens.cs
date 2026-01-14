using System;

namespace Database.Authentication;

public sealed record AuthenticationTokens(
    string AccessToken,
    DateTimeOffset? AccessTokenExpirationDate,
    string? IdentityToken,
    string? RefreshToken
);