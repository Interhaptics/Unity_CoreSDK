//
//  CoreHapticsPlugin.swift
//  Unity Core Haptics Plugin
//
//  Created by Tanay Singhal on 6/18/20.
//  Copyright © 2020 Tanay Singhal. All rights reserved.
//

import Foundation
import CoreHaptics
import UnityFramework_CoreHapticsPrivate

@available(iOS 13.0, *)
@objc public class UnityCoreHaptics: NSObject {
    
    @objc static let shared = UnityCoreHaptics()
    
    static var _engineNeedsStart : Bool = true
    static var _debugMode : Bool = false
    
    static var _engine: CHHapticEngine!
    static var Engine: CHHapticEngine! {
        get {
            if (_engine == nil) {
                CreateEngine()
            }
            if (_engineNeedsStart) {
                StartEngine()
            }
            return _engine
        }
    }
    
    static var _supportsHaptics: Bool?
    static var SupportsHaptics: Bool {
        get {
            if (_supportsHaptics == nil) {
                _supportsHaptics = CHHapticEngine.capabilitiesForHardware().supportsHaptics
                Debug(log: "Supports haptics \(_supportsHaptics!)")
            }
            return _supportsHaptics!
        }
    }

    static var EngineCreatedCallback : HapticCallback?;
    static var EngineErrorCallback : HapticCallback?;
    
    // Name of C# class that will exist in Unity for communicating with this native code
    let kCallbackTarget = "UnityCoreHaptics"
    
    static func Debug(log: String) {
        if (_debugMode) {
            print("--- \(log) ---")
        }
    }
    
    /// - Tag: CreateEngine
    @objc public static func CreateEngine() {
        Debug(log: "Create engine")
        // Create and configure a haptic engine.
        do {
            _engine = try CHHapticEngine()
            try _engine.start()
        } catch let error {
            print("Engine Creation Error: \(error)")
            if let errorCallback = EngineErrorCallback { errorCallback() }
            return
        }

        if let createdCallback = EngineCreatedCallback { createdCallback() }
        _engineNeedsStart = false
        
        // The stopped handler alerts you of engine stoppage due to external causes.
        _engine.stoppedHandler = { reason in
            print("The engine stopped for reason: \(reason.rawValue)")
            switch reason {
            case .audioSessionInterrupt: print("Audio session interrupt")
            case .applicationSuspended: print("Application suspended")
            case .idleTimeout: print("Idle timeout")
            case .systemError: print("System error")
            case .notifyWhenFinished: print("Playback finished")
            @unknown default:
                print("Unknown error")
            }
            
            _engineNeedsStart = true
        }
        
        // The reset handler provides an opportunity for your app to restart the engine in case of failure.
        _engine.resetHandler = {
            Debug(log: "Engine was reset")
            
            // Tell the rest of the app to start the engine the next time a haptic is necessary.
            _engineNeedsStart = true
        }
    }
    
    static func StartEngine() {
        // Start haptic engine to prepare for use.
        if (_engineNeedsStart)
        {
            do {
                Debug(log: "Start engine")
                try _engine.start()
                
                // Indicate that the next time the app requires a haptic, the app doesn't need to call engine.start().
                _engineNeedsStart = false
            } catch let error {
                print("The engine failed to start with error: \(error)")
            }
        }
    }

    @objc public static func SetDebug(bool: Bool) {
        _debugMode = bool
    }
    
    @objc public static func SupportsCoreHaptics() -> Bool {
        return SupportsHaptics
    }
    
