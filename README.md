# PapaClick

一个轻量的 Windows 连点器，基于 C# / .NET 8 开发。

## 说明

- 本项目在 AI（WorkBuddy）协助下制作。
- 桌面程序版与命令行版共用同一套核心引擎（PapaClick.Core）。
- 命令行版功能将于后续版本提供支持。

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

## 许可证

本项目基于 [GPL-3.0](LICENSE) 协议开源。
