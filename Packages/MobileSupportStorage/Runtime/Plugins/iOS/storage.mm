#import <Foundation/Foundation.h>

extern "C" {
long storage_getFreeDiskSpace(bool includeDeletableCaches) {
    if ([UIDevice currentDevice].systemVersion.floatValue >= 11.0) {
        NSURLResourceKey resourceKey = NSURLVolumeAvailableCapacityKey; // real free space
        if (includeDeletableCaches) {
            // value similar to showed in System
            resourceKey = NSURLVolumeAvailableCapacityForImportantUsageKey;
        }
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSURL *fileURL = [NSURL fileURLWithPath:[paths firstObject]];
        NSError *error = nil;
        NSDictionary *results = [fileURL resourceValuesForKeys:@[resourceKey] error:&error];
        
        if (!results) {
            return -1;
        }
        
        uint64_t totalFreeSpace = [results[resourceKey] unsignedLongLongValue];
        return totalFreeSpace;
    } else {
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSDictionary *attributes = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error: nil];

        if (!attributes) {
            return -1;
        }

        NSNumber *freeFileSystemSizeInBytes = [attributes objectForKey:NSFileSystemFreeSize];
        uint64_t totalFreeSpace = [freeFileSystemSizeInBytes unsignedLongLongValue];
        return totalFreeSpace;
    }
}
}
