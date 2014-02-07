# SimTelemetry

SimTelemetry will be a live and post analysis telemetry application for simulation environments, with focus on mainstream race simulators. SimTelemetry is hosted on Github as an open source project licensed under GPLv3. Feel free to contribute to this project where possible.

## Focus

The aim is to create an open and free application to record, display and analyze live telemetry. You can use the program solely to display live track info and car data and/or record telemetry and analyse post race. Several additional features may be added later, like hardware peripheral support, export to 3rd party telemetry formats and more.

## Progress

The software is currently in development for a while. As it started out as a hobby project, progress may be not linear over time. 

At this moment I'm looking into getting support ready for rFactor and rFactor2 , possibly sidetracking other classic race simulators like GTR2 and Race07. If you have any inquires on other race simulators, please create an issue ticket.

The live display will probably look something like this:
https://dl.dropboxusercontent.com/u/207647/ScreenShot444.png
https://dl.dropboxusercontent.com/u/207647/ScreenShot445.png

## Builds

As of now there is no final build or even a beta build available. Once the program is mature enough for a build to be released, a more finalized website will also be made.

## Contributing

Any contribution is welcome! Whether it's contributing (technical) information about other race simulators or actual code contributed to the project. 

SimTelemetry is developed in C# .NET 4.0. A plugin system is implemented to allow for installation of additional plug-ins. The majority of the code should be tested using UnitTests, to detect mistakes earlier and have a clearer specification of the use-case of each object.