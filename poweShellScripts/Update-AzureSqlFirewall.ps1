# ============================================
# Update Azure SQL Firewall with user-provided IP
# Run from project root: ./Update-AzureSqlFirewall.ps1
# ============================================

# Configuration
$resourceGroup = "DevPulseRG"
$sqlServerName = "devpulsedbserver"
$ruleName      = "ClientIPAddress"

# Prompt user for IP
$publicIp = Read-Host "Enter the IPv4 address shown in SSMS error message"

# Basic validation
if (-not ($publicIp -match '^\d{1,3}(\.\d{1,3}){3}$')) {
    Write-Host "Invalid IPv4 format. Please enter something like 122.105.196.222"
    exit
}

Write-Host "Using IP: $publicIp"

# Update firewall rule
Write-Host "Updating Azure SQL firewall rule '$ruleName'..."
az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $sqlServerName `
    --name $ruleName `
    --start-ip-address $publicIp `
    --end-ip-address $publicIp | Out-Null

Write-Host "Firewall rule updated successfully."

# Show rules
Write-Host "`nCurrent firewall rules for server '$sqlServerName':"
az sql server firewall-rule list `
    --resource-group $resourceGroup `
    --server $sqlServerName `
    --output table