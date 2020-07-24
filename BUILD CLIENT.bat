cd LouveSystems.Logger
dotnet publish -c Release -f netcoreapp3.0
mkdir ..\Release\netcoreapp3.0
copy bin\Release\netcoreapp3.0\publish\LouveSystems.Logger.dll ..\Release\netcoreapp3.0\.
dotnet publish -c Release -f net471
mkdir ..\Release\net471
copy bin\Release\net471\publish\LouveSystems.Logger.dll ..\Release\net471\.
PAUSE