{
  outputs = {
    self,
    flake-utils,
    nixpkgs,
  }:
    {
      overlays = {
        default = final: prev: {
          addictions-tracker = final.callPackage ./nix/addictions-tracker {};
          libSkiaSharp = final.callPackage ./nix/libSkiaSharp.nix {};
        };
        csharp-ls = final: prev: {
          csharp-ls = final.callPackage ./nix/csharp-ls {};
        };
      };
    }
    // flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {
        inherit system;
        overlays = [self.overlays.csharp-ls self.overlays.default];
      };
    in {
      devShells.default = pkgs.callPackage ./nix/shell.nix {};
      packages = {inherit (pkgs) addictions-tracker csharp-ls libSkiaSharp;};
    });
}
