using System;
using System.Text.Json;

namespace Database.Methods;

public interface IMethod
{
    public JsonElement Calculate(JsonElement input);
}