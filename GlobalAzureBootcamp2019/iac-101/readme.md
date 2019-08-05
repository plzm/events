# Global Azure Bootcamp 2019 - Manchester, NH

## Infrastructure as Code 101

## Summary

The Azure portal makes it easy to deploy many different types of resources and workloads. But doing the same (or similar) things manually, over and over again, is tiresome and error-prone.

In this workshop, you will learn how to script and templatize a complete infrastructure deployment that includes these Azure resources:

- A Resource Group
- A Network Security Group (NSG)
- A Virtual Network (VNet)
- A Subnet
- A Virtual Machine (VM)

## Level / Pre-Requisites / Time

Level: Intermediate

Pre-Requisites:
- Basic understanding of Javascript Object Notation (JSON) and Powershell or bash will be helpful, but is not mandatory.
- Azure subscription. If you don't have one, you can get started at https://azure.com/free.

Time: 60 minutes. Plus - all instructions/files are in this public github repo, so you can keep working on this after the session.

## Overview

Azure Resource Manager (ARM) is the deployment and management service for Azure. Its management layer enables deployment, organization, and security for resources like the ones you will deploy in this workshop.
Reference: https://docs.microsoft.com/azure/azure-resource-manager/

ARM Templates are documents that describe resources you want to deploy. You specify information including resource types, parameters, variables, dependencies, and outputs. Then you tell ARM to run your template(s).
Reference: https://docs.microsoft.com/azure/templates/

You can create a single template that describes multiple resources. You can also create multiple templates, one per resource - this offers more flexibility later. Today, you'll create a single template for everything you'll deploy to Azure.

Templates often separate the generic resource definition(s) - like VM, network, storage, and so on - from the exact configuration parameters for a specific deployment. For example, your generic resource definition might lay out a VM, but your parameters are where you'd specify the VM's size. This way, you could change your parameters as needed without changing the generic definition itself.

