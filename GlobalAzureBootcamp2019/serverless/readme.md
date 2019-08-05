# Global Azure Bootcamp 2019 - Manchester, NH

## Serverless Text Analytics Pipeline

## Summary

Say you have text - whether social media messages about your organization (or you!), or a press release, or an earnings statement, or anything.

It is very useful to analyze text and extract certain insights from it. Text analytics and language processing is complex, so it's great that Microsoft has a set of Azure AI Cognitive Services. You can read more about these here: https://azure.microsoft.com/services/cognitive-services/.

Briefly, Azure Cognitive Services are a set of web services, each of which exposes some Artificial Intelligence capabilities prepared by Microsoft, and made available through a web API endpoint that we can use in ordinary code.

In this workshop, you will build a simple processing pipeline in Azure. This pipeline contains the following pieces:

1. An inbound Queue. Pieces of text (whether emails, or social media messages, etc.) will be put into this queue by some external process. The pieces of text will wait in the Queue until something else picks them up for further processing.
    - Queues are great for building loosely coupled applications. They are a fundamental element of modern processing platforms.
2. A serverless Azure Function. This Azure capability lets us run our own code without worrying about virtual machines and other infrastructure. You'll create an Azure Function to take incoming messages (pieces of text) off the Queue and do some work on each text message.
3. A Text Analytics Cognitive Service. You'll deploy one of these. Your Azure Functions code will call this cognitive service, pass it the message it got off the Queue, and bring back insights about the message.
4. Azure Blob Storage. After your Azure Function retrieves insights about a message, you'll save the insights as text to a file in storage.

![](images/architecture.png?raw=true)

## Level / Pre-Requisites / Time

Level: Intermediate 

Pre-Requisites:
- Azure subscription. If you don't have one, you can get started at https://azure.com/free.
- Some programming experience, especially in C#, will be helpful but is not required. The more you have, the more you can customize and change this workshop, but if you are new to C#, don't worry - follow along and ask your presenter/proctor for help.

Time: 60 minutes. Plus - all instructions/files are in this public github repo, so you can keep working on this after the session.

## Tasks

### Log into the Azure portal

Go to https://portal.azure.com and log in. See the "iac-101" workshop in this repo for details on logging in, if needed.

### Create a new Resource Group for this workshop

Create a new Resource Group for this workshop.

**Option 1 - Portal**

You can do this in the portal by clicking "+ Create a resource", typing "Resource Group" into the search box, then clicking "Create". Provide a name and region and finish the creation process.

![](images/rg_create.png?raw=true)

**Option 2 - Cloud Shell and Azure CLI**

You can also use the Cloud Shell and Azure CLI commands. See the "iac-101" workshop in this repo for details on Azure CLI and Cloud Shell.

Type the command `az group create --name gab19-rg-serverless --location eastus`. In this command, I'm specifying my Resource Group name as "gab19-rg-serverless", but use a name you like. And I'm specifying the East US ("eastus") Azure region; use that for all resources in this workshop.

_Tip_: remember that at the Cloud Shell, you can first type `az interactive` so that you'll get interactive suggestions as you type Azure CLI commands.

_Note_: not all screenshots in this workshop will use exactly the same names. That's fine: use your own names for Resource Groups and other resources anyway.

When you're done, navigate into your Resource Group.

### Create a Storage Account

Azure storage accounts provide several storage services. In this workshop, we'll use two:
- Queues: we'll use a queue for incoming messages to process
- Blob Storage: we'll use this to store a file with our text analytics result

**Option 1 - Portal**

In your Resource Group, locate the "+ Add" button and click it.

![](images/rg_add_button.png?raw=true)

In the search box, type "Storage Account" and click Enter, then select "Storage Account" and click Create.

![](images/storage_create.png?raw=true)

On the "Create storage account" page:
- Specify the Resource Group you just created
- Specify a storage account name: you must use all lower case letters and digits, and this name must be globally unique. This page will show you error messages as you type, until you get a storage account name that is acceptable.
- Ensure "(US) East US" is set for Location.
- Set "Replication" to "Locally-redundant storage (LRS)
- Leave the other defaults as is.

Click "Review + create", then click "Create" on the next page.

![](images/storage_create2.png?raw=true)

![](images/storage_create3.png?raw=true)

