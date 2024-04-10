# events-aggregation-system

Reqirements:
Windows, VisualStudio 2022, .net 8 SDK, .netFramework 4.8 SDK

How to run the project:

One time setup:

- Download and install
    - latest Erlang from https://www.erlang.org/downloads
    - latest RabbitMq service from https://www.rabbitmq.com/docs/install-windows

- Enable Management plugin from this documentation - https://www.rabbitmq.com/docs/management

(default login details are always username: guest, password: guest, do not change them on loalhost env)
- Login at http://localhost:15672/#/ , under Overview tab expand Import definitions choose file: rabbit_definitions.json from this repo, click Upload broker definitions.

- Install windows service
    - Open src/All.sln in VisualStudio and build it.
    - Open cmd.exe as Administrator, navigate to this repo folder and execute InstallWindowsService.bat (do not run it from file explorer)
    - Locate the new service in Services.msc and start it.

Running the services:

- Web api may be launched with F5 in VisualStudio in dedug mode, Or by executing - dotnet run in src\EventsWebService folder
- Api is explorable at - http://localhost:5083/swagger/index.html
- Authentication key is in the appsettings.json file.
- Windows service is in manual mode, start it when needed.

- Basic functionality of the system:

1. Web api is accepting different type of events.
1. When event is post it's data and type is send as message to the rabbitMQ queue.
1. The windows service is listening for messages in the queue.
1. When a message is received it than stores the data in a local database file - \src\EventsProcessWindowsService\eventsdb.db

There is addtitional queue - eventsQueue.tests - which also receives the same messages, but they are not consumed, it is to be used in tests for validations of the features in the web api.

- More details can be found in - SystemDocumentation.txt