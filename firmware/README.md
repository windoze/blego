Firmware for BBC Micro:bit
==========================

This firmware should be used with this library.

The firmware enables following services:
----------------------------------------

1. Accelerometer Service
2. Button Service
3. Device Information Service
4. Event Service
5. IO Pin Service
6. LED Service
7. Temperature Service

The Magnetometer Service has been disabled as there is a [bug in microbit-dal library](https://github.com/lancaster-university/microbit-dal/issues/262), the program may crash on calibration.

The paring has been disabled as the library currently doesn't support BLE pairing.

You can customize the firmware by modifing the source code under the `src` directory and generate the `.hex` file with [Yotta](https://lancaster-university.github.io/microbit-docs/offline-toolchains) or [online IDE](https://ide.mbed.com/compiler/) provided by [Mbed](https://os.mbed.com/).
