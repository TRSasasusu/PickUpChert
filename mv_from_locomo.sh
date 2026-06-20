#!/bin/bash

if [ -e ../TravelersLocomotionPack/TravelersLocomotionPack/assets/assetbundles/pickupchert ]; then
    echo "getting assetbundle of PickUpChert..."
    mv ../TravelersLocomotionPack/TravelersLocomotionPack/assets/assetbundles/pickupchert* PickUpChert/assets/assetbundles/.
fi
echo "copying ILocomotionAPI.cs..."
cp ../TravelersLocomotionPack/TravelersLocomotionPack/ILocomotionAPI.cs PickUpChert/.

python3 set_script_in_unity.py
