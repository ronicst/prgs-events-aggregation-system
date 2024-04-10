set currentPath=%cd%
sc.exe create "EventsProcessWindowsService" binpath=%currentPath%\src\EventsProcessWindowsService\bin\Debug\EventsProcessWindowsService.exe