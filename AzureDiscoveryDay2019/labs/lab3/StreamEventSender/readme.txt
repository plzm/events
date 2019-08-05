simulator.zip contains a runnable device simulator for the Azure Discovery Days 2019 workshop, lab 3. The source code is in the src/ folder .

How to use the runnable simulator:
1. Download and install the latest .NET Core Runtime from https://dotnet.microsoft.com/download (the simulator was built on .NET Core 2.2 so use at least that)
2. Obtain your inbound Event Hub's connection string that you created as part of the lab.
3. Download and unzip simulator.zip
4. Open a command prompt in the folder where you unzipped. Run this command:
dotnet Sender.dll "your connection string"

Assuming you completed all the steps in lab 3 successfully, your Azure Function should be receiving messages from this device simulator, and should be showing activity in the "Logs" tab.
