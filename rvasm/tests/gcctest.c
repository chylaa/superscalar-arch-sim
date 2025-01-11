#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

int main() {
    uint32_t a = 42;
    uint32_t b = 69;
    WRITE(FIRST_FREE_ADDR, (a+b));
    return 0;
}
