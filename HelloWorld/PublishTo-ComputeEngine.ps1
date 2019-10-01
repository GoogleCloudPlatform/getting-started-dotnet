# Copyright (c) 2019 Google LLC.
# 
# Licensed under the Apache License, Version 2.0 (the "License"); you may not
# use this file except in compliance with the License. You may obtain a copy of
# the License at
# 
# http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
# WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
# License for the specific language governing permissions and limitations under
# the License.

<#
.SYNOPSIS
Deploys HelloWorld to an IIS instance running on Google Compute Engine.

.PARAMETER ComputeEngineInstanceIpAddress
The public IP address of your compute engine instance.  Optional.

.PARAMETER ComputeEngineInstanceUserName
The username to use when authenticating with your compute engine instance.  Optional.

.EXAMPLE
.\PublishTo-ComputeEngine.ps1
Enter your Compute Engine instance's public IP address: 1.2.3.4
Enter username for 1.2.3.4: admin
Enter password for 1.2.3.4\admin: *******

#>


Param([string]$ComputeEngineInstanceIpAddress, [string]$ComputeEngineInstanceUserName)

$pubxmlPath = (Get-Item 'Properties\PublishProfiles\ComputeEngine.pubxml').FullName
$pubxml = [xml] (Get-Content $pubxmlPath)

# Retrieve the Compute Engine instance's IP address.
$ip = if ($ComputeEngineInstanceIpAddress) { 
    $ComputeEngineInstanceIpAddress
} else { 
    $pubxml.Project.PropertyGroup.MSDeployServiceURL 
}
if ([String]::IsNullOrWhiteSpace($ip)) {
    $ip = Read-Host -Prompt "Enter your Compute Engine instance's public IP address"
}

# Retrieve the username.
$username = if ($ComputeEngineInstanceUserName) { 
    $ComputeEngineInstanceUserName 
} else { 
    $pubxml.Project.PropertyGroup.UserName 
}
if ([String]::IsNullOrWhiteSpace($username)) {
    $username = Read-Host -Prompt "Enter username for $ip"
}

# Save the username and ip address.
$pubxml.Project.PropertyGroup.MSDeployServiceURL = "$ip"        
$pubxml.Project.PropertyGroup.SiteUrlToLaunchAfterPublish = "http://$ip/"
$pubxml.Project.PropertyGroup.UserName = "$username"
$pubxml.Save($pubxmlPath)

# Ask for the password.
$securePassword = Read-Host -Prompt "Enter password for ${ip}\$username" -AsSecureString
$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList "$username", $securePassword
$password = $credential.GetNetworkCredential().password
# [START getting_started_dotnet_publish]
# Publish it!
dotnet publish -c Release `
    /p:PublishProfile=Properties\PublishProfiles\ComputeEngine.pubxml `
    "/p:Password=$password"
# [END getting_started_dotnet_publish]
