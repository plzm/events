﻿# ##############################
# Purpose: Delete selected resources in a resource group based on name prefix.
# NOTE... if you leave the prefix empty, be prepared for total destruction in your RG :) You have been warned!
#
# Author: Patrick El-Azem
# ##############################

# Arguments with defaults
param
(
    [string]$SubscriptionId = '[FILL IN]',
    [string]$ResourceGroupName = '[FILL IN]',
    [string]$NamePrefixToDelete = '[FILL IN]'
)

# Delete VMs first to remove any leases
Write-Host 'Removing VMs'

$vms = .\ResourceGroup-GetResources.ps1 -SubscriptionId $SubscriptionId -ResourceGroupName $ResourceGroupName | where {$_.Name -like ($NamePrefixToDelete + '*') -and $_.ResourceType -eq 'Microsoft.Compute/virtualMachines'}

$vms | foreach {Write-Host ('Removing ' + $_.Name + ' | ' + $_.ResourceId); Remove-AzureRmResource -ResourceId $_.ResourceId -Force}

# Delete other resources
Write-Host 'Removing other resources'

$resources = .\ResourceGroup-GetResources.ps1 -SubscriptionId $SubscriptionId -ResourceGroupName $ResourceGroupName | where {$_.Name -like ($NamePrefixToDelete + '*')}

$resources | foreach {Write-Host ('Removing ' + $_.Name + ' | ' + $_.ResourceId); Remove-AzureRmResource -ResourceId $_.ResourceId -Force}
