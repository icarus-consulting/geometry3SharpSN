[CmdletBinding()]
param(
    [Parameter(Position = 0, ValueFromRemainingArguments = $true)]
    [string[]] $BuildArguments
)

$ErrorActionPreference = "Stop"
$BuildProjectFile = Join-Path $PSScriptRoot "build\_build.csproj"

dotnet build $BuildProjectFile --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

dotnet run --project $BuildProjectFile --no-build -- --root $PSScriptRoot $BuildArguments
exit $LASTEXITCODE