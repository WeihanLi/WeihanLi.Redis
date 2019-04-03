VersionSuffix="preview-$(date -u +"%Y%m%d-%H%M%S")"
echo "VersionSuffix:$VersionSuffix"
sed -i "s/<VersionSuffix.*>.*<\/VersionSuffix>/<VersionSuffix>$VersionSuffix<\/VersionSuffix>/" $BUILD_SOURCESDIRECTORY/build/version.props
cat $BUILD_SOURCESDIRECTORY/build/version.props