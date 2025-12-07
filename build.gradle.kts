// Top-level build file where you can add configuration options common to all sub-projects/modules.
// Note: The version catalog approach (libs.versions.toml) is preferred but requires
// network access to Google Maven repository. In production environments, use:
// plugins {
//     alias(libs.plugins.android.application) apply false
//     alias(libs.plugins.kotlin.android) apply false
// }

plugins {
    id("com.android.application") version "7.4.2" apply false
    id("org.jetbrains.kotlin.android") version "1.8.0" apply false
}