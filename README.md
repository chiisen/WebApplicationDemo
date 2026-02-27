# 🚀 WebApplicationDemo

歡迎來到 **WebApplicationDemo** 專案！這是一個展示如何整合現代 .NET 10 技術棧的範例專案。🌟

## 🛠️ 環境準備

1. 🐋 **安裝 Docker**：確保您的系統已安裝 Docker 環境。
2. 📦 **啟動容器服務**：請依序啟動以下服務：
    - 🗄️ [MS-SQL Server](./Docker/ms_sql/docker-compose.yml)
    - ⚡ [Redis](./Docker/redis/docker-compose.yml)
    - 🔍 [Seq Log Server](./Docker/seq_server/docker-compose.yml)
3. 💻 **安裝開發工具**：建議使用 **Visual Studio 2022** 或 **VS Code**。
4. 🏗️ **資料庫設定**：在 MS-SQL 中建立資料庫與資料表，並透過本專案完成 CRUD API 操作。
5. 🚀 **快取整合**：建立 API 與 Redis 進行資料存取測試。
6. 📊 **日誌監控**：綜合以上操作，將執行歷程完整紀錄至 Seq 系統中。

---

## 🔗 快速連結

- 🌐 **Swagger 測試網址**: [https://localhost:7174/swagger/index.html](https://localhost:7174/swagger/index.html)
- 📂 [檔案目錄詳細說明](./Docs/FileDirectoryDescription.md)
- ⏱️ [Coravel 排程建立指南](./Docs/Coravel.md)

---

## 🧪 執行與測試

### 🏁 啟動 API 本體
使用以下指令來執行 Web API 專案：
```bash
dotnet run --project WebApplicationDemo/WebApplicationDemo.csproj
```

### 🧪 執行單元測試
確保所有功能正常運行：
```bash
dotnet test MSTest/MSTest.csproj
```

