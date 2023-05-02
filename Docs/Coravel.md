Coravel 呼叫 DemoSchedule 類別，每分鐘執行一次，並且在啟動時執行一次

- EveryMinute 每分鐘執行一次
- PreventOverlapping 如果相同的計劃任務到期，但它的另一個實例仍在運行，Coravel 將忽略當前到期的任務
- RunOnceAtStart 在應用程式啟動時立即運行任務

```csharp=
scheduler.Schedule<DemoSchedule>().EveryMinute().PreventOverlapping("DemoScheduleLock").RunOnceAtStart();
```

---

- PreventOverlappingSchedule 
    - Prevent(防止) 
    - Overlapping(重疊)

呼叫 PreventOverlappingSchedule 類別，每10秒執行一次，並且在啟動時執行一次，由於 PreventOverlapping 會防止重複執行，所以 20 秒那次會取消，30秒後才會執行下一次