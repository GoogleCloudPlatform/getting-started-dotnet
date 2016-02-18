# Copyright(c) 2015 Google Inc.
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

# Recursively retrieve all the files in a directory that match one of the
# masks.
function GetFiles($path = $pwd, [string[]]$masks = '*', $maxDepth = 0, $depth=-1)
{
    foreach ($item in Get-ChildItem $path)
    {
        if ($masks | Where {$item -like $_})
        {
            $item
        }
        if ($maxDepth -ge 0 -and $depth -ge $maxDepth)
        {
            # We have reached the max depth.  Do not recurse.
        }
        elseif (Test-Path $item.FullName -PathType Container)
        {
            GetFiles $item.FullName $masks $maxDepth ($depth + 1)
        }
    }
}

# Run inner runTests.ps1 scripts.
filter RunTestScript {
    Set-Location $_.Directory
    echo $_.FullName
    Invoke-Expression (".\" + $_.Name)
    $LASTEXITCODE
}

##############################################################################
# aspnet-core tests.

dnvm use 1.0.0-rc1-update1 -r clr

# Given a *test.js file, build the project and run the test on localhost.
filter BuildAndRunLocalTest {
    dnu restore
    dnu build
    $webProcess = Start-Process dnx web -PassThru
    Try
    {
        Start-Sleep -Seconds 4  # Wait for web process to start up.
        casperjs $_ http://localhost:5000
        $LASTEXITCODE
    }
    Finally
    {
        Stop-Process $webProcess
    }
}

##############################################################################
# aspnet tests.
cd aspnet
$env:GETTING_STARTED_DOTNET = pwd
$env:APPLICATIONHOST_CONFIG =  Get-ChildItem .\applicationhost.config
nuget restore
msbuild
cd ..

# Given the name of a website in our ./applicationhost.config, return its port number.
function GetPortNumber($sitename) {
    $node = Select-Xml -Path $env:APPLICATIONHOST_CONFIG `
        -XPath "/configuration/system.applicationHost/sites/site[@name='$sitename']/bindings/binding" | 
        Select-Object -ExpandProperty Node
    $chunks = $node.bindingInformation -split ':'
    $chunks[1]
}

# Run the the website, as configured in our ./applicationhost.config file.
function RunIISExpress($sitename) {
    $argList = ('/config:"' + $env:APPLICATIONHOST_CONFIG + '"'), ("/site:" + $sitename), "/apppool:Clr4IntegratedAppPool"
    Start-Process iisexpress.exe  -ArgumentList $argList -PassThru
}

# Run the website, then run the test javascript file with casper.
# Called by inner runTests.
function RunIISExpressTest($sitename = '', $testjs = 'test.js') {
    if ($sitename -eq '') 
    {
        $sitename = (get-item -Path ".\").Name
    }
    $port = GetPortNumber $sitename
    $webProcess = RunIISExpress $sitename
    Try
    {
        Start-Sleep -Seconds 4  # Wait for web process to start up.
        casperjs $testjs http://localhost:$port
        $LASTEXITCODE
    }
    Finally
    {
        Stop-Process $webProcess
    }
}

##############################################################################
# main
# Leave the user in the same directory as they started.
$originalDir = Get-Location
Try
{
    # Use Where-Object to avoid infinitely recursing, because this script
    # matches the mask.
    GetFiles -masks '*runtests.ps1' -maxDepth 2 | Where-Object FullName -ne $PSCommandPath | RunTestScript
}
Finally
{
    Set-Location $originalDir
}
