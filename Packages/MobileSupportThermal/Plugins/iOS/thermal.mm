#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*Callback)(int);

static NSObject* s_observer = Nil;

void thermal_startMonitoring(Callback callback) {
    if (s_observer != Nil) return;
    
    s_observer = [[NSNotificationCenter defaultCenter] addObserverForName:NSProcessInfoThermalStateDidChangeNotification
                                                                   object:nil
                                                                    queue:[NSOperationQueue mainQueue]
                                                               usingBlock:^(NSNotification * _Nonnull notification) {
        NSProcessInfoThermalState state = [[NSProcessInfo processInfo] thermalState];
        (callback)((int)state);
    }];
    
    // report current value
    NSProcessInfoThermalState state = [[NSProcessInfo processInfo] thermalState];
    (callback)((int)state);
}

void thermal_stopMonitoring() {
    if (s_observer == Nil) return;
    
    [[NSNotificationCenter defaultCenter] removeObserver:s_observer name:NSProcessInfoThermalStateDidChangeNotification object:nil];
    s_observer = Nil;
}

#ifdef __cplusplus
}
#endif
