//-----------------------------------------------------------------------------
//
//    ** DO NOT EDIT THIS FILE! **
//    This file was generated by a tool
//    re-running the tool will overwrite this file.
//
//-----------------------------------------------------------------------------


#include "NeoPixel.h"
#include "NeoPixel_NeoPixel_NeoPixelNative.h"

using namespace NeoPixel;


HRESULT Library_NeoPixel_NeoPixel_NeoPixelNative::Write___STATIC__VOID__SZARRAY_U1__I4__U4( CLR_RT_StackFrame& stack )
{
    TINYCLR_HEADER(); hr = S_OK;
    {
        CLR_RT_TypedArray_UINT8 param0;
        TINYCLR_CHECK_HRESULT( Interop_Marshal_UINT8_ARRAY( stack, 0, param0 ) );

        INT32 param1;
        TINYCLR_CHECK_HRESULT( Interop_Marshal_INT32( stack, 1, param1 ) );

        UINT32 param2;
        TINYCLR_CHECK_HRESULT( Interop_Marshal_UINT32( stack, 2, param2 ) );

        NeoPixelNative::Write( param0, param1, param2, hr );
        TINYCLR_CHECK_HRESULT( hr );
    }
    TINYCLR_NOCLEANUP();
}
