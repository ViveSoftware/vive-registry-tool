# VIVE Registry Tool

VIVE Registry Tool lets your project access packages from VIVE by editing the `Packages/manifest.json` in your project, adding VIVE registry as a [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html).

After VIVE registry has been added, you can install packages from VIVE in Unity package manager, including:

- VIVE Input Utility
- Wave XR Plugin

Learn more about [VIVE registry](https://developer.vive.com/resources/knowledgebase/vive-registry/).

## Requirements

- Unity 2019.1 or newer

## Installation

Download the `*.unitypackage` from one of the following sources:

- [Unity Asset Store]()
- [GitHub](https://github.com/ViveSoftware/vive-registry-tool/releases)

Open your project, click `Assets/Import Package/Custom Package` in the menu, or drag the `*.unitypackage` onto your Unity editor, to import it.

![](https://i.imgur.com/vF0OsZf.png)

After importing, the following window will show up automatically.

![](https://i.imgur.com/nhjZmuv.png)

After adding the URL, Unity package manager will be automatically opened. Wait for it to refresh the package list then you’re ready to discover the packages from VIVE.

### Showing Preview Packages

If you want to see packages in preview, remember to check `Show preview packages` for the package manager.

#### Before 2020.1

![](https://i.imgur.com/kx4dcyv.png)

#### 2020.1 and Newer 

![](https://i.imgur.com/Noks4Dq.png)

![](https://i.imgur.com/PgzoF7Z.png)

### Switching to My Registries

Only in Unity 2020.1 and newer, packages from scoped registries will be listed in another place called `My Registries`.

![](https://i.imgur.com/gAWlUAF.png)

## Contact

- [Reporting Issues](https://github.com/ViveSoftware/vive-registry-tool/issues)
- email: ViveSoftware@htc.com 