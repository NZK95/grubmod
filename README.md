# grubmod
[![Downloads](https://img.shields.io/github/downloads/NZK95/grubmod/total.svg)](https://github.com/NZK95/grubmod/releases)

> ### Disclaimer
> The author is not responsible for any possible damage caused to hardware as a result of using this project. <br>
> This software does not guarantee any increase in performance and is intended for enthusiasts only. <br>
> You use this program at your own risk. <br>

By default, BIOS/UEFI only shows users a small part of its variables (settings). Most of them are hidden, and the standard interface doesn’t display them. These hidden options control everything — from memory timings to CPU power parameters.  
**BIOS/UEFI variables** are key–value pairs stored in non-volatile memory (NVRAM). Manufacturers usually lock access to these variables.

**grubmod** is a tool that allows you to view and modify hidden BIOS variables directly through the GRUB bootloader using a convenient graphical interface.  
It automatically generates a valid script with the desired changes and applies it following the given instructions.

## Known Issues
- As far as known, it doesn’t work on AMD systems.
- Some parameters don’t have all possible values.


## Features
- **Displays all BIOS/UEFI variables, including hidden ones, along with their data.**
- **Convenient filters for viewing and analyzing parameters.**
- **Modify variables and apply changes through GRUB.**
- **Import and export configs.**
- **Built-in presets with various configurations.**
- **Script generation and execution for automating changes.**
- **Error tracking via logging.**


## Usage

### 1. Preparing working files
1. Download the latest release of `grubmod`.
2. Download the **current** version of your BIOS firmware and place it in your working folder.
3. Open the firmware file in `UEFIToolNE` in the `files` folder.
4. In the window that opens, press `Ctrl + F`.
5. Select the `Text` section and type any parameter name, for example: `ASPM`.

<p align="left">
  <img src="https://github.com/NZK95/grubmod/blob/master/docs/Usage/%231.png?raw=true">
</p>

6. After the search, go to the `Setup/PE32 image section` entry.
7. Right-click the `PE32 image section` in the `Setup` section.
8. Select **Extract as is.**

<p align="left">
  <img src="https://github.com/NZK95/grubmod/blob/master/docs/Usage/%232.png?raw=true">
</p>

9. Save the file in `files` folder.
10. Open CLI in the `files` folder, and insert the command:
```
ifrextractor.exe "extracted_file_name.sct"
```
11. Open the saved file in grubmod using the **Import file** button.



---

### 3. Preparing the USB drive
1. Format your USB drive as `FAT32`.
2. Download a UEFI shell file from one of these sources:
   - [UEFI-Shell](https://github.com/pbatard/UEFI-Shell/releases/latest) (`shellx64.efi`)
   - Official [EDK2 stable releases](https://github.com/tianocore/edk2/releases/download/edk2-stable202002/ShellBinPkg.zip) (`ShellBinPkg > UefiShell > X64 > Shell.efi`) — may be outdated, not recommended.
3. Rename the downloaded UEFI file to `BOOTX64.EFI` and place it at:
   `USB:\EFI\BOOT`
4. Place `setup_var.efi` and `setupvar-script.nsh` in the root of the USB drive.


### 4. Script execution
1. Boot from the USB drive via your motherboard’s boot menu (check your model’s key combo online).
2. In the UEFI shell, identify your USB drive:
   - Check the displayed **Mapping Table** (to re-display it, type `map`). Most likely, it’ll be `FS0`.
3. Run the script with:
   ```bash
   .\setupvar-script.nsh
   ```
4. Wait until the script finishes execution.
5. Power off your PC using the physical power button to apply the changes.

---

### Config Syntax
If you want to create/load a custom config, follow this syntax:  
If a parameter is missing or incorrect, it will be ignored.
```bash
PARAMETER NAME | DESIRED VALUE
```
If you want a max value:
```bash
PARAMETER NAME | Max
```
The config file must have a `.txt` extension, and the filename must contain the word `config`.  
Examples: `my-config.txt`, `Configtest.txt`.


### Keyboard shortcuts
```
Enter     - Search
Ctrl + C  - Clear
Ctrl + G  - Google
Ctrl + E  - Export script
Ctrl + L  - Load config
```

## Credits
- [UEFI-Editor by BoringBoredom](https://github.com/BoringBoredom/UEFI-Editor?tab=readme-ov-file#how-to-change-hidden-settings-without-flashing-a-modded-bios)
- [grub-mod-setup_var by datasone](https://github.com/datasone/grub-mod-setup_var)
- [setup_var.efi](https://github.com/datasone/setup_var.efi?tab=readme-ov-file)
- [UEFI](https://github.com/LongSoft/UEFITool#known-issues)




## Troubleshooting & Support
If you encounter errors or bugs, please report them via the [issue tracker](https://github.com/NZK95/grubmod/issues).<br>
Sometimes, after changing BIOS parameters, the system might not boot.
To reset BIOS to defaults, clear the CMOS or remove the battery.
