#import <Foundation/Foundation.h>

extern "C" {
long storage_getFreeDiskSpace() {
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
