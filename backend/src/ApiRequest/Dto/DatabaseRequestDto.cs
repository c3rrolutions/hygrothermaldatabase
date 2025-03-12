using System;

namespace Database.ApiRequest.Dto;

public sealed record DatabaseRequestDto(
Guid DatabaseId,
string Name,
string Description,
Uri Locator
);