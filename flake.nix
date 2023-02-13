{
  inputs = {
    CSharpLanguageServer.flake = false;
    CSharpLanguageServer.url = "github:razzmatazz/csharp-language-server/0.6.0";

    angle2.flake = false;
    angle2.url = "git+https://chromium.googlesource.com/angle/angle.git";

    piex.flake = false;
    piex.url = "git+https://android.googlesource.com/platform/external/piex.git";

    skia.flake = false;
    skia.url = "github:mono/skia/v2.88.3";

    skiaSharp.flake = false;
    skiaSharp.url = "github:mono/SkiaSharp/v2.88.3";
  };

  outputs = {
    self,
    flake-utils,
    nixpkgs,
    ...
  } @ inputs:
    {
      overlays = import ./nix/overlays inputs;
    }
    // flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {
        inherit system;
        overlays = [self.overlays.CSharpLanguageServer self.overlays.default];
      };
    in {
      devShells.default = pkgs.callPackage ./nix/shell.nix {};
      packages = {
        inherit (pkgs) CSharpLanguageServer addictions-tracker libSkiaSharp;
      };
    });
}
