msbuild /m /p:Configuration=Release Castle.Windsor.Lifestyles.sln
.nuget\nuget.exe pack Castle.Windsor.Lifestyles\Castle.Windsor.Lifestyles.csproj -Properties Configuration=Release
