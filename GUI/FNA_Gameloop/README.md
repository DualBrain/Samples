# FNA-XNA

See: https://fna-xna.github.io/

This project requires a reference to the FNA project located at:

https://github.com/FNA-XNA/FNA

Be sure to use the `Core` project.

NOTE: I modified the `<TargetFrameworks>netstandard2.0;net5.0</TargetFrameworks>` to be `net7.0` as `net5.0` is not on my machine and Visual Studio "complains". Note that it will still work if you ignore the complaint, but I just don't like the noise.

It also requires a several additional DLLs (included) to either be with the
produced executable or in the appropriate DLL path(s).

You can find the latest versions of these at: https://fna.flibitijibibo.com/archive/fnalibs.tar.bz2

## More

- [FNA WASM](https://github.com/wattsyart/fna-wasm)
- [FNA WASM Builds](https://github.com/clarvalon/FNA-WASM-Build/actions)
