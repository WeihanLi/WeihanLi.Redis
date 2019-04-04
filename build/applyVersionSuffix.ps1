# Enable -Verbose option
[CmdletBinding()]

# Regular expression pattern to find the version in the build number 
# and then apply it to the assemblies
$VersionSuffixRegex = "<VersionSuffix(.*)>(.*)</VersionSuffix>"

$VersionSuffix = "preview-$((Get-Date).ToUniversalTime().ToString("yyyyMMdd-HHmmss"))"

if("$env:BUILD_SOURCEBRANCHNAME" -eq "master"){
    $VersionSuffix = ""
}

Write-Verbose "VersionSuffix: $VersionSuffix"

$file = Get-Item("$Env:BUILD_SOURCESDIRECTORY/build/version.props")
$filecontent = Get-Content($file)
$filecontent -replace $VersionSuffixRegex, "<VersionSuffix>$VersionSuffix</VersionSuffix>" | Out-File $file

Write-Verbose "$file.FullName - version applied"