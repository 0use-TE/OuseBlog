```csharp
JsonSerializer.Serialize(mergedData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
```