    @objc public static func PlayTransientHaptic(intensity: Float, sharpness: Float) {
        Debug(log: "Playing transient haptic")
        if (!SupportsHaptics)
        {
            return;
        }
        
        let clampedIntensity = Clamp(value: intensity, min: 0, max: 1);
        let clampedSharpness = Clamp(value: sharpness, min: 0, max: 1);
        
        let hapticIntensity = CHHapticEventParameter(parameterID: .hapticIntensity, value: clampedIntensity);
        let hapticSharpness = CHHapticEventParameter(parameterID: .hapticSharpness, value: clampedSharpness);
        let event = CHHapticEvent(eventType: .hapticTransient, parameters: [hapticIntensity, hapticSharpness], relativeTime: 0);
        
        do {
            let pattern = try CHHapticPattern(events: [event], parameters: []);
            let player = try Engine.makePlayer(with: pattern);
            try player.start(atTime: CHHapticTimeImmediate);
        }
        catch let error
        {
            print("Failed to play transient pattern: \(error.localizedDescription).");
            if let errorCallback = EngineErrorCallback { errorCallback() }
        }
    }
    
    
    @objc public static func PlayContinuousHaptic(intensity: Float, sharpness: Float, duration: Double) {
        Debug(log: "PlayContinuousHaptic method, intensity : \(intensity) sharpness : \(sharpness) duration : \(duration)");

        if (!SupportsHaptics)
        {
            return;
        }

        let clampedIntensity = Clamp(value: intensity, min: 0, max: 1);
        let clampedSharpness = Clamp(value: sharpness, min: 0, max: 1);

        let hapticIntensity = CHHapticEventParameter(parameterID: .hapticIntensity, value: clampedIntensity);
        let hapticSharpness = CHHapticEventParameter(parameterID: .hapticSharpness, value: clampedSharpness);
        let event = CHHapticEvent(eventType: .hapticContinuous, parameters: [hapticIntensity, hapticSharpness], relativeTime: 0, duration: duration);

        do
        {
            let pattern = try CHHapticPattern(events: [event], parameters: []);
            let player = try Engine.makePlayer(with: pattern);
            try player.start(atTime: CHHapticTimeImmediate);
        }
        catch let error
        {
            print("Failed to play continuous pattern: \(error.localizedDescription).");
            if let errorCallback = EngineErrorCallback { errorCallback() }
        }
    }

    @objc public static func PlayHapticsFromJSON(str: String) {
        Debug(log: "Playing haptic from JSON")
        StartEngine();
        if (!SupportsHaptics)
        {
            return;
        }
        
        do
        {
            let jsonData = str.data(using: String.Encoding.utf8);
            
            try Engine.playPattern(from: jsonData!);
            
            // Engine.notifyWhenPlayersFinished { (Error) -> CHHapticEngine.FinishedAction in
            //     if let completeCallback = onComplete { completeCallback() }
            //     return CHHapticEngine.FinishedAction.leaveEngineRunning;
            // }
        }
        catch let error
        {
            print("Failed to play pattern from JSON: \(error.localizedDescription).");
            if let errorCallback = EngineErrorCallback { errorCallback() }
        }
    }
    
    @objc public static func PlayHapticsFromFile(path: String) {
        Debug(log: "Playing haptic from file \(path)")
        
        if !SupportsHaptics {
            return
        }
        
        do {
            try Engine.playPattern(from: URL(fileURLWithPath: path))

            // Engine.notifyWhenPlayersFinished { (Error) -> CHHapticEngine.FinishedAction in
            //     if let completeCallback = onComplete { completeCallback() }
            //     return CHHapticEngine.FinishedAction.leaveEngineRunning;
            // }

        } catch {
            print("Failed to play pattern from file: \(path)");
            if let errorCallback = EngineErrorCallback { errorCallback() }
        }
    }
    
    //MARK: Callbacks
    @objc public static func RegisterEngineCreated(callback: HapticCallback?)
    {
        EngineCreatedCallback = callback;
    }

    @objc public static func RegisterEngineError(callback: @escaping HapticCallback)
    {
        EngineErrorCallback = callback;
    }
    
    @objc public static func CancelHaptics()
    {
        Engine.stop();
        _engineNeedsStart = true;
    }
    
    //MARK: Helper functions
    static func Clamp(value: Float, min: Float, max: Float ) -> Float {
        if (value > max)
        {
            return max;
        }
        if (value < min)
        {
            return min;
        }
        return value;
    }
}
