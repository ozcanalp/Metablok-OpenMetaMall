#pragma once

#include "string"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
@interface AvatarSDKImagePicker: NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>
- (void)getPhoto;
- (void)getPhotoFromLibrary;
@end
extern "C" void getPhoto(const char* className, const char* callbackFunction);
extern "C" void getPhotoFromLibrary(const char* className, const char* callbackFunction);
