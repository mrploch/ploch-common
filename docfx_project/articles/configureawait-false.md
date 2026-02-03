# ConfigureAwait(false) explained: what it does and when to use it

This guide explains how `ConfigureAwait(false)` works in .NET, its benefits and trade‑offs, and provides practical rules of thumb for when to use it.

## TL;DR
- `await someTask.ConfigureAwait(false)` tells the awaiter NOT to capture the current SynchronizationContext/TaskScheduler for the continuation.
- This means the continuation after the `await` can run on any thread (usually a ThreadPool thread).
- Use it in library/internal code paths that do not touch thread‑affine contexts (UI thread, ASP.NET classic request context).
- Do NOT use it when you must resume on a specific context (e.g., WinForms/WPF UI updates, ASP.NET classic code that touches `HttpContext.Current`).
- In ASP.NET Core, Console apps, Worker services, and background code that doesn’t care about context, it’s typically safe and can slightly improve throughput.

## What does `ConfigureAwait(false)` actually do?
When you `await` a `Task`, by default the awaiter:
1. Captures the current context (if any):
   - UI apps (WinForms/WPF) have a `SynchronizationContext` that represents the UI thread.
   - ASP.NET (classic, .NET Framework) has a request `SynchronizationContext`.
   - In ASP.NET Core and most console/services scenarios, there is no custom `SynchronizationContext` (it’s usually null), so continuations resume on the `TaskScheduler.Current` which is typically the ThreadPool.
2. Schedules the continuation to run back on that captured context.

`ConfigureAwait(false)` tells the awaiter: “Do not capture the context.” The continuation is free to run on any available thread (typically a ThreadPool worker). This avoids the overhead of marshaling back to a specific context and can sidestep certain deadlock patterns.

Notes:
- `ConfigureAwait(false)` does not suppress `ExecutionContext` flow (e.g., `AsyncLocal<T>`, security principal). It only affects context capture for continuations (SynchronizationContext/TaskScheduler).
- There are also `ValueTask.ConfigureAwait(false)` overloads with the same semantics.

## Why/when is it useful?
- Avoiding deadlocks in context-bound environments: In UI and ASP.NET classic apps, blocking on async (e.g., calling `.Result`/`.Wait()`) can deadlock because the continuation tries to get back to the captured context, which is blocked by the synchronously waiting thread. Using `ConfigureAwait(false)` in library internals reduces the chance of such deadlocks when consumers misuse async.
- Throughput and latency: Skipping context capture avoids a marshal back to the original context, which can slightly improve performance, especially in hot paths or high-frequency awaits.
- Library code that shouldn’t assume a caller’s context: Libraries generally should not require resuming on a particular context; letting continuations run anywhere makes them more broadly usable.

## When NOT to use it
Use the default `await` (without `ConfigureAwait(false)`) if you need to continue on a specific context:
- UI apps (WinForms/WPF) after the await you update controls or interact with UI-bound objects.
- ASP.NET classic code that relies on `HttpContext.Current` or other context-affine APIs right after the await.
- Any code that assumes thread-affinity or requires a particular `SynchronizationContext` or `TaskScheduler`.

## Typical guidance by application type
- UI apps (WinForms/WPF):
  - In UI-layer methods where you update the UI after await, omit `ConfigureAwait(false)` so you resume on the UI thread.
  - In deeper library/helper methods that don’t touch UI, consider `ConfigureAwait(false)` to avoid marshaling back.
- ASP.NET (classic, .NET Framework):
  - In request-handling code that accesses context-bound APIs after an await, omit `ConfigureAwait(false)`.
  - In libraries and background work not touching the request context, consider `ConfigureAwait(false)`.
- ASP.NET Core, Console apps, Worker services:
  - There’s typically no custom `SynchronizationContext`; continuations already run on the ThreadPool.
  - You can still use `ConfigureAwait(false)` in libraries for consistency and to avoid future surprises if code is reused in a context-bound environment, but it’s often not necessary for deadlock avoidance.

## Common patterns
- Library/internal helper method that does not touch UI or request context:
  ```csharp
  public static async Task<byte[]> DownloadAsync(HttpClient http, string url, CancellationToken ct)
  {
      using var resp = await http.GetAsync(url, ct).ConfigureAwait(false);
      resp.EnsureSuccessStatusCode();
      return await resp.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
  }
  ```

- UI layer method that must update the UI after await:
  ```csharp
  private async Task LoadAsync()
  {
      var data = await _service.GetDataAsync(); // no ConfigureAwait(false)
      // Safe to update UI because we resumed on the UI context
      myLabel.Text = data.Title;
  }
  ```

- ASP.NET Core controller (context not required):
  ```csharp
  [HttpGet("/items/{id}")]
  public async Task<ItemDto> GetItem(Guid id, CancellationToken ct)
  {
      // ConfigureAwait(false) is optional here; it won’t change behavior in most ASP.NET Core scenarios
      return await _service.GetItemAsync(id, ct).ConfigureAwait(false);
  }
  ```

## FAQs
- Does `ConfigureAwait(false)` make code run faster? Sometimes a bit. It removes the overhead of context capture and marshaling back, which can help in hot paths. But don’t expect dramatic gains; prioritize clarity, correctness, and consistent policy.
- Is it required in ASP.NET Core? No. There’s usually no SynchronizationContext to capture, so it rarely changes behavior. It can still be a good default in reusable libraries.
- Can I update the UI after `await x.ConfigureAwait(false)`? Not directly. You would need to marshal back to the UI thread (e.g., using `SynchronizationContext.Post` or the UI framework’s dispatcher) before touching UI components.
- Does it affect `AsyncLocal<T>` or security principals? No. `ConfigureAwait(false)` does not suppress `ExecutionContext`; values in `AsyncLocal<T>` still flow.

## Practical rules of thumb
- Library/internal code: default to `ConfigureAwait(false)` unless you have a reason to resume on the caller’s context.
- App/UI/request code: omit `ConfigureAwait(false)` when you rely on resuming on the context right after the await.
- Avoid sync-over-async (`.Result`/`.Wait()`), especially on context-bound threads. Prefer fully async flows; `ConfigureAwait(false)` helps but is not a substitute for proper async usage.

## See also
- Stephen Toub, “ConfigureAwait FAQ” (Microsoft DevBlogs)
- Stephen Cleary, “Don’t block on async code” and “Context vs. Context”
