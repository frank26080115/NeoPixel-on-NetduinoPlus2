//-----------------------------------------------------------------------------
//
//                   ** WARNING! ** 
//    This file was generated automatically by a tool.
//    Re-running the tool will overwrite this file.
//    You should copy this file to a custom location
//    before adding any customization in the copy to
//    prevent loss of your changes when the tool is
//    re-run.
//
//-----------------------------------------------------------------------------


#include "NeoPixel.h"
#include "NeoPixel_NeoPixel_NeoPixelNative.h"
#include "NeoPixel_NativeCode.h"

using namespace NeoPixel;

void NeoPixelNative::Write( CLR_RT_TypedArray_UINT8 param0, INT32 param1, UINT32 param2, HRESULT &hr )
{
	NeoPixelNativeWrite(param0, param1, param2);
}

