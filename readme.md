# Awaitable Wait Handle
Is a set of classes allowing to use await operator on wait handles.

## Usage 
Add using directive and go ahead!
```
using Zagidziran.Concurrent.AwatableWaitHandle;

await new ManualResetEvent(true);
await new ManualResetEvent(false).WithTimeout(TimeSpan.Zero);
```
Additionally you can try *WithTimeout* extension method to specify wait timeout.

## NuGet

Nuget package to use is Zagidziran.Concurrent.AwatableWaitHandle. Awailable on nuget.org
