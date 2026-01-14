#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/cpp/string_view.hpp"


int main() {
    std::string_view s{"Hello"};
    uint32_t a = 42;
    uint32_t b = 69;
    uint32_t c = s.length();
    WRITE(FIRST_FREE_ADDR, (a+b+c));
    return 0;
}
