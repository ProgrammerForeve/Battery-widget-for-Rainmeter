
# Rainmeter Battery Plugin

A custom Rainmeter plugin that provides various battery-related information, such as battery percentage, status, charge status, and battery details (name, ID). This plugin retrieves battery information using WMI (Windows Management Instrumentation) and is compatible with Rainmeter skins.

## Features

- **Battery Percentage**: Displays the current battery percentage as a decimal (e.g., 75%).
- **Battery Status**: Displays the current status of the battery (e.g., High, Low, Charging).
- **Battery Status (Numeric)**: Displays the battery status in a numeric format (e.g., `0` for Unknown, `1` for High, `2` for Low, `3` for Critical, `4` for Charging).
- **Battery Charge Status**: Displays whether the battery is charging or discharging.
- **Battery Charge Status (Numeric)**: Displays `1` if charging, `0` if discharging.
- **Battery Name**: Displays the battery's real name retrieved via WMI.
- **Battery ID**: Displays the battery's real ID retrieved via WMI.

## Installation

### Requirements
- [Rainmeter](https://www.rainmeter.net/) should be installed.
- Ensure you have .NET Framework installed (the plugin requires .NET for WMI access).

### How to Install
1. Download the latest release of the plugin) or clone this repository.
2. Compile the C# code into a DLL using Visual Studio (or use the precompiled DLL).
3. Place the compiled `.dll` file in the `Plugins` folder of your Rainmeter installation (usually located in `Documents\Rainmeter\Plugins`).
4. Create a Rainmeter skin that references the plugin. See the example skin configuration below for details.

## Usage

### Example Skin

Here’s an example `.ini` file that displays the battery details from the plugin:

```ini
[Rainmeter]
Update=1000
DynamicWindowSize=1
AccurateText=1

[Variables]
FontColor=255,255,255,255
FontSize=12
FontFace=Arial

; Measure for Battery Percentage
[MeasureBatteryPercentage]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryPercentage

; Measure for Battery Status as text (e.g., High, Low, Charging, etc.)
[MeasureBatteryStatus]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryStatus

; Measure for Battery Status in Numeric form
[MeasureBatteryStatusNumeric]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryStatusNumeric

; Measure for Charge Status as text (e.g., Charging, Discharging)
[MeasureBatteryChargeStatus]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryChargeStatus

; Measure for Charge Status in Numeric form (1 if Charging, 0 otherwise)
[MeasureBatteryChargeStatusNumeric]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryChargeStatusNumeric

; Measure for Battery Name
[MeasureBatteryName]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryName

; Measure for Battery ID
[MeasureBatteryID]
Measure=Plugin
Plugin=PluginBattery
MeasureType=BatteryID

; Display Battery Percentage
[BatteryPercentageDisplay]
Meter=String
MeasureName=MeasureBatteryPercentage
Text=Battery Percentage: %1%
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=10

; Display Battery Status as Text
[BatteryStatusDisplay]
Meter=String
MeasureName=MeasureBatteryStatus
Text=Battery Status: %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=40

; Display Battery Status as Numeric
[BatteryStatusNumericDisplay]
Meter=String
MeasureName=MeasureBatteryStatusNumeric
Text=Battery Status (Numeric): %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=70

; Display Charge Status as Text
[BatteryChargeStatusDisplay]
Meter=String
MeasureName=MeasureBatteryChargeStatus
Text=Charge Status: %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=100

; Display Charge Status as Numeric
[BatteryChargeStatusNumericDisplay]
Meter=String
MeasureName=MeasureBatteryChargeStatusNumeric
Text=Charge Status (Numeric): %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=130

; Display Battery Name
[BatteryNameDisplay]
Meter=String
MeasureName=MeasureBatteryName
Text=Battery Name: %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=160

; Display Battery ID
[BatteryIDDisplay]
Meter=String
MeasureName=MeasureBatteryID
Text=Battery ID: %1
FontSize=#FontSize#
FontColor=#FontColor#
FontFace=#FontFace#
X=10
Y=190
```

### Skin Details
- The skin retrieves various battery-related information from the plugin.
- The skin updates every second (`Update=1000`), but you can adjust the frequency as needed.
- The `FontColor`, `FontSize`, and `FontFace` can be easily customized via the `[Variables]` section.

## Contributing

Feel free to fork the repository, make changes, and submit pull requests! Any improvements, bug fixes, or additional features are welcome.

### License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Troubleshooting

- **No Battery Detected**: Ensure your system has a battery or check if WMI is accessible.
- **Unknown Battery Information**: The plugin may return `"Unknown"` if it cannot find any battery information from WMI.

## Credits

- This plugin uses WMI (Windows Management Instrumentation) for battery data retrieval.
- Special thanks to [Rainmeter](https://www.rainmeter.net/) for creating the Rainmeter platform.

## Contact
If you have any questions or suggestions, feel free to open an issue .


