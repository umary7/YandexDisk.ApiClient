# YandexDisk.ApiClient
[![NuGet version (YandexDisk.ApiClient)](https://img.shields.io/nuget/v/YandexDisk.ApiClient.svg?style=flat-square)](https://www.nuget.org/packages/YandexDisk.ApiClient/)

C# client for Yandex.Disk REST API

Yandex.Disk REST API documentation: https://yandex.com/dev/disk/rest/

## Installation

```powershell
Install-Package YandexDisk.ApiClient
```

## Usage

Initialize client with OAuth token:
```csharp
var client = new YandexDiskClient(token);
```

Initialize client with HttpClient from HttpClientFactory in ASP.NET Core:



## License

The MIT License (MIT)
