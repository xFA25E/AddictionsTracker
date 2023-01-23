{
  alejandra,
  csharp-ls,
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
  statix,
  sqlite,
}: let
  libraryPath = lib.makeLibraryPath [libX11 libICE libSM libSkiaSharp];
in
  mkShell {
    packages = [
      alejandra
      csharp-ls
      dos2unix
      dotnetCorePackages.sdk_7_0
      dotnetPackages.Nuget
      libxml2.bin
      statix
      sqlite
    ];

    shellHook = ''
      export LD_LIBRARY_PATH=${libraryPath}:$LD_LIBRARY_PATH
    '';
  }
