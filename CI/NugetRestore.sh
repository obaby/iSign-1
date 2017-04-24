sln="$1"
curl -s -O https://dist.nuget.org/win-x86-commandline/v3.3.0/nuget.exe
mono nuget.exe restore "$sln"