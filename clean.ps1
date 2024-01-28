dotnet nuget locals all --clear
Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Remove-Item .\artifacts\ -Force -Recurse
