#!/bin/bash

# Devices list
DEVICES=("mayfly" "unicorn" "cupid" "zeus" "diting")

# Build
. build/envsetup.sh
mkdir -p "$HOME/out"

for DEVICE in "${DEVICES[@]}"; do
	echo "====== Building for device: $DEVICE ======"
	riseup "$DEVICE" user

	# Build fastboot
	rise fb -j$(nproc --all) | tee "log_${DEVICE}_fb.txt"
	fb_file=$(find "out/target/product/$DEVICE" -name "RisingOS_Revived-*-${DEVICE}-fastboot.zip" 2>/dev/null)
	if [[ -f "$fb_file" ]]; then
		mv "$fb_file" ../out/
	else
		echo "❌ Fastboot zip not found for $DEVICE"
	fi

	# Build OTA
	rise b -j$(nproc --all) | tee "log_${DEVICE}_ota.txt"
	ota_file=$(find "out/target/product/$DEVICE" -name "RisingOS_Revived-*-${DEVICE}-ota.zip" 2>/dev/null)
	if [[ -f "$ota_file" ]]; then
		mv "$ota_file" ../out/
	else
		echo "❌ OTA zip not found for $DEVICE"
	fi

	# Xoá thư mục product nếu tồn tại
	PRODUCT_DIR="out/target/product/$DEVICE"
	if [[ -d "$PRODUCT_DIR" ]]; then
		rm -rf "$PRODUCT_DIR"
		echo "🧹 Đã xoá thư mục build: $PRODUCT_DIR"
	fi
done
