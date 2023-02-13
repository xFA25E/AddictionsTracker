{
  buildDotnetModule,
  dotnetCorePackages,
  libICE,
  libSM,
  libSkiaSharp,
  libX11,
}: let
  version = "0.0.1";
in
  buildDotnetModule {
    inherit version;
    pname = "AddictionsTracker";

    src = ./../../../src;

    projectFile = "AddictionsTracker.csproj";
    nugetDeps = ./nugetDeps.nix;

    dotnet-sdk = dotnetCorePackages.sdk_7_0;
    dotnet-runtime = dotnetCorePackages.runtime_7_0;

    runtimeDeps = [libX11 libICE libSM libSkiaSharp];
  }
