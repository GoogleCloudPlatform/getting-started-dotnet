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

##############################################################################
#.SYNOPSIS
# Runs all the gcloud commands necessary to run this sample.  Includes:
# 1. Creating KMS keyring
# 2. Creating KMS key
# 3. Giving permission to the App Engine service account to encrypt and
#    decrypt with the key.
# 4. Creating a Google Cloud Storage bucket.
# 5. Updating appsettings.json with names of key ring, key, and bucket.
# 
#.OUTPUTS
# Log of success and failure. 
#
#.EXAMPLE
# .\Set-Up.ps1
##############################################################################

Param ([string]$keyRingId = 'dataprotectionprovider', [string]$keyId = 'masterkey',
    [string]$bucketName, [string]$projectId)

Import-Module .\SetUp.psm1

$projectId = if ($projectId) { $projectId } else { gcloud config get-value project }
$bucketName = if ($bucketName) { $bucketName } else { "$projectId-bucket" }

# Check to see if the key ring already exists.
$matchingKeyRing = (gcloud kms keyrings list --format json --location global `
    --filter="projects/$projectId/locations/global/keyRings/$keyRingId" | ConvertFrom-Json).name
if ($matchingKeyRing) {
    Write-Host "The key ring $matchingKeyRing already exists."
} else { 
    # Create the new key ring.
    Write-Host "Creating new key ring $keyRingId..." 
    gcloud kms keyrings create $keyRingId --location global
}

# Check to see if the key already exists
$keyName = "projects/$projectId/locations/global/keyRings/$keyRingId/cryptoKeys/$keyId"
$matchingKey = (gcloud kms keys list --format json --location global `
    --keyring $keyRingId --filter="$keyName" | ConvertFrom-Json).name
if ($matchingKey) {
    Write-Host "The key $matchingKey already exists."
} else { 
    # Create the new key.
    Write-Host "Creating new key $keyId..."
    gcloud kms keys create $keyId --location global --keyring $keyRingId --purpose=encryption
}

# Check to see if the bucket already exists.
$matchingBucket = (gsutil ls -b gs://$bucketName) 2> $null
if ($matchingBucket) {
    Write-Host "The bucket $bucketName already exists."
} else {
    # Create the bucket.
    gsutil mb -p $projectId gs://$bucketName
}

Update-Appsettings $keyName $bucketName

