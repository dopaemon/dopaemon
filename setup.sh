#!/bin/bash

set -e

PWDIR=$(pwd)

# Config
BRANCH_DEVICE="lineage-22.2"
BRANCH_VENDOR="lineage-22.2"
BRANCH_KERNEL="lineage-22.2"
MY_BRANCH="RisingOS-15"

GIT_DEVICE="https://github.com/LineageOS"
MY_DEVICE="https://github.com/Android-Unofficial"
GIT_VENDOR="https://github.com/TheMuppets"
GIT_KERNEL="https://github.com/LineageOS"

# Devices list
DEVICES=("mayfly" "unicorn" "cupid" "zeus" "diting")

# Clone device trees
declare -A DEVICE_REPOS=(
	["sm8450-common"]="${MY_DEVICE}/android_device_xiaomi_sm8450-common"
	["cupid"]="${GIT_DEVICE}/android_device_xiaomi_cupid"
	["zeus"]="${GIT_DEVICE}/android_device_xiaomi_zeus"
	["mayfly"]="${GIT_DEVICE}/android_device_xiaomi_mayfly"
	["unicorn"]="${GIT_DEVICE}/android_device_xiaomi_unicorn"
 	["diting"]="${GIT_DEVICE}/android_device_xiaomi_diting"
)

for name in "${!DEVICE_REPOS[@]}"; do
	target_dir="${PWDIR}/device/xiaomi/${name}"
	if [ ! -d "$target_dir" ]; then
		echo "Cloning device tree: $name"
		branch="${MY_BRANCH}"
		[[ "$name" != "sm8450-common" ]] && branch="${BRANCH_DEVICE}"
		git clone -b "$branch" --single-branch "${DEVICE_REPOS[$name]}" "$target_dir"
	fi
done

# Clone vendor blobs
for name in "sm8450-common" "cupid" "zeus" "mayfly" "unicorn" "diting"; do
	target_dir="${PWDIR}/vendor/xiaomi/${name}"
	repo="${GIT_VENDOR}/proprietary_vendor_xiaomi_${name}"
	if [ ! -d "$target_dir" ]; then
		echo "Cloning vendor: $name"
		git clone -b "${BRANCH_VENDOR}" --single-branch --depth=1 "$repo" "$target_dir"
	fi
done

# Clone kernel
if [ ! -d "${PWDIR}/kernel/xiaomi/sm8450" ]; then
	echo "Cloning kernel main tree"
	git clone -b Rebase --single-branch https://github.com/dopaemon/android_kernel_xiaomi_common.git "${PWDIR}/kernel/xiaomi/sm8450"
	cd "${PWDIR}/kernel/xiaomi/sm8450"
	git submodule update --init --recursive
	cd "$PWDIR"
fi

for kernel_module in "sm8450-modules" "sm8450-devicetrees"; do
	target_dir="${PWDIR}/kernel/xiaomi/${kernel_module}"
	if [ ! -d "$target_dir" ]; then
		echo "Cloning kernel module: $kernel_module"
		git clone -b "${BRANCH_KERNEL}" --single-branch "${GIT_KERNEL}/android_kernel_xiaomi_${kernel_module}" "$target_dir"
	fi
done

# Clone hardware
HARDWARE_DIR="${PWDIR}/hardware/xiaomi"
if [ ! -d "$HARDWARE_DIR" ]; then
	echo "Cloning hardware/xiaomi"
	git clone -b "${BRANCH_DEVICE}" --single-branch "${GIT_DEVICE}/android_hardware_xiaomi.git" "$HARDWARE_DIR"
fi

# Clone priv keys
KEYS_DIR="${PWDIR}/vendor/lineage-priv/keys"
if [ ! -d "$KEYS_DIR" ]; then
	echo "Cloning vendor keys"
	mkdir -p "$(dirname "$KEYS_DIR")"
	git clone https://github.com/Android-Unofficial/android_vendor_lineage-priv_keys.git "$KEYS_DIR"
fi
