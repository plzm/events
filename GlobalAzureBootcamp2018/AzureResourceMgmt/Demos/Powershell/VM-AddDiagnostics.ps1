﻿# ##############################
# Purpose: Add diagnostics to an existing RM VM
#
# Author: Patrick El-Azem
# ##############################

# Arguments with defaults
param
(
	[string]$SubscriptionId = '',
    [string]$Location = 'East US',
	[string]$ResourceGroupNameVM = '',
	[string]$ResourceGroupNameStorageAccountDiagnostics = '',
	[string]$StorageAccountNameDiagnostics = '',
    [string]$StorageAccountSkuNameDiagnostics = 'Standard_LRS',
	[string]$VMName = '',
	[string]$DiagnosticsXMLFilePath = ((Get-Location).Path + '\VM-Diagnostics.xml'),
    [string]$AppInsightsInstrumentationKey = ''
)

$diagnosticsXMLFilePathSource = $DiagnosticsXMLFilePath
$diagnosticsXMLFilePathForVM = ((Get-Location).Path + '\VM-Diagnostics-' + $VMName + '.xml')

$tokenSubscriptionId = '###SUBSCRIPTIONID###'
$tokenResourceGroupNameVM = '###RGNAME###'
$tokenVMName = '###VMNAME###'
$tokenAppInsightsKey = '###APPIKEY###'

$configXml = [string](Get-Content -Path $diagnosticsXMLFilePathSource)
$configXml = $configXml.Replace($tokenSubscriptionId, $SubscriptionId)
$configXMl = $configXml.Replace($tokenResourceGroupNameVM, $ResourceGroupNameVM)
$configXMl = $configXml.Replace($tokenVMName, $VMName)
$configXMl = $configXml.Replace($tokenAppInsightsKey, $AppInsightsInstrumentationKey)

New-Item -Path $diagnosticsXMLFilePathForVM -Value $configXMl -ItemType File -Force

# Ensure diagnostics storage account exists
$sa = .\StorageAccount-CreateGet.ps1 -ResourceGroupName $ResourceGroupNameStorageAccountDiagnostics -Location $Location -StorageAccountName $StorageAccountNameDiagnostics -StorageAccountSkuName $StorageAccountSkuNameDiagnostics

# Set boot diagnostics
$vm = Get-AzureRmVm -ResourceGroupName $ResourceGroupNameVM -Name $VMName

Set-AzureRmVMBootDiagnostics -Enable -ResourceGroupName $ResourceGroupNameStorageAccountDiagnostics -VM $vm -StorageAccountName $StorageAccountNameDiagnostics | Out-Null

Update-AzureRmVM -ResourceGroupName $ResourceGroupNameVM -VM $vm

# Set guest OS diagnostics
Set-AzureRmVMDiagnosticsExtension `
	-ResourceGroupName $ResourceGroupNameVM `
	-VMName $VMName `
	-StorageAccountName $StorageAccountNameDiagnostics `
	-DiagnosticsConfigurationPath $diagnosticsXMLFilePathForVM `
	-AutoUpgradeMinorVersion $true

Remove-Item -Path $diagnosticsXMLFilePathForVM -Force