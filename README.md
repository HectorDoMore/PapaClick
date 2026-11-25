# PapaClick

一个轻量的 Windows 连点器，基于 C# / .NET 8 开发。

## 说明

- 本项目在 AI（WorkBuddy）协助下制作。
- 桌面程序版与命令行版共用同一套核心引擎（PapaClick.Core）。
- 全局热键：**F7 开始/停止切换**（程序在后台也响应）。
- 点击间隔以秒为单位，最小 0.001 秒；引擎在连点期间自动启用高精度计时。

## 项目结构

```
PapaClick/
├── PapaClick.sln
└── src/
    ├── PapaClick.Core/   ← 连点核心引擎（类库，两版共用）
    ├── PapaClick.CLI/    ← 命令行版（开发中）
    └── PapaClick.GUI/    ← 桌面程序版（WinForms）
```

## 环境要求

- Windows 10/11
- .NET 8 SDK

## 构建

```bash
dotnet build PapaClick.sln
```

## 命令行版用法

```
PapaClick.CLI --interval <秒> --button <left|right> --mode <single|double>
```

示例：`PapaClick.CLI --interval 0.05 --button right`。运行中按 Enter 停止，点击发生在当前光标位置。

## 许可证

本项目基于 [GPL-3.0](LICENSE) 协议开源。
