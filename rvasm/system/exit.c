#include "../simlib/stddef.h"

#ifdef __USER_CODE

__attribute__((naked)) void exit() {
    asm volatile ("ret"::);
}

#else

__attribute__((naked)) void exit() {
    __ebreak();
}

#endif