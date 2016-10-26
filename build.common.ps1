$Script:UseWriteHost = $true

if(!$Global:ColorScheme) {
    $Global:ColorScheme = @{
        "Banner"=[ConsoleColor]::Cyan
        "RuntimeName"=[ConsoleColor]::Yellow
        "Help_Header"=[ConsoleColor]::Yellow
        "Help_Switch"=[ConsoleColor]::Green
        "Help_Argument"=[ConsoleColor]::Cyan
        "Help_Optional"=[ConsoleColor]::Gray
        "Help_Command"=[ConsoleColor]::DarkYellow
        "Help_Executable"=[ConsoleColor]::DarkYellow
        "ParameterName"=[ConsoleColor]::Cyan
        "Warning" = [ConsoleColor]::Yellow
    }
}

function _WriteOut {
    param(
        [Parameter(Mandatory=$false, Position=0, ValueFromPipeline=$true)][string]$msg,
        [Parameter(Mandatory=$false)][ConsoleColor]$ForegroundColor,
        [Parameter(Mandatory=$false)][ConsoleColor]$BackgroundColor,
        [Parameter(Mandatory=$false)][switch]$NoNewLine)

    if($__TestWriteTo) {
        $cur = Get-Variable -Name $__TestWriteTo -ValueOnly -Scope Global -ErrorAction SilentlyContinue
        $val = $cur + "$msg"
        if(!$NoNewLine) {
            $val += [Environment]::NewLine
        }
        Set-Variable -Name $__TestWriteTo -Value $val -Scope Global -Force
        return
    }

    if(!$Script:UseWriteHost) {
        if(!$msg) {
            $msg = ""
        }
        if($NoNewLine) {
            [Console]::Write($msg)
        } else {
            [Console]::WriteLine($msg)
        }
    }
    else {
        try {
            if(!$ForegroundColor) {
                $ForegroundColor = $host.UI.RawUI.ForegroundColor
            }
            if(!$BackgroundColor) {
                $BackgroundColor = $host.UI.RawUI.BackgroundColor
            }

            Write-Host $msg -ForegroundColor:$ForegroundColor -BackgroundColor:$BackgroundColor -NoNewLine:$NoNewLine
        } catch {
            $Script:UseWriteHost = $false
            _WriteOut $msg
        }
    }
}

function _WriteConfig{
param(
        [Parameter(Mandatory=$true,Position=0)]$name,
        [Parameter(Mandatory=$true,Position=1)]$value)
		
	_WriteOut -NoNewline -ForegroundColor $Global:ColorScheme.ParameterName "${name}: "
	_WriteOut "$value"

}

function _DownloadNuget{
param(
        [Parameter(Mandatory=$true,Position=0)]$rootPath)

$sourceNugetExe = "http://nuget.org/nuget.exe"
$targetNugetExe = "$rootPath\nuget.exe"


if(!(Test-Path $targetNugetExe )){
    _WriteOut "Downloading nuget to $targetNugetExe"
    Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
}
else{
    # _WriteOut "nuget.exe is already present"
}

Set-Alias nuget $targetNugetExe -Scope Global

}