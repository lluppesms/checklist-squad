// --------------------------------------------------------------------------------
// Private Networking Module
// Creates the VNet with subnets and Private DNS Zone for SQL
// Only deployed when enablePrivateNetworking is true (controlled by caller)
// --------------------------------------------------------------------------------

@description('Name of the Virtual Network.')
param vnetName string

@description('Azure region for resources.')
param location string

@description('Tags to apply to all resources.')
param tags object

@description('VNet address space (e.g., 10.0.0.0/16).')
param vnetAddressPrefix string

@description('Subnet prefix for Web App VNet integration.')
param webAppSubnetPrefix string

@description('Subnet prefix for private endpoints.')
param privateEndpointSubnetPrefix string

@description('Deployment suffix for unique deployment names.')
param deploymentSuffix string

// --------------------------------------------------------------------------------
// Virtual Network (AVM)
// --------------------------------------------------------------------------------
module vnetModule 'br/public:avm/res/network/virtual-network:0.8.1' = {
  name: 'vnet${deploymentSuffix}'
  params: {
    name: vnetName
    location: location
    tags: tags
    addressPrefixes: [
      vnetAddressPrefix
    ]
    subnets: [
      {
        name: 'snet-webapp'
        addressPrefix: webAppSubnetPrefix
        delegation: 'Microsoft.Web/serverFarms'
      }
      {
        name: 'snet-pe'
        addressPrefix: privateEndpointSubnetPrefix
      }
    ]
  }
}

// --------------------------------------------------------------------------------
// Private DNS Zones (AVM)
// --------------------------------------------------------------------------------
module sqlDnsZoneModule 'br/public:avm/res/network/private-dns-zone:0.8.1' = {
  name: 'sqlDnsZone${deploymentSuffix}'
  params: {
    name: 'privatelink${environment().suffixes.sqlServerHostname}'
    tags: tags
    virtualNetworkLinks: [
      {
        virtualNetworkResourceId: vnetModule.outputs.resourceId
        registrationEnabled: false
      }
    ]
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
output vnetResourceId string = vnetModule.outputs.resourceId
output webAppSubnetResourceId string = vnetModule.outputs.subnetResourceIds[0]  // snet-webapp
output peSubnetResourceId string = vnetModule.outputs.subnetResourceIds[1]      // snet-pe
output sqlDnsZoneResourceId string = sqlDnsZoneModule.outputs.resourceId
