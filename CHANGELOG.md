# 9th of February 2023
# v1.0.7

+ SpatialHapticSource clean and split with EventHapticSource
+ SampleScene_XR added; added assets for scene inside - materials; models, prefabs.
+ custom body parts for controllers instead of one body part
+ added 3d quest 2 controller model; solved haptic residual OnTriggerExit and multiple HapticSources playing at the same time on both controllers
+ added XRControllerHapticSource class as example which inherits from SpatialHapticSource
+ stop haptics when exiting trigger collision fix; check for Stop or Continue haptics when exiting collider
+ added AudioHapticsSource to work in sync with Unity AudioSource components

---

# 26th of January 2023
# v1.0.6

+ Fixed wrong typecast
+ Backend update for iOS
+ Fix an issue that could throw errors in MacOS editor play mode

---

# 24th of January 2023
# v1.0.5

+ Added VerboseMode and Play/Stop Parameters for Trigger and Collision (on by default); exposed HapticMaterialId
+ Update HapticSource and SpatialHapticSource with custom editors
+ HSExtension moved to SpatialHapticSource to play on Trigger and Collision (fixed to changed Trigger to false)
+ Correct location for Editor scripts
+ Extracted VibrationControl method to mitigate residual haptics
+ Fixed haptics backend linking issue

---

# 13th of January 2023
# v1.0.4

+ Added missing HAR entry points
+ Remove unused code
+ Fix debug dependencies

---

# 1st of December 2022
# v1.0.2

+ Add slider scripts for the mobile SDK

---

# 29th of November 2022
# v1.0.1

+ Add audio samples to complete the Audio/Haptics synchronized sample assets
