# HTWebRemoteHost
#### Simple remote control of your home theater devices and HTPC from any web browser

HTWebRemoteHost is the companion app to [HTWebRemote](https://github.com/nicko88/HTWebRemote).

HTWebRemoteHost is a headless service, and you must use the [HTWebRemote](https://github.com/nicko88/HTWebRemote) Windows App to create and modify your remote controls, and then "Sync" them to this HTWebRemoteHost service on your RasPi / Linux PC.

### Raspberry Pi / Linux Installation

This install script is intended to install HTWebRemoteHost on RasPi or standard Linux running a Debian-based distribution using systemd.  It may work on other distributions but it has not been tested.  You can also download the Linux release and install it manually onto your particular Linux machine.

This script will ask to install HTWebRemoteHost and also additionally ADB (Android Debug Bridge) which is needed to control an Nvidia Shield over the network.
#### Install
    sudo wget https://raw.githubusercontent.com/nicko88/HTWebRemoteHost/master/install/install.sh && sudo bash install.sh
#### Update
The primary method for updating the HTWebRemoteHost app is to use the update button located inside HTWebRemote's "Manage Remote Host" screen. Or if you like, you can run the update script manually here:

    sudo wget https://raw.githubusercontent.com/nicko88/HTWebRemoteHost/master/install/update.sh && sudo bash update.sh
#### Uninstall
    sudo wget https://raw.githubusercontent.com/nicko88/HTWebRemoteHost/master/install/uninstall.sh && sudo bash uninstall.sh

#### Note about controlling an Nvidia Shield

In order to control an Nvidia Shield from HTWebRemoteHost, you will likely need to first control it via remote buttons on Windows with [HTWebRemote](https://github.com/nicko88/HTWebRemote).  Then use the "Transfer NVShield Authorization" option found in the "Tools" menu in the "Manage Remote Host" screen inside the HTWebRemoteWindows App.