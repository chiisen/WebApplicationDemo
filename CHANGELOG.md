# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- 升級專案至 .NET 10 (`net10.0`)。
- 升級測試專案至 .NET 10 (`net10.0`)。
- 更新所有相依套件至最新版本以支援 .NET 10。

### Fixed
- 修正 `Program.cs` 中的過時 API 警告（使用 `builder.Logging` 代替 `builder.Host.ConfigureLogging`）。
- 修正 `Program.cs` 與 `CacheService.cs` 中的潛在 Null 參考警告。
- 修正 `CacheService.cs` 中的 StackTrace 安全性存取。

### Security
- 修正 `Microsoft.Data.SqlClient` 與其傳遞相依性中的多個高嚴重性安全性弱點。
