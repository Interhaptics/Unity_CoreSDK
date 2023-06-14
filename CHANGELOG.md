# 2023.06.14
# v1.2.2

+ Fix an issue that could prevent some Android phones to not render haptics

---

# 2023.06.05
# v1.2.1

+ OpenXR haptic support

---

# 2023.04.13
# v1.1.1

+ XInput sample scene. Installed input presets in Assets\InputManager for XInput or XR depending on the sample scene.
+ Updated dlls

---

# 2023.03.16
# v1.1.0

+ v1.0.7 and prior Mobile SDK moved to Legacy
+ New Mobile SDK aligned with the main SDK, using Haptic Sources
+ Mobile Sample scene
+ XInput support. Build PC games with haptics for controllers !
+ Dummy Haptic provider removed
+ Unity script bugfixing
+ Haptic engine update

---

# 2023.02.09
# v1.0.7

+ SpatialHapticSource clean and split with EventHapticSource
+ SampleScene_XR added; added assets for scene inside - materials; models, prefabs.
+ custom body parts for controllers instead of one body part
+ added 3d quest 2 controller model; solved haptic residual OnTriggerExit and multiple HapticSources playing at the same time on both controllers
+ added XRControllerHapticSource class as example which inherits from SpatialHapticSource
+ stop haptics when exiting trigger collision fix; check for Stop or Continue haptics when exiting collider
+ added AudioHapticsSource to work in sync with Unity AudioSource components

---

# 2023.01.26
# v1.0.6

+ Fixed wrong typecast
+ Backend update for iOS
+ Fix an issue that could throw errors in MacOS editor play mode

---

# 2023.01.24
# v1.0.5

+ Added VerboseMode and Play/Stop Parameters for Trigger and Collision (on by default); exposed HapticMaterialId
+ Update HapticSource and SpatialHapticSource with custom editors
+ HSExtension moved to SpatialHapticSource to play on Trigger and Collision (fixed to changed Trigger to false)
+ Correct location for Editor scripts
+ Extracted VibrationControl method to mitigate residual haptics
+ Fixed haptics backend linking issue

---

# 2023.01.13
# v1.0.4

+ Added missing HAR entry points
+ Remove unused code
+ Fix debug dependencies

---

# 2022.12.01
# v1.0.2

+ Add slider scripts for the mobile SDK

---

# 2022.11.29
# v1.0.1

+ Add audio samples to complete the Audio/Haptics synchronized sample assets
