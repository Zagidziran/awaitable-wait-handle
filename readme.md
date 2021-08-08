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

## Limitations

Waithandle should not be disposed while awaiting. The code uses [WaitForMultipleObjects](https://docs.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitformultipleobjects) behind the scenes and the API bevaior in this case is undefined.