You can create your own templates from scratch, but this is very challenging. These ways are much easier:
- Use the Azure portal to deploy resources manually. Then, download an automatically-generated template from the Azure portal, and customize that to your needs. (Some people think of this as Azure's "File-Save As" feature.) See https://docs.microsoft.com/azure/azure-resource-manager/resource-manager-quickstart-create-templates-use-the-portal for details.
- Use templates from others and edit/customize them. See Additional Resources, below, for links to many ARM templates, and see https://docs.microsoft.com/azure/azure-resource-manager/resource-manager-quickstart-create-templates-use-visual-studio-code for a walk-through of this in Visual Studio Code.

In this workshop, you'll use two files. Both are in the same github repo/folder as the file you're reading right now.
1. azuredeploy.template.json is the file that specifies the Azure resources to deploy, and how to deploy them
2. azuredeploy.parameters.json is the parameters file that provides values for a specific deployment

## Resources

As you read above, in this workshop you'll deploy several resources.

#### Virtual Machine

Documentation: https://docs.microsoft.com/azure/virtual-machines/

Working with VMs on Azure, once they're deployed, is similar to working with them elsewhere. For example, you can use Remote Desktop Protocol (RDP) to sign onto Windows VMs running on Azure, and Secure Shell (SSH) to sign onto Linux VMs.

When preparing an ARM template for a VM, we have a few decisions to make, such as which operating system to use, various specifications like number of CPU cores, how much RAM, how much storage, and so on. All these are exposed in the template for this workshop.

In Azure, VM components like Network Interface Cards (NICs) are first-class, standalone resources that you can deploy, configure, and manage separately from the VM. In this workshop, as part of the template you will deploy a NIC and set the VM to use it.

#### Network Security Group

Documentation: https://docs.microsoft.com/azure/virtual-network/security-overview

NSGs let you filter network traffic to, and from, your Azure resources. NSGs contain a set of rules, each of which tells Azure how to handle the kind of network traffic covered by the rule. For example, an NSG rule might say drop (refuse) all inbound traffic from anywhere on the public internet.

The NSG for this workshop will allow traffic from your local machine to your Azure resources, but will reject all other traffic from outside Azure.

#### Virtual Network and Subnet

Documentation: https://docs.microsoft.com/azure/virtual-network/

VNets in Azure are networks. Each VNet has an IP address range. Within the VNet, subnets can be defined. Each subnet can have its own NSG, as well as other security and traffic control features. This is much like traditional TCP/IP networking, but now networking is software-defined.

In this workshop, you'll deploy a simple VNet with a single subnet. The subnet will be protected by the NSG you create. The VM you create will be in this subnet.

## Tasks

(finally!)

### Log into the Azure Portal

In your web browser, go to https://portal.azure.com and sign in using your existing Azure credentials, or an Azure Pass if you were provided one, or using the credential you used to get started at https://azure.com/free.

### Prepare the Azure Command-Line Interface

Reference: https://docs.microsoft.com/cli/azure/

The Azure Command-Line Interface (CLI) lets you run commands against Azure at a command prompt. You can run the Azure CLI either on your local computer, or in the Azure portal's Cloud Shell. In this short workshop, you'll use the Cloud Shell.

In your web browser in the Azure Portal, find the top toolbar and click on the "Cloud Shell" button.

![](images/cloud_shell.png?raw=true)

If this is your first time using the Cloud Shell, you will be asked whether to use Bash or Powershell. For this lab, either is fine (and you can always switch) so pick either. If you're totally unsure - pick Bash.

You may then be asked to provide storage for the Cloud Shell - if so, just click "Create storage" unless you have an existing storage resource you'd like to use. This may take a few seconds to a minute or so to complete.

When done, you will be at a command prompt. Note the toolbar above the prompt, which includes an "Upload/Download files" button. Locate that, as you will need it soon.

![](images/cloud_shell_prompt.png?raw=true)

_Note: if you prefer, you can download and install the Azure CLI locally at https://docs.microsoft.com/cli/azure/install-azure-cli. In this workshop, you're using the portal's Cloud Shell since local CLI installation can take several minutes. Note that if you work in the local CLI, you will need to login to Azure using the command "az login". You do not need to do this in the Cloud Shell in the Azure portal._

###### Useful Commands

Try typing `az --version` (then hit Enter) for your first Azure CLI command.

Type `az --help` for, well, help.

### Create a Resource Group

First, you'll create a Resource Group. Decide on a good, short name (it can be simple and descriptive, like "gab19-rg"). Note it - you'll need it below. You'll also need to settle on an Azure region to use; for this workshop, use the East US region by specifying "eastus".

Now, go to the Cloud Shell.

Tip: before running other Azure CLI commands, try typing `az interactive`. This will add interactivity (suggestions) as you type further AZ CLI commands.

At the Azure CLI, run the following command to create a new Resource Group where you will deploy your resources.

`az group create --location eastus --name gab19-rg`

As you see, I'm specifying the "eastus" Azure region (location), and the name "gab19-rg" for my Resource Group. After hitting Enter, you should see a success message.

![](images/rg_create.png?raw=true)

If you go to "Resource groups" in the portal and click "Refresh", you should now see your new Resource Group.

![](images/rg_list.png?raw=true)

### Prepare a good Text Editor

A good text editor will be very useful for ARM Template work. There are many, and if you have one that you like, feel free to use it for this workshop.

I recommend Visual Studio Code (VS Code, https://code.visualstudio.com) with the following extensions for Azure and for code productivity:
- Azure Account - see https://marketplace.visualstudio.com/items?itemName=ms-vscode.azure-account
- Azure CLI Tools - see https://marketplace.visualstudio.com/items?itemName=ms-vscode.azurecli
- Azure Resource Manager Tools - see https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools
- Bracket Pair Colorizer 2 - see https://marketplace.visualstudio.com/items?itemName=CoenraadS.bracket-pair-colorizer-2
- JSON Tools - see https://marketplace.visualstudio.com/items?itemName=eriklynd.json-tools

Please download and install VS Code (use the "User Installer" if prompted), followed by the above extensions.

### Get the starter ARM Template files into your Text Editor

For this workshop, both starter (for you to work in) and completed (to use as a reference/guide) files are provided for you. Here, you will download them and open them in your text editor.

There are two ways to get these files onto your computer.

1. The easiest way: download the files directly into a new folder on your computer.
    - https://raw.githubusercontent.com/plzm/gab19/master/iac-101/azuredeploy.template.json
    - https://raw.githubusercontent.com/plzm/gab19/master/iac-101/azuredeploy.parameters.json
    - https://raw.githubusercontent.com/plzm/gab19/master/iac-101/azuredeploy.template.complete.json
    - https://raw.githubusercontent.com/plzm/gab19/master/iac-101/azuredeploy.parameters.complete.json
2. If you are familiar with git and github, you can either clone this repo (https://github.com/plzm/gab19) to your machine, or fork it to your own github account. Then go into the "iac-101" folder. _If you don't know what any of this means - don't worry, for this workshop just download the files directly using the two links directly above._

When you have the files on your computer, start VS Code and open the folder location. You should see all the files in the VS Code file list. If you're using another text editor, open all of the .json files in the "iac-101" folder for editing.

Review the azuredeploy.template.json and azuredeploy.parameters.json files.

The parameters file is a simple list of key-value pairs. For each key (like `virtualMachineName`) you'll provide a value that the deployment will use with the corresponding places in the template which need that value. This shows how you can change your deployments (e.g. to deploy a different template name, or to a different Azure region, etc.) just by editing the simple parameters file, not the more complex template file.

Some specifics you can review to see how these files are structured:
- Note how each parameter in the .parameters file also has an entry in the .template file. Note the differences between what can be provided in each file type. See how much more (including description and default value) can be provided in the template file.
- Note that the .template file has a "variables" section. Here, you can use parameters and ARM template language functions to dynamically prepare values to use in your deployment that you may not be able to pass as a fixed parameter value.
- In the .template file, look at each resource you'll deploy. (If your text editor supports it, try collapsing areas of the file to focus on one block of JSON at a time.) Do you see similarities - and differences based on the resource type?

### Prepare the template for deployment

Now, it's time for you to prepare your actual deployment. Since you've been provided starter files for this workshop, you only need to prepare the azuredeploy.parameters.json file. The azuredeploy.template.json file is ready to go, as is - but after your first successful deployment, try experimenting with it! Maybe try different VM sizes... different operating systems... explore how to add more disks than just the single operating system (OS) disk used in this simple workshop... look at the sample templates in Additional Resources.

Ensure you are editing "azuredeploy.parameters.json" in your text editor. You need to provide values for all the empty value fields. To do this, you can:
- Use the helpful tips and links provided in the .template file in many of the parameter descriptions, especially for parameters like publisher, offer, and sku
- Refer to the "azuredeploy.parameters.complete.json" file and see how that was filled out
- Use the above documentation links
- Ask the presenter or proctor

Some helpful tips for specific parameters:
- location: use the value `eastus` for today - this is the Azure East US region
- virtualMachineSize: until your first deployment succeeds, try a small/inexpensive size like `Standard_B1s`. You can upsize later.
- adminUsername: do not use `Administrator` (if you're deploying a Windows VM). Use 
- adminPassword must meet complexity rules, or your deployment will error out. Use a password that's at least 12 characters and contains a mix of upper- and lower-case, symbols, and numbers.
- osDiskStorageType: use `Premium_LRS` for this today. This is fast SSD storage.
- osDiskSizeInGB: use `128` for this. This is a safe value for a wide variety of VM sizes.
- virtualNetworkAddressSpace: if you're not very familiar with TCP/IP networking, use `192.168.0.0/16` for this. If your presenter is available, feel free to ask what this and the subnet address space mean, if you're curious. If you _are_ good with TCP/IP, then you can set any RFC1024 address space here, just leave room to carve out subnets. Don't go any smaller than a /24.
- subnetAddressSpace: see previous note, and use `192.168.1.0/24` for this.
- networkSecurityGroupRule_Source: this is your IP address, as seen from the internet. A very easy way to retrieve this is by going to https://bing.com?q=what+is+my+ip. Use the value shown there. Ask your presenter or proctor if you get stuck.
- vmTimeZone: Use `Eastern Standard Time`.

Once you have filled out all values in azuredeploy.parameters.json and saved the file, continue to the next step.

### Upload the files to Azure

Go back to the Cloud Shell in the Azure portal. Now, click the "Upload/Download files" button mentioned above. Upload both the azuredeploy.template.json and azuredeploy.parameters.json files. Optionally, run `ls` at the Cloud Shell prompt to verify the files uploaded.

### Test the deployment

Before you deploy anything, validate the files. You can do this with the following command (remember, optionally you can first type `az interactive` to get suggestions as you type):

`az group deployment validate --resource-group gab19-rg --template-file azuredeploy.template.json --parameters azuredeploy.parameters.json`

If any errors occur, try to fix them in your local files. Then, re-upload the fixed files and run this validation command again until no errors are found. If you get stuck, ask your presenter or proctor for help.

### Deploy

(Finally!)

Now it's time to deploy! Type the following command and hit Enter.

`az group deployment create --resource-group gab19-rg --template-file azuredeploy.template.complete.json --parameters azuredeploy.parameters.complete.json`

The first time you deploy, it will take a little longer as various Azure resource providers are registered in your subscription. Expect the deployment to take a few minutes the first time, and less time on later deploys.

If any errors happen during the deployment (the previous validation step doesn't catch every possible error), inspect the error messages and fix your files accordingly, then re-upload and try again. As usual, ask for help if you get stuck.

You can monitor a deployment's status while it's running. In the portal, go to your resource group

When your deployment completes, you will be returned to a prompt at the Cloud Shell. Now it's time to connect to your VM!

### Connect to your VM

In the Azure portal, navigate into your Resource Group. Then click on your Virtual machine.

![](images/vm_in_rg.png?raw=true)

On the VM Overview, click the "Connect" button. For a Windows VM, this will let you download a Remote Desktop Protocol (RDP) file. Download it to your computer, then double-click it. If you deployed a Linux VM, note the SSH connection info and use that instead.

![](images/vm_connect_rdp.png?raw=true)

If all went well, you will be prompted for your credentials and you will log into a running VM on Azure! Now picture deploying 100 very similar VMs... with this approach, you can script 100 VM deploys. Doing it manually for each distinct VM would take days!

![](images/vm_connected_rdp.png?raw=true)

### Wrap-up

This is a very simple workshop to get a basic, but complete, VM deployment running. There's a lot we have not covered, including high availability options, additional VM disks, and much more.

If you succeeded in getting your VM running and connecting to it - great job! If not - remember this github repo is public and will be available after the event.

### Cleanup and Cost Management

We did not configure VM Auto-Shutdown in this workshop. This means that the VM will stay running, accumulating cost. To conclude this workshop, clean up your resources. You can delete individual resources, but the easiest way to clean up is to delete the Resource Group you created for this workshop; that will automatically also delete all resources in the Resource Group.

To delete a Resource Group, either:
1. Use this Azure CLI command at the Cloud Shell: 
2. Or navigate to the Resource Group and click the "Delete resource group" button and provide information/confirm as prompted.

![](images/rg_delete.png?raw=true)

## Additional Resources

My repo full of ARM templates, scripts, and related artifacts: https://github.com/plzm/azure/tree/master/arm/
Microsoft repo full of ARM templates: https://github.com/Azure/azure-quickstart-templates/
