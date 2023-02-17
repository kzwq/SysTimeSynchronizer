# SysTimeSynchronizer
 A simple program for time synchronization on a specific NTP server. Built on .NET Framework v4.0, so this should run on Windows 7+ without additional library install. Code from this repo is configured to fetch time from `0.ru.pool.ntp.org`. You can change server by editing `Program.cs`.

## What is it doing?
1. Wait for 5000 ms
2. `DateTime time = `Get `DateTime` from NTP Server (thanks to [StackOverflow answer](https://stackoverflow.com/a/12150289)) (if there're internet issues, program'll exit with exit code `1`)
    1. Connect to NTP Server
    2. `var data = 0x1B`
    3. Send `data`
    4. Receive `data`
    5. Get timestamp from `data`
    6. `var target = new DateTime(1970, 1, 1)`
    7. Add timestamp to `target`
3. `SYSTEMTIME st = `Manual convert `time` to `SYSTEMTIME` (`WINAPI enum`)
4. `SetSystemTime(ref st)` (from `kernel32.dll`)
5. Done

## Who is this program for?
For computers with a dead CMOS battery, so you need to install this program for startup.