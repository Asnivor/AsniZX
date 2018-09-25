## AsniZX - ZX Spectrum Emulation 
### (or standing on the shoulders of giants)

#NOTE: THIS IS NO LONGER MAINTAINED, WORKING, OR INDEED ANY GOOD. IT WILL BE DELETED SOON

AsniZX is a work-in-progress first attempt at learning to write an emulator. Until there is actually a release I would stay well away :)

#### Current State

* Machines emulated: Spectrum 48k
* CPU: YES
* Display: YES
* Memory: YES
* Memory contention/timings: UNKNOWN
* Sound: NO
* Keyboard: NO
* Tape: NO
* Loading of snapshot formats: NO
* Peripherals: NO

As can be seen, there is still a ways to go. I hope to eventually be able to support all popular spectrum emulation formats, all ZX Spectrum models (and clones) and peripherals.

#### Attribution & References

The Z80 emulator (as well as some ideas on c# interface implementation and pretty much a direct copy of the main emulation loop and ULA screen timing code) have been taken from Istvan Novak's amazing ZX Spectrum IDE with VS2017 integration.
He is doing some amazing work and I encourage everyone to check out his project:

* Spectnetide - https://github.com/Dotneteer/spectnetide/

I am also pouring over the source for the following excellent emulators in order to try and work out how better people than me have done things previously:

* Zero - https://github.com/ArjunNair/Zero-Emulator/
* ZX Spin - https://sites.google.com/site/pauldunn/ZXSpin_Source_Luca.zip
* BizHawk - https://github.com/TASVideos/BizHawk
* Emulator.NES - https://github.com/Xyene/Emulator.NES/tree/master/dotNES

And during my slow, ardious learning process I have been crying whilst looking at the following resources:

* http://www.worldofspectrum.org/faq/reference/reference.htm
* http://www.zxdesign.info
* http://codersbucket.blogspot.co.uk/2015/04/interrupts-on-zx-spectrum-what-are.html
* http://www.z80.info
* http://www.breakintoprogram.co.uk/
