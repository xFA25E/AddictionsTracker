{
  buildDotnetModule,
  dotnetCorePackages,
  fetchFromGitHub,
  runCommand,
  src,
  writeText,
}: let
  parseVersionScript = writeText "parse-version-script.awk" ''
    $1 == "##" && $2 != "[Unreleased]" {
        gsub("(^\\[|\\]$)", "", $2);
        printf "%s", $2;
        exit;
    }
  '';
  version = builtins.readFile (runCommand "get-version" {} ''
    awk -f ${parseVersionScript} <"${src}/CHANGELOG.md" >$out
  '');
in
  buildDotnetModule {
    inherit src version;
    pname = "CSharpLanguageServer";

    projectFile = "src/csharp-language-server.sln";
    nugetDeps = ./nugetDeps.nix;

    dotnet-sdk = dotnetCorePackages.sdk_7_0;
    dotnet-runtime = dotnetCorePackages.runtime_7_0;
  }
