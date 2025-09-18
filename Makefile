# BMM App Development Makefile

.PHONY: help dev-android build-android clean-android restore list-emulators start-emulator install-android check-sdk clean-emulator stop-emulator dev-clean quick-build

# Default target
help:
	@echo "BMM App Development Commands:"
	@echo "  make dev-android     - Build and run Android app in emulator"
	@echo "  make build-android   - Build Android project only"
	@echo "  make clean-android   - Clean Android project"
	@echo "  make restore         - Restore NuGet packages"
	@echo "  make list-emulators  - List available Android emulators"
	@echo "  make start-emulator  - Start default Android emulator"
	@echo "  make install-android - Install app to running emulator"
	@echo "  make check-sdk       - Check Android SDK setup"
	@echo "  make clean-emulator  - Clean emulator data and cache"
	@echo "  make stop-emulator   - Stop running emulator"
	@echo "  make dev-clean       - Clean emulator then build and deploy"
	@echo "  make quick-build     - Fast build without emulator management"

# Configuration
ANDROID_PROJECT = BMM.UI.Android/BMM.UI.Droid.csproj
CONFIGURATION = Debug
ANDROID_SDK_DIR = $(HOME)/Library/Android/sdk

# Auto-detect first available emulator
DEFAULT_EMULATOR = $(shell ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -list-avds 2>/dev/null | head -1)

# Environment variables for Android SDK
export ANDROID_SDK_ROOT = $(ANDROID_SDK_DIR)
export ANDROID_HOME = $(ANDROID_SDK_DIR)
export PATH := $(ANDROID_SDK_DIR)/emulator:$(ANDROID_SDK_DIR)/platform-tools:$(ANDROID_SDK_DIR)/tools:$(PATH)

# Main development target
dev-android: check-sdk start-emulator build-android install-android
	@echo "Android app deployed and running in emulator!"

# Start emulator if not running
start-emulator:
	@echo "Starting Android emulator..."
	@echo "Android SDK: $(ANDROID_SDK_DIR)"
	@echo "Available emulators:"
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -list-avds 2>/dev/null || echo "No emulators found"
	@if [ -z "$(DEFAULT_EMULATOR)" ]; then \
		echo "No Android emulators found. Please create one first."; \
		echo "You can create one using Android Studio or the command line."; \
		exit 1; \
	fi
	@echo "Using emulator: $(DEFAULT_EMULATOR)"
	@if ! ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/platform-tools/adb devices | grep -q emulator; then \
		echo "Starting emulator $(DEFAULT_EMULATOR) with extended storage..."; \
		ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -avd $(DEFAULT_EMULATOR) -no-snapshot-save -partition-size 4096 & \
		echo "Waiting for emulator to boot..."; \
		ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/platform-tools/adb wait-for-device; \
		echo "Waiting for system to be ready..."; \
		timeout 60 $(ANDROID_SDK_DIR)/platform-tools/adb shell 'while [[ -z $$(getprop sys.boot_completed) ]]; do sleep 1; done'; \
		echo "Emulator ready!"; \
	else \
		echo "Emulator already running"; \
	fi

# List available emulators
list-emulators:
	@echo "Available Android emulators:"
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -list-avds

# Restore NuGet packages
restore:
	@echo "Restoring NuGet packages..."
	@dotnet restore \
		--maxcpucount \
		--verbosity minimal

# Build Android project
build-android: restore
	@echo "Building Android project..."
	@dotnet build $(ANDROID_PROJECT) \
		--configuration $(CONFIGURATION) \
		--verbosity minimal \
		--maxcpucount \
		--no-dependencies \
		-p:AndroidSdkDirectory=$(ANDROID_SDK_DIR) \
		-p:AcceptAndroidSDKLicenses=true \
		-p:BuildInParallel=true \
		-p:UseSharedCompilation=true \
		-p:ContinuousIntegrationBuild=false

# Install app to emulator
install-android:
	@echo "Installing app to emulator..."
	@dotnet build $(ANDROID_PROJECT) \
		--configuration $(CONFIGURATION) \
		--verbosity minimal \
		--maxcpucount \
		--no-dependencies \
		-p:AndroidSdkDirectory=$(ANDROID_SDK_DIR) \
		-p:AcceptAndroidSDKLicenses=true \
		-p:BuildInParallel=true \
		-p:UseSharedCompilation=true \
		-t:Install

# Clean Android project
clean-android:
	@echo "Cleaning Android project..."
	@dotnet clean $(ANDROID_PROJECT) --configuration $(CONFIGURATION)
	@rm -rf BMM.UI.Android/bin BMM.UI.Android/obj


# Quick build without emulator management
quick-build:
	@echo "Quick build..."
	@dotnet build $(ANDROID_PROJECT) \
		--configuration $(CONFIGURATION) \
		--verbosity minimal \
		--maxcpucount \
		--no-restore \
		-p:BuildInParallel=true \
		-p:UseSharedCompilation=true

# Check Android SDK setup
check-sdk:
	@echo "Checking Android SDK setup..."
	@echo "Android SDK Directory: $(ANDROID_SDK_DIR)"
	@echo "Checking if SDK directory exists..."
	@if [ -d "$(ANDROID_SDK_DIR)" ]; then \
		echo "SDK directory found"; \
	else \
		echo "SDK directory not found at $(ANDROID_SDK_DIR)"; \
		exit 1; \
	fi
	@echo "Checking emulator..."
	@if [ -f "$(ANDROID_SDK_DIR)/emulator/emulator" ]; then \
		echo "Emulator found"; \
	else \
		echo "Emulator not found at $(ANDROID_SDK_DIR)/emulator/emulator"; \
	fi
	@echo "Checking ADB..."
	@if [ -f "$(ANDROID_SDK_DIR)/platform-tools/adb" ]; then \
		echo "ADB found"; \
	else \
		echo "ADB not found at $(ANDROID_SDK_DIR)/platform-tools/adb"; \
	fi
	@echo "Checking available AVDs..."
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -list-avds

# Stop running emulator
stop-emulator:
	@echo "Stopping Android emulator..."
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/platform-tools/adb emu kill || echo "No emulator running"

# Clean emulator data and cache
clean-emulator: stop-emulator
	@echo "Cleaning emulator data and cache..."
	@if [ -z "$(DEFAULT_EMULATOR)" ]; then \
		echo "No Android emulators found."; \
		exit 1; \
	fi
	@echo "Cleaning emulator: $(DEFAULT_EMULATOR)"
	@echo "This will clear app data and cache but preserve the emulator..."
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/emulator/emulator -avd $(DEFAULT_EMULATOR) -wipe-data -no-window -no-audio &
	@sleep 10
	@ANDROID_SDK_ROOT=$(ANDROID_SDK_DIR) ANDROID_HOME=$(ANDROID_SDK_DIR) $(ANDROID_SDK_DIR)/platform-tools/adb emu kill || echo "Emulator stopped"
	@echo "Emulator cleaned"

# Alternative development target that cleans emulator first
dev-clean: clean-emulator dev-android