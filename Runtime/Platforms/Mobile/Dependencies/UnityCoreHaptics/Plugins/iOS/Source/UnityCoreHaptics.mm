#import "UnityCoreHaptics-Bridging-Header.h"
#import "UnityFramework/UnityFramework-Swift.h"

extern "C"
{
  // Other functions
  void _coreHapticsCreateEngine(bool value)
  {
    [UnityCoreHaptics CreateEngine];
  }

  void _coreHapticsStopEngine()
  {
    [UnityCoreHaptics CancelHaptics];
  }

  void _coreHapticsSetDebug(bool value)
  {
    [UnityCoreHaptics SetDebugWithBool:value];
  }

  bool _coreHapticsSupportsCoreHaptics()
  {
    return [UnityCoreHaptics SupportsCoreHaptics];
  }

  void _coreHapticsPlayTransientHaptic(float intensity, float sharpness)
  {
    [UnityCoreHaptics PlayTransientHapticWithIntensity:intensity sharpness:sharpness];
  }

  void _coreHapticsPlayContinuousHaptic(float intensity, float sharpness, float duration)
  {
    [UnityCoreHaptics PlayContinuousHapticWithIntensity:intensity sharpness:sharpness duration:duration];
  }

  void _coreHapticsPlayHapticsFromJSON(const char* jsonStr)
  {
      if (jsonStr == nil)
      {
          printf("jsonStr is nil");
          return;
      }

      NSString *str = [[NSString alloc] initWithUTF8String:jsonStr];

      [UnityCoreHaptics PlayHapticsFromJSONWithStr:str];
  }

  // Note: path relative to Data/Raw folder
  void _coreHapticsPlayHapticsFromFile(const char* path)
  {
      if (path == nil)
      {
          printf("file path is nil");
          return;
      }

      NSString *str = [[NSString alloc] initWithUTF8String:path];
      
      [UnityCoreHaptics PlayHapticsFromFileWithPath:str];
  }

  // Callbacks
  void _coreHapticsRegisterEngineCreated(HapticCallback callback)
  {
    [UnityCoreHaptics RegisterEngineCreatedWithCallback:callback];
  }

  void _coreHapticsRegisterEngineError(HapticCallback callback)
  {
    [UnityCoreHaptics RegisterEngineErrorWithCallback:callback];
  }
}
