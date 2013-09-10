#include <TinyCLR_Interop.h>
//#include <Time_decl.h> // no longer needed

#if defined(PLATFORM_ARM_STM32F4_ANY)
#include <DeviceCode/stm32f4xx.h>
#elif defined(PLATFORM_ARM_STM32F2_ANY)
#include <DeviceCode/stm32f2xx.h>
#else
#include <DeviceCode/stm32f10x.h>
#endif

#include "NeoPixel.h"
#include "NeoPixel_NeoPixel_NeoPixelNative.h"
#include "NeoPixel_NativeCode.h"

#define NP_DELAY_LOOP(x) do { volatile int ___i = (x); while (___i--) { asm volatile ("nop"); } } while (0) // every loop is 5 ticks of CPU clock (using GCC with full optimization)

// these numbers were tuned specifically for Netduino Plus 2, STM32F4 running at 168MHz
// tuning done using an o'scope
#define NP_WAIT_T1H() NP_DELAY_LOOP(12) // 700 ns needed, tested 12 = 680ns
#define NP_WAIT_T1L() NP_DELAY_LOOP(10) // 600 ns needed, tested 10 = 580ns
#define NP_WAIT_T0H() NP_DELAY_LOOP(5)  // 350 ns needed, tested 5  = 350ns
#define NP_WAIT_T0L() NP_DELAY_LOOP(15) // 800 ns needed, tested 15 = 800ns

// this is found in STM32_time_functions.cpp but the prototype is missing from Time_decl.h , so we declare it here
//int CPU_SystemClocksToMicroseconds(int Ticks);

void NeoPixelNativeWrite(CLR_RT_TypedArray_UINT8 data, INT32 count, UINT32 pin)
{
	GPIO_TypeDef* port = ((GPIO_TypeDef *) (GPIOA_BASE + ((pin & 0xF0) << 6)));
	UINT16 *_BSRRH, *_BSRRL;
	// note: I think the BSRRx registers are reversed in the declaration, hence why this code seems reversed
	#if defined(PLATFORM_ARM_STM32F2_ANY) || defined(PLATFORM_ARM_STM32F4_ANY)
	_BSRRH = (UINT16*)&(port->BSRRL); _BSRRL = (UINT16*)&(port->BSRRH);
	#else
	_BSRRH = (UINT16*)&(((UINT16*)(&port->BSRR))[1]);  _BSRRL = (UINT16*)&(((UINT16*)(&port->BSRR))[0]);
	#endif
	UINT8 portBit = 1 << (pin & 0x0F);
	*_BSRRL = portBit; // clears the pin low

	/*
	// old timing code, not needed, because CLR is slow enough, we don't need to add our own delay

	// wait for previous latch
	UINT64 curTicks = HAL_Time_CurrentTicks();
	UINT64 elapsedTicks;
	if (curTicks >= *lastWriteTime) {
		elapsedTicks = curTicks - *lastWriteTime;
	}
	else {
		elapsedTicks = (*lastWriteTime) - curTicks;
	}

	int remainingTime = 50 - CPU_SystemClocksToMicroseconds((int)elapsedTicks);
	if (remainingTime > 0)
	{
		HAL_Time_Sleep_MicroSeconds_InterruptEnabled(remainingTime);
	}
	//*/

	__disable_irq(); // disable interrupts so that timing is accurate

	// pin is already assumed low
	// data is already in GRB order
	INT32 byteIdx, bitMask;
	INT32 cx3 = count * 3; // 3 bytes per neopixel
	for (byteIdx = 0; byteIdx < cx3; byteIdx++) // for every byte in the array
	{
		for (bitMask = 0x80; bitMask > 0; bitMask >>= 1) // MSB first
		{
			*_BSRRH = portBit; // sets the pin high
			if ((data[byteIdx] & bitMask) != 0)
			{
				NP_WAIT_T1H();
				*_BSRRL = portBit; // clears the pin low
				NP_WAIT_T1L();
			}
			else
			{
				NP_WAIT_T0H();
				*_BSRRL = portBit; // clears the pin low
				NP_WAIT_T0L();
			}
		}
	}

	__enable_irq();

	/*
	// old timing code, not needed, because CLR is slow enough, we don't need to add our own delay
	*lastWriteTime = HAL_Time_CurrentTicks();
	//*/

	// old timing code, not needed, because CLR is slow enough, we don't need to add our own delay
	// delay not needed because the CLR method call overhead is much longer than 50 microseconds
	//HAL_Time_Sleep_MicroSeconds_InterruptEnabled(50);
}