// BP Calculator infrastructure.
// Deployed once per environment; prod additionally gets a staging slot so the
// release pipeline can do a VIP swap (blue/green), mirroring the module's
// temperatureconverterCICD reference topology.

targetScope = 'resourceGroup'

@description('Environment this deployment represents.')
@allowed([
  'dev'
  'qa'
  'prod'
])
param environmentName string

@description('Short prefix that makes the globally-unique web app name identifiable.')
@minLength(2)
@maxLength(8)
param namePrefix string

param location string = resourceGroup().location

// F1 is free but cannot host deployment slots, so only prod pays for Standard.
var isProd = environmentName == 'prod'
var appServicePlanSku = isProd ? 'S1' : 'F1'
var appName = '${namePrefix}-bpcalculator-${environmentName}'
var appServicePlanName = '${namePrefix}-bp-plan-${environmentName}'
var logAnalyticsName = '${namePrefix}-bp-logs-${environmentName}'
var appInsightsName = '${namePrefix}-bp-ai-${environmentName}'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// One Application Insights resource per environment, so dev/qa/prod telemetry
// never lands in the same stream.
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    IngestionMode: 'LogAnalytics'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: false
  }
}

var commonAppSettings = [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: appInsights.properties.ConnectionString
  }
  {
    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
    value: '~3'
  }
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: isProd ? 'Production' : 'Staging'
  }
]

resource appService 'Microsoft.Web/sites@2024-11-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: commonAppSettings
    }
  }
}

// Blue slot. Traffic is swapped into production only after the gated checks pass.
resource stagingSlot 'Microsoft.Web/sites/slots@2024-11-01' = if (isProd) {
  parent: appService
  name: 'staging'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: commonAppSettings
    }
  }
}

@description('Public hostname of the environment, used as the target for E2E, ZAP and k6 runs.')
output appHostName string = appService.properties.defaultHostName

@description('Hostname of the staging slot (prod only).')
output stagingSlotHostName string = isProd ? stagingSlot.properties.defaultHostName : ''

output appServiceName string = appService.name
output appInsightsName string = appInsights.name
