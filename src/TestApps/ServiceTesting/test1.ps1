$accountName = ".\krzys"
$secPassword = ConvertTo-SecureString 'prezI"$7?' -AsPlainText -Force

$serviceCredential = New-Object System.Management.Automation.PSCredential ($accountName, $secPassword)
$binaryPath = "C:\devmf\verity\current\master\data-mgmt-agent\AMM\Main\Agents\GoogleDrive\GoogleDriveProcessor\bin\Debug\GoogleDriveProcessor.exe"
$params = @{
    Name           = "MicroFocusFAS-GoogleDriveProcessor"
    BinaryPathName = $binaryPath
    DisplayName    = "Micro Focus FAS-Google Drive Processor"
    StartupType    = "Manual"
    Description    = "Processes Google Drive items for Micro Focus FAS"
    Credential     = $serviceCredential
}

