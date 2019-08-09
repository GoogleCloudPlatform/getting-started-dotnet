# Copyright 2017 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip([string]$zipfile, [string]$outpath)
{
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

# Install dotnet command line.
choco install -y --sxs dotnetcore-sdk --version 2.2.203

# Install codeformatter
dotnet tool install -g dotnet-format

# Lint the code
Push-Location
try {
    Set-Location github\getting-started-dotnet\
    Import-Module .\BuildTools.psm1
    Lint-Code
} finally {
    Pop-Location
}

# Install phantomjs
Unzip $env:KOKORO_GFILE_DIR\phantomjs-2.1.1-windows.zip \
$env:PATH = "$env:PATH;$(Resolve-Path \phantomjs-2.1.1-windows)\bin"

# Install casperjs
Unzip $env:KOKORO_GFILE_DIR\casperjs-1.1.4-1.zip \
$casperJsInstallPath = Resolve-Path \casperjs-1.1.4-1
$env:PATH = "$env:PATH;$casperJsInstallPath\bin"

# Install nuget command line.
choco install nuget.commandline

# Install IISExpress.  Some tests use it.
choco install -y iisexpress
$env:PATH = "$env:PATH;${env:ProgramFiles(x86)}\IIS Express\"


# Run the tests.
github\getting-started-dotnet\.kokoro-windows\main.ps1