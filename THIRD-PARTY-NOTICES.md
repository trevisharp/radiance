# Third party notices

Radiance ships the following third party material.

## Roboto

`Radiance/Fonts/Resources/Roboto-Regular.ttf` is embedded in the Radiance
assembly and is served by `Font.Default`.

 - Copyright 2011 The Roboto Project Authors (https://github.com/googlefonts/roboto-classic)
 - Licensed under the SIL Open Font License, Version 1.1.
 - The full license text is in `Radiance/Fonts/Resources/Roboto-OFL.txt`.

The copy shipped here is a modified version of the font published at
https://github.com/google/fonts/tree/main/ofl/roboto. It was built from
`Roboto[wdth,wght].ttf` by taking the `wght=400, wdth=100` instance and then
subsetting it to Basic Latin, Latin-1 Supplement, Latin Extended-A, general
punctuation and a few symbols, with hinting removed. The font carries no
Reserved Font Name, so the original name is kept.

The commands used were:

```bash
python -m fontTools.varLib.instancer "Roboto[wdth,wght].ttf" wght=400 wdth=100 \
    -o Roboto-Regular-full.ttf

python -m fontTools.subset Roboto-Regular-full.ttf \
    --unicodes="U+0000-00FF,U+0100-017F,U+0192,U+02C6-02DC,U+2000-206F,U+20AC,U+2122,U+2190-2193,U+2212,U+25A0-25CF,U+FFFD" \
    --no-hinting --drop-tables+=DSIG,STAT,GDEF \
    --output-file=Roboto-Regular.ttf
```
