build:
	dotnet publish -o builds/ -r linux-x64 -p:PublishSingleFile=true --self-contained true
	rm builds/*.pdb

build-win:
	dotnet publish -o builds/ -r win-x64 -p:PublishSingleFile=true --self-contained true
	rm builds/*.pdb

run:
	dotnet run

clean:
	dotnet clean