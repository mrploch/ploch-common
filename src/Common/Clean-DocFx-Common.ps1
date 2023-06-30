cd $PSScriptRoot
Remove-Item _site -Force
Remove-Item api/*.yml -Exclude toc.yml -Force
Remove-Item api/.manifest -Force

if ($error)
{
    Write-Host
    Write-Host "Last error:"
    Write-Host $error
    Read-Host Completed with errors. Press enter.
}