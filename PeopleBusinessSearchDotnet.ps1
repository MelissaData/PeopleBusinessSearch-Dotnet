<#
.SYNOPSIS
    Builds and runs the Melissa People Business Search Cloud API .NET sample.

.DESCRIPTION
    This script builds PeopleBusinessSearchDotnet with dotnet publish, then runs the
    resulting executable, passing along the license and (if supplied) the search fields.

    Overall flow:
      1. Resolve the license (parameter, prompt, or MD_LICENSE environment variable).
      2. Publish PeopleBusinessSearchDotnet in Release configuration to
         .\PeopleBusinessSearchDotnet\Build.
      3. Run the built executable: one-shot mode if any search field was supplied,
         otherwise interactive mode (the .NET program prompts for each field).

.PARAMETER maxrecords
    Maximum number of records to return in one-shot mode.

.PARAMETER matchlevel
    Match level for the search in one-shot mode.

.PARAMETER addressline1
    Street address to search on in one-shot mode.

.PARAMETER locality
    Locality (city) to search on in one-shot mode.

.PARAMETER administrativearea
    Administrative area (state/province) to search on in one-shot mode.

.PARAMETER postal
    Postal code to search on in one-shot mode.

.PARAMETER anyname
    Individual or company name to search on in one-shot mode.

.PARAMETER license
    License string. Resolved in this order:
      1. This parameter.
      2. An interactive prompt, if the parameter was not supplied.
      3. The MD_LICENSE environment variable, if the prompt was left blank.
    Note that the environment variable is the last resort, not the first: running
    without -license always prompts, even when MD_LICENSE is set.

.PARAMETER quiet
    Accepted for parity with other sample scripts; not currently used to suppress output.

.EXAMPLE
    .\PeopleBusinessSearchDotnet.ps1 -license "your-license"

.EXAMPLE
    .\PeopleBusinessSearchDotnet.ps1 -maxrecords "10" -matchlevel "10" -addressline1 "22382 Avenida Empresa" -locality "Rancho Santa Margarita" -administrativearea "CA" -postal "92688" -anyname "Melissa Data" -license "your-license"
#>

######################### Parameters ##########################
param(
    $maxrecords = '',
    $matchlevel = '',
    $addressline1 = '',
    $locality = '',
    $administrativearea = '',
    $postal = '',
    $anyname = '',
    $license = '',
    [switch]$quiet = $false
    )

# Uses the location of the .ps1 file
$CurrentPath = $PSScriptRoot
Set-Location $CurrentPath
$ProjectPath = "$CurrentPath\PeopleBusinessSearchDotnet"
$BuildPath = "$ProjectPath\Build"

If (!(Test-Path $BuildPath)) {
  New-Item -Path $ProjectPath -Name 'Build' -ItemType "directory"
}

########################## Main ############################
Write-Host "`n================ Melissa People Business Search Cloud API ==================`n"

# Get license (either from parameters or user input)
if ([string]::IsNullOrEmpty($license) ) {
  $license = Read-Host "Please enter your license string"
}

# Check for License from Environment Variables 
if ([string]::IsNullOrEmpty($license) ) {
  $license = $env:MD_LICENSE
}

if ([string]::IsNullOrEmpty($license)) {
  Write-Host "`nLicense String is invalid!"
  Exit
}

# Start program
# Build project
Write-Host "`n=============================== BUILD PROJECT =============================="

dotnet publish -f="net7.0" -c Release -o $BuildPath PeopleBusinessSearchDotnet\PeopleBusinessSearchDotnet.csproj

# Run project
# No search fields supplied -> run interactively; otherwise pass the supplied ones through for one-shot mode.
if ([string]::IsNullOrEmpty($maxrecords) -and [string]::IsNullOrEmpty($matchlevel) -and [string]::IsNullOrEmpty($addressline1) -and [string]::IsNullOrEmpty($locality) -and [string]::IsNullOrEmpty($administrativearea) -and [string]::IsNullOrEmpty($postal) -and [string]::IsNullOrEmpty($anyname)) {
  dotnet $BuildPath\PeopleBusinessSearchDotnet.dll --license $license
}
else {
  # Only pass flags that have a value. Windows PowerShell drops empty-string arguments to
  # native programs, which would shift the next flag name into this flag's value.
  # Any field left out here is prompted for by the program.
  $runArgs = @('--license', $license)
  if (-not [string]::IsNullOrEmpty($maxrecords))         { $runArgs += '--maxrecords', $maxrecords }
  if (-not [string]::IsNullOrEmpty($matchlevel))         { $runArgs += '--matchlevel', $matchlevel }
  if (-not [string]::IsNullOrEmpty($addressline1))       { $runArgs += '--addressline1', $addressline1 }
  if (-not [string]::IsNullOrEmpty($locality))           { $runArgs += '--locality', $locality }
  if (-not [string]::IsNullOrEmpty($administrativearea)) { $runArgs += '--administrativearea', $administrativearea }
  if (-not [string]::IsNullOrEmpty($postal))             { $runArgs += '--postal', $postal }
  if (-not [string]::IsNullOrEmpty($anyname))            { $runArgs += '--anyname', $anyname }
  dotnet $BuildPath\PeopleBusinessSearchDotnet.dll @runArgs
}
