## [1.0.5](https://github.com/fuqunaga/PrefsGUISyncForMirror/compare/v1.0.4...v1.0.5) (2024-09-26)


### Bug Fixes

* error when entering PlayMode while PrefsGUIEditorWindow is open. ([4402746](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/4402746f58595e44e912485441a9034f222c9ae6))
* Over capacity error with Spawn data in PrefsGUISync. Split transmission. ([5915707](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/5915707e35b99e6a06485a05dab0876292a89da4))
* PrefsGUISyncEditorRosettaUI support new RosettaUI style ([2bcf39c](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/2bcf39c04fe981e2f63ae36676a995cf5b16d83b))
* SyncDicionary callback warning ([a15dedf](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/a15dedf2fa37027e585587a6890bfc64826321da))


### Performance Improvements

* Serialize only changed Prefs in PrefsGUISyncForMirror.SendPrefs(). ([30d4222](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/30d4222731f2c259727aa0be1ec3d1f931aabf36))

## [1.0.4](https://github.com/fuqunaga/PrefsGUISyncForMirror/compare/v1.0.3...v1.0.4) (2023-04-24)


### Bug Fixes

* PrefsBounds/PrefsBoundsInt is not sync ([a082365](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/a0823658c5ff3c012ae451155b6303fa9dc9dc90))

## [1.0.3](https://github.com/fuqunaga/PrefsGUISyncForMirror/compare/v1.0.2...v1.0.3) (2023-03-10)


### Bug Fixes

* **upgrade:** Mirror73 ([54787c9](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/54787c9b12ce843690e279d9ccc4f88f39d4b3a6))

## [1.0.2](https://github.com/fuqunaga/PrefsGUISyncForMirror/compare/v1.0.1...v1.0.2) (2022-08-26)


### Bug Fixes

* Server value was not set until the prefs value was referenced on the client side (until alreadyGet==true) ([676affd](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/676affde3bcd904c7791e7a0cc9bcb84645b3a1b))

## [1.0.1](https://github.com/fuqunaga/PrefsGUISyncForMirror/compare/v1.0.0...v1.0.1) (2022-07-14)


### Bug Fixes

* add .releaserc.yhm for semantic-release ([235a1e2](https://github.com/fuqunaga/PrefsGUISyncForMirror/commit/235a1e256290e41d6ce5be33a8ebe0b3f22417f5))
