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

dnvm use 1.0.0-rc1-final -r clr

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

filter RunTestScript {
    Set-Location $_.Directory
    Invoke-Expression $_.FullName
    $LASTEXITCODE
}

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
