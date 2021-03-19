Param([string] $serviceName, 
      [string] $displayName, 
      [string] $description,
      [string] $binaryPath, 
      $startupType, 
      [string] $accountName, 
      [string] $password)

sc.exe delete $serviceName
$secPassword = ConvertTo-SecureString $password -AsPlainText -Force
$serviceCredential = New-Object System.Management.Automation.PSCredential ($accountName, $secPassword)

New-Service -Name $serviceName -DisplayName $displayName -Description $description -StartupType $startupType -BinaryPathName $binaryPath -Credential $serviceCredential
