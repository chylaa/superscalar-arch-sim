
#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

#define WIDTH (4)

const uint32_t iterations = 1000;
const uint32_t add = 2;
uint32_t result[WIDTH] = {0};

inline void __attribute__((always_inline)) 
ilp(){
    for (int i = 0; i < iterations; i++){
        //  Independent adds. Up to WIDTH adds can be run in parallel.
        for (int j = 0; j < WIDTH; j++){
            result[j] += add;
        }
    }
}

int main() {
    ilp();
}