After storage account creation completes, you will see it in your Resource Group. Note that you may need to click the Resource Group's "Refresh" button for the new storage account to show up.

![](images/rg_refresh.png?raw=true)

**Option 2 - Cloud Shell and Azure CLI**

Customize the following command with your own Resource Group name that you created previously, and a unique storage account name. _Do not run this command until you make those two changes! And don't use the same storage account name and resource group name shown in my screenshot below the command line!_

`az storage account create --name YOUR_STORAGE_ACCT_NAME_GOES_HERE --resource-group YOUR_RESOURCE_GROUP_NAME_GOES_HERE --location eastus --kind StorageV2 --sku Standard_LRS`

After running your CLI command, address any error messages (for example, about your storage account name) and try again until the command runs and you can see a success message in the output.

![](images/storage_create_cli.png?raw=true)

![](images/storage_create_cli2.png?raw=true)

Now make sure you can see the new storage account in the portal, in your Resource Group. (See note about the Resource Group "Refresh" button above.)

### Deploy Blob Storage and Queue

In the portal, navigate to your storage account and go to its Overview page. Among other information, you should see tiles for "Blobs" and "Queues".

#### Blob Container

Navigate into the Blobs tile and create a new Container using the "+ Container" button. Give it a very simple name, like "messages", click OK, and make sure your new container shows up in the list of containers.

Now navigate back to your storage account's Overview.

#### Queue

Navigate into the Queues tile and create a new Queue using the "+ Queue" button. Give it a simple name, like "incoming", click OK, and make sure your new queue shows up in the list of queues.

### Deploy Text Analytics Cognitive Service

Now, deploy the Text Analytics AI Cognitive Service that you'll use to process the text of incoming messages.

In your Resource Group, click "+ Add". Enter "text analytics" in the search box, hit Enter, then click "Text Analytics" in the results, then click "Create".

![](images/cog_svc_create.png?raw=true)

On the Create page, provide a name for your cognitive service. In the screenshot, you can see I'm naming mine "gab19-txta". Feel free to come up with your own brief, descriptive name.

Set your region to East US.

**Important** Set the Pricing tier to "S0 (25K Transactions per 30 days)" as shown. This is the lowest tier that enables ALL Text Analytics capabilities.

Last, make sure your Resource Group is selected, then click Create.

![](images/cog_svc_create2.png?raw=true)

After deployment completes, you should see the new "Cognitive Services" in your Resource Group Overview page. (Remember that Resource Group "Refresh" button...)

You'll come back to this new Cognitive Service resource, but for now you have one more resource to deploy first.

### Deploy Azure Function

In your Resource Group, click "+ Add" again. In the search box, type "Function", click "Function App" in the list, then click "Create".

![](images/azfn_create.png?raw=true)

On the Create page, specify options as follows.
- App name: you'll need a unique name here. The page will show you error messages until you type in an acceptable one.
- Resource Group: select your existing Resource Group here
- OS: Set this to "Windows"
- Hosting Plan: set this to "Consumption Plan"
- Location: set this to "East US"
- Runtime Stack: set this to ".NET"
- Storage: select "Use existing", then select the storage account you created above
- Application Insights: leave it as is.
When you have set all the options, click "Create" and wait for the deployment to complete. Look for the new Function App in your Resource Group Overview (remember that Resource Group "Refresh" button...)

![](images/azfn_create2.png?raw=true)

### Review

At this point you should see all the above resources in your Resource Group's Overview page. If not (remember the "Refresh" button...) then go back and see what you missed before continuing.

![](images/rg_all_resources.png?raw=true)

### Configure the Azure Function

Now you'll go through several steps to get the Azure Function ready to process messages.

In your Resource Group, click on the Function App you created. Find the "+ New function" button and click it.

