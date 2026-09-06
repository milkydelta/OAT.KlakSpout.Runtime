# KlakSpout for OnAirTap

I took a few different versions of KlakSpout and rearranged the C# code so that they can all be used in the same way.

2.0.6 is the version previously included with OnAirTap. It supports Direct3D 11 and Direct3D 12. The minimum "supported" Unity version is 2022.3, but I've used it on Unity 2019 with no problems. As a **hard** minimum, this requires net472. It will not work on games running net35.

0.1.3 is the latest version of KlakSpout to officially support Unity 5. It is only usable with Direct3D 11. This one also worked on 2019.

v1 is from the "v1" branch of the KlakSpout repo. I've not actually tested it, and I'm not sure what it's for, but the minimum "supported" version is... Unity 2019.


## Installation

Download the version that you need.

Place the managed (`OAT.Klak.Spout.Runtime`) DLL in `BepInEx/plugins` or `UserLibs`, depending on your mod loader.

Place the unmanaged (`OAT_KlakSpout`) DLL in `<game name>_Data/Plugins`.
