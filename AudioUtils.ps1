$ModuleCheck = Get-Module -ListAvailable -Name AudioDeviceCmdlets
if (-Not $ModuleCheck) {
    Install-Module -Name AudioDeviceCmdlets
} 

Import-Module -Name AudioDeviceCmdlets

function Get-PlaybackDevices {
    Get-AudioDevice -List | Where-Object {$_.Type -eq "Playback"}
}

function Set-PlaybackDevice {
    param(
        [string]$DeviceId
    )

    Set-AudioDevice -ID $DeviceId -DefaultOnly
}