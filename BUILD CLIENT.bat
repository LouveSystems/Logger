cd LouveSystems.Logger
dotnet publish -c Release -f net6.0
mkdir ..\Release\net6.0
copy bin\Release\net6.0\publish\LouveSystems.Logger.dll ..\Release\net6.0\.
dotnet publish -c Release -f net472
mkdir ..\Release\net472
copy bin\Release\net472\publish\LouveSystems.Logger.dll ..\Release\net472\.
PAUSE