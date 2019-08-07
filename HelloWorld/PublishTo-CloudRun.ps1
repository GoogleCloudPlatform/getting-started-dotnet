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

# Build the application locally.
dotnet publish -c Release

# Collect some details about the project that we'll need later.
$projectId = gcloud config get-value project
$region = 'us-central1'

# Use Google Cloud Build to build a Docker container and publish to Google
# Container Registry. 
gcloud builds submit --tag gcr.io/$projectId/hello-world `
    bin/Release/netcoreapp2.0/publish

# Run the container with Google Cloud Run.
gcloud beta run deploy hello-world --region $region --platform managed `
    --image gcr.io/$projectId/hello-world --allow-unauthenticated

