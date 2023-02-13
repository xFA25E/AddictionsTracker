inputs: {
  default = final: prev: {
    addictionsTracker = final.callPackage ./addictions-tracker {};
    libSkiaSharp = final.callPackage ./lib-skia-sharp.nix {
      angle2Src = inputs.angle2;
      piexSrc = inputs.piex;
      skiaSharpSrc = inputs.skiaSharp;
      src = inputs.skia;
    };
  };
  CSharpLanguageServer = final: prev: {
    CSharpLanguageServer = final.callPackage ./csharp-language-server {
      src = inputs.CSharpLanguageServer;
    };
  };
}
