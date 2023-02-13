{
  CSharpLanguageServer,
  alejandra,
  dos2unix,
  dotnetCorePackages,
  dotnetPackages,
  lib,
  libICE,
  libSM,
  libSkiaSharp,
  libX11,
  libxml2,
  mkShell,
  sqlite,
  statix,
}: let
  libraryPath = lib.makeLibraryPath [libX11 libICE libSM libSkiaSharp];
in
  mkShell {
    packages = [
      CSharpLanguageServer
      alejandra
      dos2unix
      dotnetCorePackages.sdk_7_0
      dotnetPackages.Nuget
      libxml2.bin
      sqlite
      statix
    ];

    shellHook = ''
      export LD_LIBRARY_PATH=${libraryPath}''${LD_LIBRARY_PATH:+:}$LD_LIBRARY_PATH
    '';
  }