On the next screen, click "In-portal", then click "Continue". (As you can see, there are several other options for Function authoring; we won't use those in this workshop but they are available for more advanced scenarios.)

![](images/azfn1.png?raw=true)

On the next screen, click "More templates...", then "Finish and view templates".

![](images/azfn2.png?raw=true)

On the next screen, find and click "Azure Queue Storage trigger".

![](images/azfn3.png?raw=true)

Next, you may see a message "Extensions not installed" with an "Install" button. If so, click "Install" and wait for the installation to complete.

On the next screen, give your new function a name, and enter the name of the queue you created above. **Be careful to enter the queue name exactly!** Then click "Create".

![](images/azfn4.png?raw=true)

Next, you will be in the main code file, run.csx, of your new function. Some starter code is pre-filled for you. Note the "Logs" tab at the bottom; click it to bring up a live log feed. In general, keep this tab opened while you are working in your Function.

![](images/azfn5.png?raw=true)

Next, before you write any code, you need to tell your Azure Function to get a Nuget package. If you don't know what that is: Nuget is a public repository of add-in libraries that you can use in your .NET code.

To do this with an Azure Function that you're editing in the browser in the Azure portal, you need to upload a special file, with a special format, into your Function.

In this repo, look for a file named "function.proj". If you cloned the repo, this file will be in the "serverless" subfolder of your clone location. Otherwise, just download the file directly:
https://raw.githubusercontent.com/plzm/gab19/master/serverless/function.proj

You do not need to edit the function.proj file, but feel free to look at it in a text editor. It is formatted as Azure Functions requires, so that the Function will go retrieve the Nuget packages listed in the file, and install them in your Function.

Make sure you have the "Logs" tab expanded before continuing, so you can watch what happens.

On the right of the code window, find the "Files" tab and expand it.

![](images/azfn6.png?raw=true)

Click the "Upload" button.

![](images/azfn7.png?raw=true)

Specify the function.proj file you uploaded just above. Now, watch on the Logs tab as Azure Functions installs the Nuget library/libraries specified in the function.proj file. You should see a lot of blue text scroll by, similar to the next screenshot.

![](images/azfn8.png?raw=true)

When the restore process completes, you should see a success message similar to the next screenshot.

![](images/azfn9.png?raw=true)

Almost done with Azure Function configuration!

Next, remember how you created an Azure Function with a Queue Trigger, above? That connected your function to the queue you created - in fact, at this point your function is already "listening" to that queue, but nothing has come in yet.

What you still need to do, though, is connect your Azure Function to an OUTPUT. In other words, after your function receives an incoming message on your queue (and processes it, which you'll do below), your function still needs to save the results somewhere.

To do this, you'll now connect your function to the Blob Storage container you created above. This is where your function will save text analytics results.

In your function window, find the "Integrate" link on the left and click it.

![](images/azfn10.png?raw=true)

As you'll see on the Integrate page, your queue trigger is already shown. You need to add an output. Find the "+ New Output" button on the page and click it.

![](images/azfn11.png?raw=true)

Find "Azure Blob Storage" and click it, then click "Select" at the bottom.

![](images/azfn12.png?raw=true)

Now you need to configure this new Blob Storage output.

You can leave the "Blob Parameter Name" at its default. Optionally, you can change it, but make sure it is a variable name that is acceptable in C#. If you don't know what this means, leave the default in place.

**IMPORTANT** you MUST change the value for "Path" (see screenshot). Start with the Blob Storage container you created above. Here, we will use a Function capability to give each output file a filename of a random GUID (Globally Unique Identifier) and the .json file extension. I used a container name of "messages", so the exact value for "Path" in my case is:
`messages/{rand-guid}.json`
You should substitute your container name for "messages" in that value.

Then click "Save".

![](images/azfn13.png?raw=true)

Next, return to your function code window by clicking your function's name in the left nav bar.

![](images/azfn14.png?raw=true)

Now you're ready for some coding!

### Azure Function Code

Your Azure Function code will need to do the following steps:
- Prepare the information to connect to the Cognitive Services API you deployed above
- Connect to the API, and send the message from the queue to the API
- Retrieve the text analysis from the API
- Write the text analysis result to an output file in the Blob Storage output you just created, above
If this is unclear, take another look at the architecture diagram at the top of this document.

If you cloned the repo, you will have a "run.csx" file in the "serverless" folder. Open that file in a text editor. Copy the entire contents of the file, and paste it into the Azure Function code window. **Overwrite everything that is currently in the code window.**

Alternately, you can bring up the code file directly in your browser by following this link:
https://raw.githubusercontent.com/plzm/gab19/master/serverless/run.csx
Again, simply copy the enire contents of this file and replace the code in your Function window.

Your function code window should now look like the next screenshot.

![](images/azfn15.png?raw=true)

Before you continue, you now need to provide the API key from the Cognitive Service you created earlier. Look on line 18: you will need to replace the text of `YOUR_KEY_HERE` with this value.

Go back to your Resource Group Overview. Find your Cognitive Service resource and click on it. Find the "Keys" tab and click on it. Now, copy either Key to your clipboard - the next screenshot indicates Key 1, but you can use either.

![](images/cog_svc_key.png?raw=true)

Now go back to your function code and replace `YOUR_KEY_HERE` with the key value you just copied from your Cognitive Service.

Now, open the "Logs" tab at the bottom of your window, then click the "Save" button at the top of the window. If everything went well, your Logs tab should show a "Compilation successful" message. If not, troubleshoot the errors as needed. If you are not a C# developer, you may need some help fixing code errors here.

![](images/azfn16.png?raw=true)

### Test Your Azure Function

Now it's time to test your function!

Remember how you opened the "Files" tab on the right of your code window previously? This time, open the "Test" tab in the same area.

![](images/azfn17.png?raw=true)

In the test window, paste any text you'd like, then click "Run" at the bottom. This will send your test text to the function for processing, and write out a new file to the Blob Storage container you designated earlier.

(If you can't think of test text, try the following...)


![](images/azfn18.png?raw=true)

Look for a message in the "Logs" tab that shows the result of your test. Hopefully, you will see a success message! If not, address it and re-test until you get a success message from your test.

![](images/azfn19.png?raw=true)

Now, navigate back to your Resource Group overview. Find the Storage Account you deployed earlier in this lab and click on it.

![](images/rg_storage.png?raw=true)

In the storage account, click on "Blobs".

![](images/storage_blob.png?raw=true)

Find the container you created earlier and click on it.

![](images/storage_blob2.png?raw=true)

You should now see a list of GUID-named JSON files - one for each test (or queue message) that happened in your function.

![](images/storage_blob3.png?raw=true)

You can click on any one of the files shown, then on "Edit blob" to see its contents. If you see JSON with text analytics results - your Azure Function works! Note that you can download the files too. Explore the output some and see what was returned for the text you tested.

![](images/storage_blob4.png?raw=true)

_Tip_: to work with Azure Storage using a desktop tool, download Azure Storage Explorer at https://storageexplorer.com.

### Use the input Queue

So far, you have tested your Azure Function using the code editor's Test tab. Now, let's run an end-to-end test by sending a message to the queue - this is similer to a "real world" scenario where a workload (outside of the context of this workshop) would be sending messages to an ingest point like your queue.

Navigate back to your storage account, but this time click on "Queues" instead of "Blobs". Find the queue you created earlier and click on it.

![](images/storage_q.png?raw=true)

On the queue page, click the "+ Add message" button.

![](images/storage_q2.png?raw=true)

Put some text in the "Message text" textbox, then click "OK".

![](images/storage_q3.png?raw=true)

The message will now be shown in your queue page for a few seconds. Try clicking "Refresh"; the message should disappear very quickly, once your Azure Function picks it up and processes it.

![](images/storage_q4.png?raw=true)

Finally, navigate back to your storage account's "Blobs" area and confirm that a new JSON file has been generated for the message you just enqueued.

That's it! Your text analytics pipeline is now operating end to end.

### Cleanup

When you're all done testing and queueing, clean up by deleting your Resource Group. This will delete all resources you deployed into the Resource Group, as well as the Resource Group itself.

![](images/rg_delete.png?raw=true)

You can also delete individual resources one by one.

### Additional Resources

- Azure Functions documentation: https://docs.microsoft.com/azure/azure-functions/
- Azure Storage documentation: https://docs.microsoft.com/azure/storage/
- Cognitive Services overview: https://azure.microsoft.com/services/cognitive-services/
- Cognitive Services documentation: https://docs.microsoft.com/azure/cognitive-services/
- Text Analytics API docs: https://docs.microsoft.com/dotnet/api/microsoft.azure.cognitiveservices.language.textanalytics?view=azure-dotnet
- Text Analytics API console: https://eastus.dev.cognitive.microsoft.com/docs/services/TextAnalytics-v2-1
- Azure Storage Explorer: https://storageexplorer.com
