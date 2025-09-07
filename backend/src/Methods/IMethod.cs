using System;
using System.Text.Json;

namespace Database.Methods;

public interface IMethod
{
    public JsonDocument Calculate(JsonDocument input);
}