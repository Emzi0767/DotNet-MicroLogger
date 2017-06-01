﻿![MicroLogger](https://raw.githubusercontent.com/Emzi0767/DotNet-MicroLogger/master/art/ulogger.png)

# MicroLogger for .NET Framework

A tiny logger implementation for .NET Framework.

## Compatible .NET versions:

The library is compatible with following versions of .NET Framework:

* .NET Framework 2.0
* .NET Framework 4.0
* .NET Framework 4.5
* .NET Framework 4.6
* .NET Framework 4.7
* .NET Standard 1.0 (See [here](https://blogs.msdn.microsoft.com/dotnet/2016/09/26/introducing-net-standard/) for details)
* PCL (.NETFX 4.0, Windows 8, Sliverlight 4, Windows Phone 7)

## Installation

Install [`Emzi0767.MicroLogger`](https://www.nuget.org/packages/Emzi0767.MicroLogger) from NuGet.

## Usage

The logger is in the `Emzi0767` namespace. To initialize the logger, create an instance of it.

The logger implements `IDisposable`, it is recommended you either use it in a `using` block, or remember to `.Dispose()` it when you're done working with it.

### Configuration

The logger is fairly configurable. Here's a quick explanation of what its settings do:

* `OutputToDebug`: Enables outputting to `System.Diagnostics.Debug`. By default, this is disabled. Note that this option is not available in the PCL version of the library.
* `PaddingCharacter`: Character used to pad tag strings. Example: setting it to `'_'`, and `TagLength` to `6`, will pad `"tag"` to `"tag___"`.
* `TagLength`: Length to which to pad or trim tags. Example: setting it to `5`, and `PaddingCharacter` to `' '`, will pad `"tag"` to `"tag  "`, trim `"tagtag"` to `"tagta"`, and leave `"penta"` as-is.
* `DefaultTag`: Tag used for `Log` and `LogAsync` calls which do not specify a tag.
* `DateTimeFormat`: String used to determine how to format `DateTime` and `DateTimeOffset` objects when printing them. Defaults to `"yyyy-MM-dd HH:mm:ss zzz"`.
* `LoggingLevel`: Maximum level of messages to log. For example, setting this to `LogLevel.Error` will only log messages with `LogLevel` of `.Error` and `.Critical`, while `.Info`, `.Verbose`, `.Warning`, and `.Debug` will be ignored. Note that `System.Diagnostics.Debug` will receive all messages, regardless of level.

### Registering and unregistering loggers

To register a logger, pass a `System.IO.TextWriter` instance to `.RegisterOutput()`. To unregister a logger, pass its instance to `.UnregisterOutput()`.

### Logging

To log a message, call an appropriate `.Log()` overload.

In for .NET Framework 4.5+ and .NET Standard targets, asynchronous `.LogAsync()` overloads are available.

### Complete example

```cs
using System;
using System.IO;
using System.Text;
using Emzi0767;

// let's log to a file
var fn = string.Concat("log-", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), ".log");
var utf8 = new UTF8Encoding(false);

using (var fs = File.Create(fn))
using (var sw = new StreamWriter(fs, utf8))
using (var log = new MicroLogger()) // or just remember to .Dispose() it
{
	log.TagLength = 10;
	log.RegisterOutput(sw);
	log.RegisterOutput(Console.Out); // also log to console
	
	// log something synchronously
	log.Log(DateTime.Now, LogLevel.Info, "example", "Log registration successful.");

	// log something asynchronously
	await log.LogAsync(DateTime.Now, LogLevel.Info, "example", "This one was async!");
}
```

That's about it. If you have any questions, issues, suggestions, you can always open an issue or submit a PR.

## License

The project is licensed under Apache License 2.0.