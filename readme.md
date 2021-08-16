# Concurrency helper classes

## Awaitable Wait Handle
Is a set of classes allowing to use await operator on wait handles.

### Usage 
Add using directive and go ahead!
```
using Zagidziran.Concurrent.AwatableWaitHandle;

await new ManualResetEvent(true);
await new ManualResetEvent(false).WithTimeout(TimeSpan.Zero);
await new ManualResetEvent(false).WithCancelation(cancellationToken);
```
Additionally you can try *WithTimeout* extension method to specify wait timeout or *cancellationToken* to pass cancellation token.

### NuGet

Nuget package to use is Zagidziran.Concurrent.AwatableWaitHandle. Awailable on nuget.org

### Limitations

Waithandle should not be disposed while awaiting. The code uses [WaitForMultipleObjects](https://docs.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitformultipleobjects) behind the scenes and the API bevaior in this case is undefined.

## SpinMonitor 
Is a class used to syncronize on objects in monitore-like style but without cross-limitations preventing using await inside the lock statement.
Implementation is based on SpinWait. 

### Usage
```
using Zagidziran.Concurrent.SpinMonitor;

using var _ = SpinMonitor.Enter(lockObject)
DoMyStuff(); 
```

CancellationToken and timeout overloads are supported;

### NuGet

Nuget package to use is Zagidziran.Concurrent.SpinWait. Awailable on nuget.org
