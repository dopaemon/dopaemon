#!/bin/bash

# Default values
SF_USERNAME=""
FILE_PATH=""
VERSION=""
BUILD_TYPE=""
DEVICE_NAME=""
MAJOR_VERSION=""

# Prompt for SourceForge username
read -p "Enter your SourceForge username: " SF_USERNAME

# Prompt for upload type
PS3="Select upload type: "
select upload_type in "ROM (zip)" "IMG (img)"; do
    if [[ -n "$upload_type" ]]; then
        break
    else
        echo "Invalid choice."
    fi
done

if [[ "$upload_type" == "ROM (zip)" ]]; then
    # ROM Upload Logic
    if [[ -z "$FILE_PATH" ]]; then
        read -e -p "Enter the full path of the file: " FILE_PATH
    fi

    # Check if file exists
    if [[ ! -f "$FILE_PATH" ]]; then
        echo "Error: File does not exist."
        exit 1
    fi

    # Check if the file name matches the RisingOS Revived pattern
    FILENAME=$(basename "$FILE_PATH")
    if [[ ! "$FILENAME" =~ ^RisingOS_Revived-.*\.zip$ ]]; then
        echo "Error: File name does not match RisingOS Revived pattern."
        exit 1
    fi

    # Check if the file name ends with -ota.zip
    if [[ ! "$FILENAME" =~ -ota\.zip$ ]]; then
        echo "Error: ROM file name must end with -ota.zip"
        exit 1
    fi

    if [[ -z "$VERSION" ]]; then
        if [[ -z "$BUILD_TYPE" || -z "$DEVICE_NAME" ]]; then
            PS3="Select build type: "
            select build_type_option in "VANILLA" "CORE" "GAPPS"; do
                if [[ -n "$build_type_option" ]]; then
                    BUILD_TYPE="$build_type_option"
                    break
                else
                    echo "Invalid choice."
                fi
            done

            read -p "Enter the device codename: " DEVICE_NAME
        fi
        read -p "Enter the Major Version: " MAJOR_VERSION
    else
        MAJOR_VERSION="$VERSION"
        if [[ -z "$BUILD_TYPE" || -z "$DEVICE_NAME" ]]; then
            PS3="Select build type: "
            select build_type_option in "VANILLA" "CORE" "GAPPS"; do
                if [[ -n "$build_type_option" ]]; then
                    BUILD_TYPE="$build_type_option"
                    break
                else
                    echo "Invalid choice."
                fi
            done
            read -p "Enter the device codename: " DEVICE_NAME
        fi
    fi

    # Define SourceForge upload directory
    SF_PROJECT="risingos-revived"
    SF_BASE_PATH="${MAJOR_VERSION}.x/${BUILD_TYPE}/${DEVICE_NAME}"
    
    # Create remote directory using ssh
    echo "Creating remote directory structure..."
    ssh "${SF_USERNAME}@frs.sourceforge.net" "mkdir -p /home/frs/project/${SF_PROJECT}/${SF_BASE_PATH}"

    # Upload file using rsync
    echo "Uploading $FILE_PATH to SourceForge..."
    rsync -av --progress "$FILE_PATH" "${SF_USERNAME}@frs.sourceforge.net:/home/frs/project/${SF_PROJECT}/${SF_BASE_PATH}/"

    if [[ $? -eq 0 ]]; then
        echo "Upload completed successfully."
    else
        echo "Error: Upload failed. Please check your SourceForge credentials and try again."
        exit 1
    fi

elif [[ "$upload_type" == "IMG (img)" ]]; then
    # IMG Upload Logic
    read -p "Enter the Major Version: " MAJOR_VERSION
    PS3="Select build type: "
    select build_type_option in "VANILLA" "CORE" "GAPPS"; do
        if [[ -n "$build_type_option" ]]; then
            BUILD_TYPE="$build_type_option"
            break
        else
            echo "Invalid choice."
        fi
    done
    read -p "Enter the device codename: " DEVICE_NAME

    # Prompt for .img files (multiple)
    read -e -p "Enter the full path(s) of the .img file(s) (space-separated): " IMG_FILES

    # Define SourceForge upload directory
    SF_PROJECT="risingos-revived"
    SF_BASE_PATH="${MAJOR_VERSION}.x/${BUILD_TYPE}/${DEVICE_NAME}/IMGs"

    # Create remote directory using ssh
    echo "Creating remote directory structure..."
    ssh "${SF_USERNAME}@frs.sourceforge.net" "mkdir -p /home/frs/project/${SF_PROJECT}/${SF_BASE_PATH}"

    # Upload each IMG file using rsync
    for IMG_FILE in $IMG_FILES; do
        if [[ -f "$IMG_FILE" && "$IMG_FILE" =~ \.img$ ]]; then
            echo "Uploading $IMG_FILE..."
            rsync -av --progress "$IMG_FILE" "${SF_USERNAME}@frs.sourceforge.net:/home/frs/project/${SF_PROJECT}/${SF_BASE_PATH}/"
            if [[ $? -ne 0 ]]; then
                echo "Error: Upload of $IMG_FILE failed. Please check your SourceForge credentials and try again."
                exit 1
            fi
        else
            echo "Error: $IMG_FILE is not a valid .img file."
        fi
    done

    echo "IMG files upload completed successfully."

else
    echo "Invalid upload type."
    exit 1
fi
