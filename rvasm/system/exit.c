#include "../simlib/stddef.h"

#ifdef __cplusplus
extern "C" {

/* Call destructors for static objects */
void __exit_call_fini_array() {
    auto array = _fini_array_start;
    while (array < _fini_array_end) {
        (*array)();
        array++;
    }
}
#else
void __exit_call_fini_array() {}
#endif

/* Finalize environment */
void _fini() {
    __exit_call_fini_array();
}

#ifdef __USER_CODE

__attribute__((naked)) void _exit() {
    asm volatile ("ret"::);
}

#else

__attribute__((naked)) void _exit() {
    __ebreak();
}

#endif /* __USER_CODE */

#ifdef __cplusplus
}
#endif