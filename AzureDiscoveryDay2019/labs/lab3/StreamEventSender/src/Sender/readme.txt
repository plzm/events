Docker image prep
1. Command prompt
2. Change to the project directory - e.g. C:\Users\MyUserName\Documents\Code\azure-discoveryday2019-mdw\labs\lab3\StreamEventSender\src\Sender
3. docker ps
4. If any pelazem/azurediscday-danrt images running, stop them with docker stop {image ID}
5. docker image ls
6. If any pelazem/azurediscday-danrt images exist, remove them with docker rmi {image ID}
7. docker build --no-cache -t pelazem/azurediscday-danrt:latest -t pelazem/azurediscday-danrt:2.0 .
	a. Update the second (specific version) tag as the simulator changes
8. Do a test run with docker run --rm pelazem/azurediscday-danrt "Endpoint=sb://*****.servicebus.windows.net/;SharedAccessKeyName=eh1send;SharedAccessKey=*****=;EntityPath=eh1"
	a. Make sure you enclose the event hub connection string in double quotes!
	b. Substitute your values for ***** in the example Event Hub connection string
	c. Be ready to monitor the event hub for incoming messages
9. Hit ctrl-C to stop sending test messages. Then I suggest docker ps to get the running container ID, followed by docker stop {container ID}.
10. docker push pelazem/azurediscday-danrt
