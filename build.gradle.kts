// Top-level build file where you can add configuration options common to all sub-projects/modules.
// 
// VERSION MANAGEMENT:
// The project uses Gradle's version catalog (gradle/libs.versions.toml) for centralized
// dependency version management. However, due to environment limitations during setup,
// versions are temporarily hardcoded here.
//
// IMPORTANT: Keep these versions in sync with gradle/libs.versions.toml
// Once you have proper internet access to Google Maven repository, you can switch back to:
//
// plugins {
//     alias(libs.plugins.android.application) apply false
//     alias(libs.plugins.kotlin.android) apply false
// }
//
// Current versions: AGP 7.4.2, Kotlin 1.8.0 (stable, well-tested)

plugins {
    id("com.android.application") version "7.4.2" apply false
    id("org.jetbrains.kotlin.android") version "1.8.0" apply false
}