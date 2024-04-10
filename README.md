# events-aggregation-system
Testing strategy for events-aggregation-system
1. Covering Web API routers based on the REST requests
   * Test the endpoints exposed by API
   * Test the received messages in RabbitMQ
   * Follow E2E scenario
1. Structure of test library:
   * Common - providing helper functions to simplify
   * Configuration - providing configurations as constatns
   * E2ETests - testing workflow
   * MessageManager - for managing the messages in the queue
   * MessageQueueTests - positive tests for testing the received messages
   * Model - contains Dtos for the provided events type
   * WebApiRoutesTests - contains the tests covering web API routers
1. WebApiRoutesTests:
   * Test the positive scenario when all parameters are provided to requests
   * Negative scenarios verify the web API behavior when some of parameters are:
     ** Empty
     ** Invalid format
1. MessageQueueTests - Test the integration between web api and rabbit mq, covering the positive path.
1. e2e test covers the user flow:
   * Register user
   * Login user
   * Download file
   * Logout
  
Next step:
1. Extract the common classes in common library - good candidate is building the requests with different parameters.
2. Enhance MQManager to validate that after the BadRequest, there isn’t any received message in the queue.

Limitation:
1. e2e workflow doesn't reflect to the real flow because of missing of the unique identifier for the created user and to ensure thatexactly the same user has been login and downloaded the file.


How to configure and run SUT
Reqirements:
Windows, VisualStudio 2022, .net 8 SDK, .netFramework 4.8 SDK

How to run the project:

One time setup:
- Download and install
    - latest Erlang from https://www.erlang.org/downloads
    - latest RabbitMq service from https://www.rabbitmq.com/docs/install-windows
- Enable Management plugin from this documentation - https://www.rabbitmq.com/docs/management
(default login details are always username: guest, password: guest, do not change them on loalhost env)

If your network configuration support IPv4 and IPv6:
Known issue with rabbitmq: When there are both configuration (IPv4 and IPv6), Rabbitmq node seems to be listening only to IPv6, ignoring IPv4
Workaround:
* Disable IPv6 configuration
* Reinstall rabbitmq 


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

Note: There is addtitional queue - eventsQueue.tests - which also receives the same messages, but they are not consumed, it is to be used in tests for validations of the features in the web api.
- More details can be found in - SystemDocumentation.txt
