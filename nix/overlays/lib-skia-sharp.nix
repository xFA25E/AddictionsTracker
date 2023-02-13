{
  angle2Src,
  expat,
  fetchFromGitHub,
  fetchgit,
  fontconfig,
  gn,
  icu,
  libX11,
  libglvnd,
  libjpeg,
  libpng,
  libwebp,
  mesa,
  ninja,
  piexSrc,
  python2,
  runCommand,
  skiaSharpSrc,
  src,
  stdenv,
  writeText,
  zlib,
}: let
  versionScript = "${skiaSharpSrc}/native/linux/libSkiaSharp/libSkiaSharp.map";

  parseSonameScript = writeText "parse-soname-script.awk" ''
    $1 == "libSkiaSharp" && $2 == "soname" {
        printf "%s", $3;
        exit;
    }
  '';

  parseVersionScript = writeText "parse-version-script.awk" ''
    $1 == "SkiaSharp" && $2 == "file" {
        printf "%s", $3;
        exit;
    }
  '';

  soname = builtins.readFile (runCommand "get-soname" {} ''
    awk -f ${parseSonameScript} <"${skiaSharpSrc}/VERSIONS.txt" >$out
  '');

  version = builtins.readFile (runCommand "get-version" {} ''
    awk -f ${parseVersionScript} <"${skiaSharpSrc}/VERSIONS.txt" >$out
  '');
in
  stdenv.mkDerivation {
    inherit src version;
    pname = "libSkiaSharp";

    nativeBuildInputs = [gn ninja python2];

    buildInputs = [
      expat
      fontconfig
      icu
      libX11
      libglvnd
      libjpeg
      libpng
      libwebp
      mesa
      zlib
    ];

    preConfigure = ''
      mkdir -p third_party/externals
      ln -s ${angle2Src} third_party/externals/angle2
      ln -s ${piexSrc} third_party/externals/piex
    '';

    configurePhase = ''
      runHook preConfigure
      gn gen out/linux/x64 --args='
        extra_asmflags=[]
        extra_cflags=[ "-DSKIA_C_DLL", "-DHAVE_SYSCALL_GETRANDOM", "-DXML_DEV_URANDOM" ]
        extra_ldflags=[ "-static-libstdc++", "-static-libgcc", "-Wl,--version-script=${versionScript}" ]
        is_official_build=true
        linux_soname_version="${soname}"
        skia_enable_gpu=true
        skia_enable_skottie=true
        skia_enable_tools=false
        skia_use_dng_sdk=false
        skia_use_icu=false
        skia_use_piex=true
        skia_use_sfntly=false
        skia_use_vulkan=false
        target_cpu="x64"
        target_os="linux"
      '
      runHook postConfigure
    '';

    buildPhase = ''
      runHook preBuild
      ninja -C out/linux/x64 SkiaSharp
      runHook postBuild
    '';

    installPhase = ''
      mkdir -p $out/lib
      cp out/linux/x64/libSkiaSharp.so.${soname}  $out/lib
      ln -s $out/lib/libSkiaSharp.so.${soname} $out/lib/libSkiaSharp.so
    '';
  }
