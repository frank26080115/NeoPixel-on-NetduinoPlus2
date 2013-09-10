#ifndef _NEOPIXEL_NATIVECODE_H_
#define _NEOPIXEL_NATIVECODE_H_

#include <TinyCLR_Interop.h>
#include "NeoPixel.h"
#include "NeoPixel_NeoPixel_NeoPixelNative.h"
#include "NeoPixel_NativeCode.h"

void NeoPixelNativeWrite(CLR_RT_TypedArray_UINT8 data, INT32 count, UINT32 pin);

#endif